using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Services
{

    /** 
     *  Service Interface Name : IProductServices
     *  Created by : AkiAkira
     *  Version 1.0 : 5 Feb 2021.
     * <summary>
     * Service for ProductGroup perform on
     * <para> - Product Manage</para>
     * <para> - Product Add Edit</para>
     * Objective:
     * <para> - Able to read Product as lists with Pagination and able to search by ProductGroup & Name</para>
     * <para> - Able to read individual Product  </para>
     * <para> - Able to create individual Product</para>
     * <para> - Able to edit individual Product  </para>
     * <para> - Able to delete individual GroupProduct</para>
     * </summary>
     **/
    public interface IProductServices
    {

        ///To read Product as lists with Pagination and able to search by ProductGroup & Name
        Task<ServiceResponseWithPagination<List<ProductDTO>>> GetAll(PaginationDto pagination = null, ProductFilterDTO productFilter = null, DataOrderDTO ordering = null);

        ///To read individual Product
        Task<ServiceResponse<ProductDTO>> Get(int productId);
        ///To create individual Product
        Task<ServiceResponse<ProductDTO>> Add(ProductAddDTO addProduct);
        ///To edit individual Product
        Task<ServiceResponse<ProductDTO>> Edit(int productId, ProductAddDTO editProduct);
        ///To delete individual Product
        Task<ServiceResponse<ProductDTO>> Delete(int productId);
    }
}
