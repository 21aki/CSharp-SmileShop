using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Exceptions;
using SmileShop.Helpers;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Services
{
    public class ProductServicesThrowResult : ServiceBase, IProductServices
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public ProductServicesThrowResult(AppDBContext dbContext, IMapper mapper, IHttpContextAccessor httpContext) : base(dbContext, mapper, httpContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<ServiceResponseWithPagination<List<ProductDTO>>> GetAll(PaginationDto pagination = null, ProductFilterDTO productFilter = null, DataOrderDTO ordering = null)
        {
            // Quering data
            var query = _dbContext.Product.AsQueryable();

            // Filtering data
            query = Filter(query, productFilter);

            // Ordering
            if (!(ordering is null))
            {
                var columns = new List<string> { "Id", "GroupId", "Name", "Price", "CreatedBy", "Status" };

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
            var result = await query.Paginate(pagination).Include(entity => entity.CreatedByUser.Products).Include(entity => entity.Group).ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                return ResponseResultWithPagination.Failure<List<ProductDTO>>("Product is not Exist", ResponseType.NoContent);

            // Mapping
            var dto = _mapper.Map<List<ProductDTO>>(result);

            // Return Results
            return ResponseResultWithPagination.Success<List<ProductDTO>>(dto, paginationResult);
        }

        public async Task<ServiceResponse<ProductDTO>> Get(int productId)
        {
            // Id must be greater than 0
            if (productId <= 0)
                return ResponseResult.Failure<ProductDTO>("Id must be greater than 0", ResponseType.BadRequest);

            // Gettering data
            var data = await _dbContext.Product
                                       .Include(entity => entity.CreatedByUser.Products)
                                       .Include(entity => entity.Group)
                                       .Where(x => x.Id == productId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductDTO>("Product is not Exist", ResponseType.NoContent);

            var dto = _mapper.Map<ProductDTO>(data);

            return ResponseResult.Success<ProductDTO>(dto);
        }
        public async Task<ServiceResponse<ProductDTO>> Add(ProductAddDTO addProduct)
        {
            // User must be presented to perform this method
            if (String.IsNullOrEmpty(GetUserId()))
                return ResponseResult.Failure<ProductDTO>("User must be presented to perform this method", ResponseType.Unauthorized);

            var productGroup = await _dbContext.ProductGroup.FindAsync(addProduct.GroupId);

            if (productGroup is null)
                return ResponseResult.Failure<ProductDTO>("Product Group is not Exist", ResponseType.BadRequest);

            // Add Products
            Product product = _mapper.Map<Product>(addProduct);

            product.Group = productGroup;
            product.CreatedByUserId = Guid.Parse(GetUserId());
            product.CreatedDate = Now();
            product.Status = true;

            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductDTO>(product);

            // Add User Detail
            dto.CreatedByUserID = Guid.Parse(GetUserId());
            dto.CreatedByUserName = GetUsername();

            // Return result
            return ResponseResult.Success<ProductDTO>(dto);
        }

        public async Task<ServiceResponse<ProductDTO>> Edit(int productId, ProductAddDTO editProduct)
        {

            // Id must be greater than 0
            if (productId <= 0)
                return ResponseResult.Failure<ProductDTO>("Id must be greater than 0", ResponseType.BadRequest);

            // Gettering data
            var data = await _dbContext.Product
                                       .Include(entity => entity.CreatedByUser.Products)
                                       .Include(entity => entity.Group)
                                       .Where(x => x.Id == productId)
                                       .FirstOrDefaultAsync();

            var productGroup = await _dbContext.ProductGroup.FindAsync(editProduct.GroupId);

            if (productGroup is null)
                return ResponseResult.Failure<ProductDTO>("Product Group is not Exist", ResponseType.BadRequest);

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductDTO>("Product is not Exist", ResponseType.NoContent);

            // Set data
            _mapper.Map(editProduct, data);
            _dbContext.Product.Update(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductDTO>(data);

            // Return result
            return ResponseResult.Success<ProductDTO>(dto);
        }

        public async Task<ServiceResponse<ProductDTO>> Delete(int productId)
        {
            // Id must be greater than 0
            if (productId <= 0)
                return ResponseResult.Failure<ProductDTO>("Id must be greater than 0", ResponseType.BadRequest);

            // Gettering data
            var data = await _dbContext.Product
                                       .Include(entity => entity.CreatedByUser.Products)
                                       .Include(entity => entity.Group)
                                       .Where(x => x.Id == productId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductDTO>("Product is not Exist", ResponseType.NoContent);

            // Delete data
            _dbContext.Product.Remove(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductDTO>(data);

            // Return result
            return ResponseResult.Success<ProductDTO>(dto);
        }

        public IQueryable<Product> Filter(IQueryable<Product> query, ProductFilterDTO filter)
        {

            if (!(filter.Name is null))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (!(filter.GroupId is null))
                query = query.Where(x => x.GroupId == filter.GroupId);

            return query;
        }
    }
}
