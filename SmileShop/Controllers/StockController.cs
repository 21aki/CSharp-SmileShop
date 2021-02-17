using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileShop.DTOs;
using SmileShop.Models;
using SmileShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockServices _services;

        public StockController(IStockServices services)
        {
            _services = services;
        }

        [Route("{id}/stock/records")]
        [HttpGet]
        public async Task<IActionResult> GetStockHistory(int id, [FromQuery] PaginationDto pagination)
        {
            string errorMessage;
            try
            {
                var result = await _services.GetStockHistory(id, pagination);
                return Ok(result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return Ok(ResponseResultWithPagination.Failure<List<ProductStockDTO>>(errorMessage));
        }

        [Route("{id}/stock")]
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {

            string errorMessage;
            try
            {
                var result = await _services.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return Ok(ResponseResult.Failure<List<ProductStockDTO>>(errorMessage));
        }

        [Route("{id}/stock")]
        [HttpPost]
        public async Task<IActionResult> Set(int id, [FromBody] ProductStockAddDTO stockChanges)
        {
            string errorMessage;
            try
            {
                var result = await _services.Set(id, stockChanges);
                return Ok(result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return Ok(ResponseResult.Failure<List<ProductStockDTO>>(errorMessage));
        }

        //task<(int debit, int credit)> productbalance(int productid);
    }
}
