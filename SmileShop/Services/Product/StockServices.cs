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
    public class StockServices : ServiceBase, IStockServices
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public StockServices(AppDBContext dbContext, IMapper mapper, IHttpContextAccessor httpContext) : base(dbContext, mapper, httpContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<ServiceResponse<ProductStockDTO>> Get(int productId)
        {
            string responseMessage;

            // Check product is exist
            if (productId <= 0)
                throw new ArgumentOutOfRangeException("productId", productId, "Product ID must greater than 0");
            //return ResponseResultWithPagination.Failure<List<ProductStockDTO>>("Id must be greater than 0");


            var product = await _dbContext.Product
                                          .Where(_ => _.Id == productId)
                                          .Include(_ => _.Group)
                                          .FirstOrDefaultAsync();

            if (product is null)
                throw new InvalidOperationException("Product is not Exist");

            var lastStockHistory = await _dbContext.Stock
                                                   .Where(_ => _.ProductId == productId)
                                                   .Include(_ => _.CreatedByUser)
                                                   .OrderByDescending(_ => _.Id)
                                                   .FirstOrDefaultAsync();

            if (lastStockHistory is null)
            {
                responseMessage = $"Product ({product.Name}) have no recent stock records";
            }
            else
            {
                responseMessage = $"Product ({product.Name}) requested record successfully";
                lastStockHistory.Product = product;
            }


            var dto = _mapper.Map<ProductStockDTO>(lastStockHistory);

            return ResponseResult.Success<ProductStockDTO>(dto, responseMessage);

        }

        public async Task<ServiceResponseWithPagination<List<ProductStockDTO>>> GetStockHistory(int productId, PaginationDto pagination = null)
        {
            string responseMessage;

            // Check product is exist
            if (productId <= 0)
                throw new ArgumentOutOfRangeException("productId", productId, "Product ID must greater than 0");


            var product = await _dbContext.Product
                                          .Where(_ => _.Id == productId)
                                          .FirstOrDefaultAsync();

            if (product is null)
                throw new InvalidOperationException("Product is not Exist");

            var stockHistory = _dbContext.Stock
                                         .Where(_ => _.ProductId == productId)
                                         .OrderByDescending(_ => _.Id)
                                         .AsQueryable();

            var paginationResult = await _httpContext.HttpContext
                                                     .InsertPaginationParametersInResponse(
                                                        stockHistory,
                                                        pagination.RecordsPerPage,
                                                        pagination.Page
                                                     );

            var result = await stockHistory.Paginate(pagination)
                                           .Include(_ => _.CreatedByUser)
                                           .Include(_ => _.Product)
                                           .Include(_ => _.Product.Group)
                                           .ToListAsync();


            if (result.Count == 0)
                responseMessage = $"Product ({product.Name}) have no recent stock records";
            else
                responseMessage = $"Product ({product.Name}) requested record successfully";

            var dto = _mapper.Map<List<ProductStockDTO>>(result);

            return ResponseResultWithPagination.Success<List<ProductStockDTO>>(dto, paginationResult, responseMessage);
        }

        public async Task<ServiceResponse<ProductStockDTO>> Set(int productId, ProductStockAddDTO stockChanges)
        {
            // UserID is requrid
            if (String.IsNullOrEmpty(GetUserId()))
                throw new UnauthorizedAccessException("User must be presented to perform this method");

            // Stock value must be equal or greater than 0
            if (stockChanges.Debit == 0 && stockChanges.Credit == 0)
                throw new ArgumentNullException("Debit, Credit", "Stock value is required.");

            if (stockChanges.Debit < 0 || stockChanges.Credit < 0)
                throw new ArgumentOutOfRangeException("Debit, Credit", "Stock value must be equal or greater than 0.");

            if (stockChanges.Debit > 0 && stockChanges.Credit > 0)
                throw new ArgumentOutOfRangeException("Debit, Credit", "Stock value must be one of debit or credit only.");

            // Check product is exist
            if (productId <= 0)
                throw new ArgumentOutOfRangeException("productId", productId, "Product ID must greater than 0");


            var product = await _dbContext.Product
                                          .Where(_ => _.Id == productId)
                                          .Include(_ => _.Group)
                                          .FirstOrDefaultAsync();

            if (product is null)
                throw new InvalidOperationException("Product is not Exist");

            int balance;
            decimal price;
            (balance, price) = await ProductIsSufficient(productId, stockChanges);

            // Add stock record
            Stock productStock = new Stock
            {
                ProductId = productId,
                Debit = stockChanges.Debit,
                Credit = stockChanges.Credit,
                StockBefore = balance,
                Remark = stockChanges.Remark,
                CreatedByUserId = Guid.Parse(GetUserId()),
                CreatedDate = Now()
            };
            await _dbContext.Stock.AddAsync(productStock);

            // Update product's stockbalance
            product.StockCount = productStock.StockAfter;
            _dbContext.Product.Update(product);
            await _dbContext.SaveChangesAsync();

            productStock.Product = product;
            // Mapping data
            var dto = _mapper.Map<ProductStockDTO>(productStock);
            dto.CreatedByUserId = Guid.Parse(GetUserId());
            dto.CreatedByUserName = GetUsername();

            // Return
            return ResponseResult.Success<ProductStockDTO>(dto, $"Product ({product.Name}) stock are update successfully");
        }

        //TaskProductIsSufficient(int productId, int amount)
        public async Task<(int balance, decimal price)> ProductIsSufficient(int productId, ProductStockAddDTO stockChanges)
        {

            // Check product is exist
            if (productId <= 0)
                throw new ArgumentOutOfRangeException("productId", productId, "Product ID must greater than 0");
            //return ResponseResultWithPagination.Failure<List<ProductStockDTO>>("Id must be greater than 0");


            var product = await _dbContext.Product
                                          .Where(_ => _.Id == productId)
                                          .FirstOrDefaultAsync();

            if (product is null)
                throw new InvalidOperationException("Product is not Exist");

            // Get product balance
            var debit = await _dbContext.Stock
                                        .Where(_ => _.ProductId == productId)
                                        .SumAsync(_ => _.Debit);


            var credit = await _dbContext.Stock
                                         .Where(_ => _.ProductId == productId)
                                         .SumAsync(_ => _.Credit);

            var balance = debit - credit;

            // Stock can't be less than 0
            if (stockChanges.Credit > 0 && (balance - stockChanges.Credit) < 0)
                throw new ArithmeticException("Stock can't be less than 0");

            return (balance, product.Price);
        }
    }
}
