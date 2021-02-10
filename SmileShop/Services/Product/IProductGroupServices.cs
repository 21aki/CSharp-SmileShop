using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Services
{
    /** 
     *  Service Interface Name : IProductGroupServices
     *  Created by : AkiAkira
     *  Version 1.0 : 4 Feb 2021.
     * <summary>
     * Service for ProductGroup perform on
     * <para> - Product Group Manage</para>
     * <para> - Product Group Add Edit</para>
     * Objective:
     * <para> - Able to read GroupProduct as lists with Pagination and able to search by Name</para>
     * <para> - Able to read individual GroupProduct  </para>
     * <para> - Able to create individual GroupProduct</para>
     * <para> - Able to edit individual GroupProduct  </para>
     * <para> - Able to delete individual GroupProduct</para>
     * </summary>
     **/
    public interface IProductGroupServices
    {

        /// <summary>
        /// To read GroupProduct as lists with Pagination and able to search by Name
        /// </summary>
        Task<ServiceResponseWithPagination<List<ProductGroupDTO>>> GetAll(PaginationDto pagination = null, string productGroupfilter = null, DataOrderDTO ordering = null);

        /// <summary>
        /// To read GroupProduct as lists without Pagination and able to search by Name
        /// </summary>
        Task<ServiceResponse<List<ProductGroupDTO>>> GetList(string productGroupfilter);
        /// <summary>
        /// To read individual GroupProduct
        /// </summary>
        Task<ServiceResponse<ProductGroupDTO>> Get(int productGroupId);

        /// <summary>
        /// To create individual GroupProduct
        /// </summary>
        Task<ServiceResponse<ProductGroupDTO>> Add(ProductGroupAddDTO addProductGroup);

        /// <summary>
        /// To edit individual GroupProduct
        /// </summary>
        Task<ServiceResponse<ProductGroupDTO>> Edit(int productGroupId, ProductGroupAddDTO addProductGroup);

        /// <summary>
        /// To delete individual GroupProduct
        /// </summary>
        Task<ServiceResponse<ProductGroupDTO>> Delete(int productGroupId);
}
}
