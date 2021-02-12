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

        public async Task<ServiceResponseWithPagination<List<OrderOnlyDTO>>> GetAll(PaginationDto pagination = null, OrderFilterDTO OrderFilter = null, DataOrderDTO ordering = null)
        {

            // Quering data
            var query = _dbContext.Order.AsQueryable();

            // Filtering data
            query = filter(query, OrderFilter);


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
                return ResponseResultWithPagination.Failure<List<OrderOnlyDTO>>("Order is not Exist");

            // Mapping
            var dto = _mapper.Map<List<OrderOnlyDTO>>(result);

            // Return Results
            return ResponseResultWithPagination.Success<List<OrderOnlyDTO>>(dto, paginationResult);
        }

        public async Task<ServiceResponse<OrderDTO>> Get(int OrderId)
        {

            // Quering data
            var result = await _dbContext.Order
                                    .Include(o => o.OrderDetails)
                                    .Include(o => o.CreatedByUser)
                                    .Where(o => o.Id == OrderId)
                                    .FirstOrDefaultAsync();

            // Return error if count is 0
            if (result is null)
                return ResponseResult.Failure<OrderDTO>("Order is not Exist");

            // Mapping
            var dto = _mapper.Map<OrderDTO>(result);


            // Return Results
            return ResponseResult.Success<OrderDTO>(dto);
        }

        public async Task<ServiceResponse<OrderDTO>> Add(OrderAddDTO addOrder)
        {
            // User must be presented to perform this method
            if (String.IsNullOrEmpty(GetUserId()))
                return ResponseResult.Failure<OrderDTO>("User must be presented to perform this method");

            if (addOrder.Discount > 1 || addOrder.Discount < 0)
                return ResponseResult.Failure<OrderDTO>("Discount must be percentage range of 0.00 to 1.00");

            var listOrderDetail = _mapper.Map<List<OrderDetail>>(addOrder.OrderDetails);
            int orderQuantity = 0;
            decimal orderTotal = 0;
            decimal orderNet = 0;

            //For each OrderDetail check if Product is sufficient.

            for (int i = 0; i < listOrderDetail.Count; i++)
            {
                int balance;
                decimal price = 0;

                try
                {
                    (balance, price) = await _stockServices.ProductIsSufficient(listOrderDetail[i].ProductId, new ProductStockAddDTO { Debit = 0, Credit = listOrderDetail[i].Quantity });
                }
                catch (ArgumentNullException anEx)
                {
                    //Return fail if Product is insufficient
                    //EF will not savechange.
                    return ResponseResult.Failure<OrderDTO>(anEx.Message);
                }

                listOrderDetail[i].Price = price;
                listOrderDetail[i].DiscountPrice = price * addOrder.Discount;
                orderQuantity += listOrderDetail[i].Quantity;
                orderTotal += orderQuantity * price;

            }

            orderNet = orderTotal - listOrderDetail.Sum(_ => _.DiscountPrice);

            // Create New Order 
            Order newOrder = _mapper.Map<Order>(addOrder);

            newOrder.OrderDetails = listOrderDetail;
            newOrder.CreatedByUserId = Guid.Parse(GetUserId());
            newOrder.CreatedDate = Now();
            newOrder.Total = orderTotal;
            newOrder.ItemCount = orderQuantity;
            newOrder.Net = orderNet;

            await _dbContext.Order.AddAsync(newOrder);

            // Record a stock
            for (int i = 0; i < listOrderDetail.Count; i++)
            {
                await _stockServices.Set(listOrderDetail[i].ProductId, new ProductStockAddDTO { Debit = 0, Credit = listOrderDetail[i].Quantity, Remark = $"Deduct from Order ({newOrder.Id})" });
            }

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<OrderDTO>(newOrder);

            // Add User Details
            dto.CreatedBy = new UserDto { Id = GetUserId(), Username = GetUsername() };

            return ResponseResult.Success<OrderDTO>(dto);

        }

        public async Task<ServiceResponse<OrderDTO>> Delete(int OrderId)
        {

            // Quering data
            var result = await _dbContext.Order.FindAsync(OrderId);

            // Return error if count is 0
            if (result is null)
                return ResponseResult.Failure<OrderDTO>("Order is not Exist");

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

        public IQueryable<Order> filter(IQueryable<Order> query, OrderFilterDTO filter)
        {

            if (!(filter.StartDate is null) && !(filter.EndDate is null))
                query = query.Where(x => x.CreatedDate >= filter.StartDate && x.CreatedDate <= filter.EndDate);

            return query;
        }


    }
}
