﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Helpers;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SmileShop.Services
{
    /**
     * Service Name : ProductGroupServices
     * Created by : AkiAkira
     * Version 1.0 : 5 Feb 2021.
     * 
     * Service for ProductGroup perform on
     * - Product Group Manage
     * - Product Group Add Edit
     */

    public class ProductGroupServices : ServiceBase, IProductGroupServices
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public ProductGroupServices(AppDBContext dbContext, IMapper mapper, IHttpContextAccessor httpContext) : base(dbContext, mapper, httpContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<ServiceResponseWithPagination<List<ProductGroupDTO>>> GetAll(PaginationDto pagination = null, string productGroupfilter = null, DataOrderDTO ordering = null)
        {
            // Quering data
            var query = _dbContext.ProductGroup.AsQueryable();

            // Filtering data
            if (!(String.IsNullOrEmpty(productGroupfilter)))
            {
                query = query.Where(x => x.Name.Contains(productGroupfilter));
            }

            // Ordering
            if (!(ordering is null))
            {
                var columns = new List<string> { "Id", "Name", "CreatedBy", "CreatedDate", "Status" };

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
            var result = await query.Paginate(pagination).Include(_ => _.CreatedByUser).ToListAsync();
            //await query.Paginate(pagination).Include(_=>_.CreatedByUser).DefaultIfEmpty().ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                throw new InvalidOperationException("Product Group is not Exist");

            // Mapping
            var dto = _mapper.Map<List<ProductGroupDTO>>(result);

            // Return Results
            return ResponseResultWithPagination.Success<List<ProductGroupDTO>>(dto, paginationResult);
        }

        public async Task<ServiceResponse<List<ProductGroupDTO>>> GetList(string productGroupfilter)
        {
            // Quering data
            var query = _dbContext.ProductGroup.AsQueryable();


            // Filtering data
            if (String.IsNullOrEmpty(productGroupfilter))
            {
                throw new ArgumentNullException("Product Group Name", "This query must provided filter");
            }

            query = query.Where(x => x.Name.Contains(productGroupfilter));

            // Generate result
            var result = await query.Include(entity => entity.CreatedByUser).ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                throw new InvalidOperationException("Product Group is not Exist");

            // Mapping
            var dto = _mapper.Map<List<ProductGroupDTO>>(result);

            // Return Results
            return ResponseResult.Success<List<ProductGroupDTO>>(dto);
        }

        public async Task<ServiceResponse<ProductGroupDTO>> Get(int productGroupId)
        {
            // Id must be greater than 0
            if (productGroupId <= 0)
                throw new ArgumentOutOfRangeException("Product Group Id", "Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.ProductGroup
                                       .Include(entity => entity.CreatedByUser)
                                       .Where(x => x.Id == productGroupId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                throw new InvalidOperationException("Product Group is not Exist");

            var dto = _mapper.Map<ProductGroupDTO>(data);

            return ResponseResult.Success<ProductGroupDTO>(dto);

        }

        public async Task<ServiceResponse<ProductGroupDTO>> Add(ProductGroupAddDTO addProductGroup)
        {
            // Validation
            Validate(addProductGroup);

            // User must be presented to perform this method
            if (String.IsNullOrEmpty(GetUserId()))
                throw new UnauthorizedAccessException("User must be presented to perform this method");

            // Create & set data
            ProductGroup data = _mapper.Map<ProductGroup>(addProductGroup);

            data.CreatedByUserId = Guid.Parse(GetUserId());
            data.CreatedDate = Now();
            data.Status = true;

            // Add data
            await _dbContext.ProductGroup.AddAsync(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductGroupDTO>(data);

            // Add User Detail
            dto.CreatedByUserName = GetUsername();

            // Return result
            return ResponseResult.Success<ProductGroupDTO>(dto, $"Product Group ({data.Name}) have been added successfully");
        }

        public async Task<ServiceResponse<ProductGroupDTO>> Edit(int productGroupId, ProductGroupAddDTO addProductGroup)
        {
            // Validation
            if (productGroupId <= 0)
                throw new ArgumentOutOfRangeException("Product Group Id", "Id must be greater than 0");

            Validate(addProductGroup);

            // Gettering data
            var data = await _dbContext.ProductGroup
                                       .Include(entity => entity.CreatedByUser)
                                       .Where(x => x.Id == productGroupId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                throw new InvalidOperationException("Product Group is not Exist");

            // Set data
            _mapper.Map(addProductGroup, data);
            _dbContext.ProductGroup.Update(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductGroupDTO>(data);

            // Return result
            return ResponseResult.Success<ProductGroupDTO>(dto, $"Product Group ({data.Name}) have been edited successfully");

        }

        public async Task<ServiceResponse<ProductGroupDTO>> Delete(int productGroupId)
        {

            // Id must be greater than 0
            if (productGroupId <= 0)
                throw new ArgumentOutOfRangeException("Product Group Id", "Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.ProductGroup
                                       .Include(entity => entity.CreatedByUser)
                                       .Where(x => x.Id == productGroupId)
                                       .FirstOrDefaultAsync();

            // If no data return error
            if (data is null)
                throw new InvalidOperationException("Product Group is not Exist");

            // Remove data
            _dbContext.ProductGroup.Remove(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductGroupDTO>(data);

            // Return result
            return ResponseResult.Success<ProductGroupDTO>(dto, $"Product Group ({data.Name}) have been deleted successfully");
        }

        public void Validate(ProductGroupAddDTO productGroup)
        {
            if (String.IsNullOrWhiteSpace(productGroup.Name))
                throw new ArgumentNullException("Name", "Please fill Product Group's Name");
        }
    }
}
