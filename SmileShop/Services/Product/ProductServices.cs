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
    public class ProductServices : ServiceBase, IProductServices
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public ProductServices(AppDBContext dbContext, IMapper mapper, IHttpContextAccessor httpContext) : base(dbContext, mapper, httpContext)
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
            query = filter(query, productFilter);

            // Ordering
            if (!(ordering is null))
            {
                var columns = new List<string> { "Id","GroupId","Name","Price","CreatedBy","Status" };

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
            var result = await query.Paginate(pagination).Include(entity => entity.CreatedByUser).Include(entity => entity.Group_).ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                return ResponseResultWithPagination.Failure<List<ProductDTO>>("No Product in this query");

            // Mapping
            var dto = _mapper.Map<List<ProductDTO>>(result);

            // Return Results
            return ResponseResultWithPagination.Success<List<ProductDTO>>(dto, paginationResult);
        }

        public async Task<ServiceResponse<List<ProductDTO>>> GetAll(ProductFilterDTO productFilter = null)
        {
            // Quering data
            var query = _dbContext.Product.AsQueryable();


            // Filtering data
            query = filter(query, productFilter);

            // Generate result
            var result = await query.Include(entity => entity.CreatedByUser).Include(entity => entity.Group_).ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                return ResponseResult.Failure<List<ProductDTO>>("No Product in this query");

            // Mapping
            var dto = _mapper.Map<List<ProductDTO>>(result);

            // Return Results
            return ResponseResult.Success<List<ProductDTO>>(dto);
        }

        public async Task<ServiceResponse<ProductDTO>> Get(int productId)
        {
            // Id must be greater than 0
            if (productId <= 0)
                return ResponseResult.Failure<ProductDTO>("Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.Product
                                       .Include(entity => entity.CreatedByUser)
                                       .Include(entity => entity.Group_)
                                       .Where(x => x.Id == productId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductDTO>("No Product Group in this query");

            var dto = _mapper.Map<ProductDTO>(data);

            return ResponseResult.Success<ProductDTO>(dto);
        }
        public async Task<ServiceResponse<ProductDTO>> Add(ProductAddDTO addProduct)
        {
            // User must be presented to perform this method
            if (String.IsNullOrEmpty(GetUserId()))
                return ResponseResult.Failure<ProductDTO>("User must be presented to perform this method");

            var productGroup = await _dbContext.ProductGroup.FindAsync(addProduct.GroupId);

            if(productGroup is null)
                return ResponseResult.Failure<ProductDTO>("Invalid Product Group");

            // Create & set data
            Product product = _mapper.Map<Product>(addProduct);

            product.Group_ = productGroup;
            product.CreatedByUserId = Guid.Parse(GetUserId());
            product.CreatedDate = Now();
            product.Status = false;

            // Add data
            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductDTO>(product);

            // Add User Detail
            dto.CreatedBy = new UserDto { Id = GetUserId(), Username = GetUsername() };

            // Return result
            return ResponseResult.Success<ProductDTO>(dto);
        }

        public async Task<ServiceResponse<ProductDTO>> Edit(int productId, ProductAddDTO editProduct)
        {

            // Id must be greater than 0
            if (productId <= 0)
                return ResponseResult.Failure<ProductDTO>("Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.Product
                                       .Include(entity => entity.CreatedByUser)
                                       .Include(entity => entity.Group_)
                                       .Where(x => x.Id == productId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductDTO>("No Product in this query");

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
                return ResponseResult.Failure<ProductDTO>("Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.Product
                                       .Include(entity => entity.CreatedByUser)
                                       .Include(entity => entity.Group_)
                                       .Where(x => x.Id == productId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductDTO>("No Product in this query");

            // Delete data
            _dbContext.Product.Remove(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductDTO>(data);

            // Return result
            return ResponseResult.Success<ProductDTO>(dto);
        }

        public IQueryable<Product> filter(IQueryable<Product> query, ProductFilterDTO filter)
        {

            if (!(filter.Name is null))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (!(filter.GroupId is null))
                query = query.Where(x => x.GroupId == filter.GroupId);

            return query;
        }
    }
}
