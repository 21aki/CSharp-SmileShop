using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Services
{
    public interface IOrderServices
    {

        /** 
         *  Service Interface Name : IOrderServices
         *  Created by : AkiAkira
         *  Version 1.0 : 6 Feb 2021.
         * <summary>
         * Service for Order & Order Details perform on
         * <para> - Orders</para>
         * <para> - New Order</para>
         * <para> - Checkout</para>
         * Objective:
         * <para> - Able to read Order as lists with Pagination and able to search by Date</para>
         * <para> - Able to read individual Order with OrderDetails</para>
         * <para> - Able to create individual Order with OrderDetails</para>
         * <para> - Able to delete individual Order with OrderDetails</para>
         * </summary>
         **/


        /// To read Order as lists with Pagination and able to search by Date
        public Task<ServiceResponseWithPagination<List<OrderOnlyDTO>>> GetAll(PaginationDto pagination = null, OrderFilterDTO OrderFilter = null, DataOrderDTO ordering = null);

        /// To read individual Order with OrderDetails
        public Task<ServiceResponse<OrderDTO>> Get(int OrderId);

        /// To create individual Order with OrderDetails
        public Task<ServiceResponse<OrderDTO>> Add(OrderAddDTO addOrder);

        /// Check the selected product is sufficient to check out
        public Task<(bool, decimal)> ProductIsSufficient(int productId, int amount);

        /// To delete individual Order with OrderDetails
        public Task<ServiceResponse<OrderDTO>> Delete(int OrderId);
    }
}
