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
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderServices _service;

        public OrdersController(IOrderServices service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination, [FromQuery] OrderFilterDTO filter, [FromQuery] DataOrderDTO order)
        {
            try
            {
                var result = await _service.GetAll(pagination, filter, order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResultWithPagination.Failure<OrderDTO>(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            try
            {
                var result = await _service.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<OrderDTO>(ex.Message));
            }
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {

            try
            {
                var result = await _service.GetOrderDetails(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<List<OrderDetailDTO>>(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderAddDTO addOrder)
        {
            try
            {
                var result = await _service.Add(addOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<OrderDTO>(ex.Message));
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var result = await _service.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<OrderDTO>(ex.Message));
            }
        }
    }
}
