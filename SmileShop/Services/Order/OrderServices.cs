using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Helpers;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Services
{
    public class OrderServices : ServiceBase, IOrderServices
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IStockServices _stockServices;

        public OrderServices(AppDBContext dbContext, IMapper mapper, IHttpContextAccessor httpContext, IStockServices stockServices) : base(dbContext, mapper, httpContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContext;
            _stockServices = stockServices;
        }

        public async Task<ServiceResponseWithPagination<List<OrderDTO>>> GetAll(PaginationDto pagination = null, OrderFilterDTO OrderFilter = null, DataOrderDTO ordering = null)
        {

            // Quering data
            var query = _dbContext.Order.AsQueryable();

            // Filtering data
            query = Filter(query, OrderFilter);


            // Ordering
            if (!(ordering is null))
            {
                var columns = new List<string> { "Id", "Date", "CreatedBy" };

                if (columns.Exists(x => x == ordering.OrderBy))
                {
                    if (ordering.OrderBy == "CreatedBy") ordering.OrderBy = "CreatedByUser.Username";

                    var property = $"{ordering.OrderBy}";

                    if (!String.IsNullOrEmpty(ordering.Sort) && ordering.Sort.ToLower() == "desc")
                    {
                        query = ApplyOrder(query, property, "OrderByDescending");
                    }
                    else
                    {
                        query = ApplyOrder(query, property, "OrderBy");
                    }
                }
            }


            // Pagination
            var paginationResult = await _httpContext.HttpContext.InsertPaginationParametersInResponse(query, pagination.RecordsPerPage, pagination.Page);

            // Generate result
            var result = await query.Paginate(pagination)
                                    .Include(entity => entity.OrderDetails)
                                    .Include(entity => entity.CreatedByUser)
                                    .ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                throw new InvalidOperationException("Order is not Exist");

            // Mapping
            var dto = _mapper.Map<List<OrderDTO>>(result);

            // Return Results
            return ResponseResultWithPagination.Success<List<OrderDTO>>(dto, paginationResult);
        }

        public async Task<ServiceResponse<OrderDTO>> Get(int OrderId)
        {

            if (OrderId <= 0)
                throw new ArgumentOutOfRangeException("OrderId", "Id must be greater than 0.");

            // Quering data
            var result = await _dbContext.Order
                                    .Include(o => o.CreatedByUser)
                                    .Where(o => o.Id == OrderId)
                                    .FirstOrDefaultAsync();

            // Return error if count is 0
            if (result is null)
                throw new InvalidOperationException("Order is not Exist");

            // Mapping
            var dto = _mapper.Map<OrderDTO>(result);


            // Return Results
            return ResponseResult.Success<OrderDTO>(dto);
        }
        public async Task<ServiceResponse<List<OrderDetailDTO>>> GetOrderDetails(int OrderId)
        {

            if (OrderId <= 0)
                throw new ArgumentOutOfRangeException("OrderId", "Id must be greater than 0.");

            // Quering data
            var result = await _dbContext.OrderDetail
                                    .Include(o => o.Product)
                                    .Where(o => o.OrderId == OrderId)
                                    .ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                throw new InvalidOperationException("Order is not Exist");

            // Mapping
            var dto = _mapper.Map<List<OrderDetailDTO>>(result);


            // Return Results
            return ResponseResult.Success<List<OrderDetailDTO>>(dto);
        }

        public async Task<ServiceResponse<OrderDTO>> Add(OrderAddDTO addOrder)
        {
            // User must be presented to perform this method
            if (String.IsNullOrEmpty(GetUserId()))
                throw new UnauthorizedAccessException("User must be presented to perform this method");

            if (addOrder.Discount > 1 || addOrder.Discount < 0)
                throw new ArgumentOutOfRangeException("Discount", "Discount must be percentage range of 0.00 to 1.00");

            var listOrderDetail = _mapper.Map<List<OrderDetailProcessDTO>>(addOrder.OrderDetails);
            int orderQuantity = 0;
            decimal orderTotal = 0;
            decimal orderNet = 0;

            if (listOrderDetail.Count == 0)
                throw new ArgumentNullException("Order Detail", "Order Detail must be exist");

            //For each OrderDetail check if Product is sufficient.

            for (int i = 0; i < listOrderDetail.Count; i++)
            {

                int balance;
                decimal price = 0;

                (balance, price) = await _stockServices.ProductIsSufficient(listOrderDetail[i].ProductId, new ProductStockAddDTO { Debit = 0, Credit = listOrderDetail[i].Quantity });

                listOrderDetail[i].Price = price;
                listOrderDetail[i].DiscountPrice = price * addOrder.Discount;
                orderTotal += listOrderDetail[i].Quantity * price;

                var stock = new Stock { ProductId = listOrderDetail[i].ProductId, Debit = 0, Credit = listOrderDetail[i].Quantity, CreatedByUserId = Guid.Parse(GetUserId()), CreatedDate = Now(), Remark = $"Deduct from Order", StockBefore = balance };

                await _dbContext.AddAsync(stock);
            }
            orderQuantity = listOrderDetail.Sum(_ => _.Quantity);
            orderNet = orderTotal - listOrderDetail.Sum(_ => _.DiscountPrice);

            // Create New Order 
            Order newOrder = _mapper.Map<Order>(addOrder);

            newOrder.CreatedByUserId = Guid.Parse(GetUserId());
            newOrder.CreatedDate = Now();
            newOrder.Total = orderTotal;
            newOrder.ItemCount = orderQuantity;
            newOrder.Net = orderNet;
            newOrder.OrderDetails = _mapper.Map<List<OrderDetail>>(listOrderDetail);

            await _dbContext.Order.AddAsync(newOrder);

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<OrderDTO>(newOrder);

            // Add User Details
            dto.CreatedByUserName = GetUsername();

            return ResponseResult.Success<OrderDTO>(dto, $"/api/orders/{newOrder.Id}/detail");

        }

        public async Task<ServiceResponse<OrderDTO>> Delete(int OrderId)
        {

            if (OrderId <= 0)
                throw new ArgumentOutOfRangeException("OrderId", "Id must be greater than 0.");

            // Quering data
            var result = await _dbContext.Order.FindAsync(OrderId);

            // Return error if count is 0
            if (result is null)
                throw new InvalidOperationException("Order is not Exist");

            var orderDetails = await _dbContext.OrderDetail
                                               .Where(od => od.OrderId == OrderId)
                                               .FirstOrDefaultAsync();

            _dbContext.OrderDetail.RemoveRange(orderDetails);
            await _dbContext.SaveChangesAsync();

            _dbContext.Order.Remove(result);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<OrderDTO>(result);

            // Return Results
            return ResponseResult.Success<OrderDTO>(dto);
        }

        public IQueryable<Order> Filter(IQueryable<Order> query, OrderFilterDTO filter)
        {

            if (!(filter.StartDate is null) && !(filter.EndDate is null))
                query = query.Where(x => x.CreatedDate >= filter.StartDate && x.CreatedDate <= filter.EndDate);

            return query;
        }


    }
}
