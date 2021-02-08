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
using System.Linq.Expressions;
using System.Reflection;
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
            if (!(productGroupfilter is null))
            {
                query = query.Where(x => x.Name.Contains(productGroupfilter));
            }

            // Ordering
            if (!(ordering is null))
            {
                var columns = new List<string> { "Id", "Name", "CreatedBy", "CreatedDate", "Status" };

                if (columns.Exists(x => x == ordering.OrderBy))
                {
                    if (ordering.OrderBy == "CreatedBy") ordering.OrderBy = "CreatedByUser_.Username";

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
            var result = await query.Paginate(pagination).Include(entity => entity.CreatedUser_).ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                return ResponseResultWithPagination.Failure<List<ProductGroupDTO>>("No Product Group in this query");

            // Mapping
            var dto = _mapper.Map<List<ProductGroupDTO>>(result);

            // Return Results
            return ResponseResultWithPagination.Success<List<ProductGroupDTO>>(dto, paginationResult);
        }

        public async Task<ServiceResponse<List<ProductGroupDTO>>> GetAll(string productGroupfilter)
        {
            // Quering data
            var query = _dbContext.ProductGroup.AsQueryable();


            // Filtering data
            if (!String.IsNullOrEmpty(productGroupfilter))
            {
                query = query.Where(x => x.Name.Contains(productGroupfilter));
            }

            // Generate result
            var result = await query.Include(entity => entity.CreatedUser_).ToListAsync();

            // Return error if count is 0
            if (result.Count == 0)
                return ResponseResult.Failure<List<ProductGroupDTO>>("No Product Group in this query");

            // Mapping
            var dto = _mapper.Map<List<ProductGroupDTO>>(result);

            // Return Results
            return ResponseResult.Success<List<ProductGroupDTO>>(dto);
        }

        public async Task<ServiceResponse<ProductGroupDTO>> Get(int productGroupId)
        {
            // Id must be greater than 0
            if (productGroupId <= 0)
                return ResponseResult.Failure<ProductGroupDTO>("Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.ProductGroup
                                       .Include(entity => entity.CreatedUser_)
                                       .Where(x => x.Id == productGroupId)
                                       .FirstAsync();

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductGroupDTO>("No Product Group in this query");

            var dto = _mapper.Map<ProductGroupDTO>(data);

            return ResponseResult.Success<ProductGroupDTO>(dto);

        }

        public async Task<ServiceResponse<ProductGroupDTO>> Add(ProductGroupAddDTO addProduct)
        {
            // User must be presented to perform this method
            if (String.IsNullOrEmpty(GetUserId()))
                return ResponseResult.Failure<ProductGroupDTO>("User must be presented to perform this method");

            var currentUser = await _dbContext.Users.FindAsync(Guid.Parse(GetUserId()));

            // Create & set data
            ProductGroup productGroup = _mapper.Map<ProductGroup>(addProduct);

            productGroup.CreatedUser_ = currentUser;
            productGroup.CreatedDate = Now();
            productGroup.Status = false;

            // Add data
            await _dbContext.ProductGroup.AddAsync(productGroup);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductGroupDTO>(productGroup);

            // Return result
            return ResponseResult.Success<ProductGroupDTO>(dto);
        }

        public async Task<ServiceResponse<ProductGroupDTO>> Edit(int productGroupId, ProductGroupAddDTO addProduct)
        {

            // Id must be greater than 0
            if (productGroupId <= 0)
                return ResponseResult.Failure<ProductGroupDTO>("Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.ProductGroup
                                       .Include(entity => entity.CreatedUser_)
                                       .Where(x => x.Id == productGroupId)
                                       .FirstAsync();
            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductGroupDTO>("No Product Group in this query");

            // Set data
            _mapper.Map(data, addProduct);
            _dbContext.ProductGroup.Update(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductGroupDTO>(data);

            // Return result
            return ResponseResult.Success<ProductGroupDTO>(dto);

        }

        public async Task<ServiceResponse<ProductGroupDTO>> Delete(int productGroupId)
        {

            // Id must be greater than 0
            if (productGroupId <= 0)
                return ResponseResult.Failure<ProductGroupDTO>("Id must be greater than 0");

            // Gettering data
            var data = await _dbContext.ProductGroup
                                       .Include(entity => entity.CreatedUser_)
                                       .Where(x => x.Id == productGroupId)
                                       .FirstAsync();

            // If no data return error
            if (data is null)
                return ResponseResult.Failure<ProductGroupDTO>("No Product Group in this query");

            // Remove data
            _dbContext.ProductGroup.Remove(data);
            await _dbContext.SaveChangesAsync();

            // Mapping
            var dto = _mapper.Map<ProductGroupDTO>(data);

            // Return result
            return ResponseResult.Success<ProductGroupDTO>(dto);
        }
    }
}