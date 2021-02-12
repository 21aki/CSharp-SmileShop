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
    [Route("api/[controller]")]
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
            var result = await _service.GetAll(pagination, filter, order);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var result = await _service.Get(id);
            return Ok(result);
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
                //Return fail if Product is insufficient
                //EF will not savechange.
                return Ok(ResponseResult.Failure<OrderDTO>(ex.Message));
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
