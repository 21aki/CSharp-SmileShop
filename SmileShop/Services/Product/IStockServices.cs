using SmileShop.DTOs;
using SmileShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmileShop.Services
{
    /**
     *  Interface Name : IStockServices 
     *  Created by : AkiAkira
     *  Version : 1.0 - 11 Feb 2021
     *  
     *  <summary>
     *  Services for Stock Perform on
     *  <para>- Stock Management</para>
     *  <para>- Stock Add/Edit</para>
     *  </summary>
     * 
     **/
    public interface IStockServices
    {

        Task<ServiceResponseWithPagination<List<ProductStockDTO>>> GetStockHistory(int productId, PaginationDto pagination);

        Task<ServiceResponse<ProductStockDTO>> Get(int productId);

        Task<ServiceResponse<ProductStockDTO>> Set(int productId, ProductStockAddDTO stockChanges);
        Task<(int balance, decimal price)> ProductIsSufficient(int productId, ProductStockAddDTO stockChanges);
    }
}
