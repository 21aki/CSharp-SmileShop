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

    [Route("api/Products")]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        private IProductGroupServices _ProductGroupService { get; }

        public ProductGroupController(IProductGroupServices productGroupService)
        {
            _ProductGroupService = productGroupService;
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination, [FromQuery] string filter = null, [FromQuery] DataOrderDTO ordering = null)
        {
            try
            {
                var paginationResult = await _ProductGroupService.GetAll(pagination, filter, ordering);
                return Ok(paginationResult);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResultWithPagination.Failure<List<ProductGroupDTO>>(ex.Message));
            }
        }

        [HttpGet("groups/list")]
        public async Task<IActionResult> GetList([FromQuery] string filter)
        {
            try
            {
                var result = await _ProductGroupService.GetList(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<List<ProductGroupDTO>>(ex.Message));
            }
        }

        [HttpGet("groups/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _ProductGroupService.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductGroupDTO>(ex.Message));
            }
        }

        [HttpPost("groups")]
        public async Task<IActionResult> Add(ProductGroupAddDTO addProduct)
        {
            try
            {
                var result = await _ProductGroupService.Add(addProduct);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductGroupDTO>(ex.Message));
            }
        }

        [HttpPut("groups/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ProductGroupAddDTO addProduct)
        {
            try
            {
                var result = await _ProductGroupService.Edit(id, addProduct);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductGroupDTO>(ex.Message));
            }
        }

        [HttpDelete("groups/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _ProductGroupService.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductGroupDTO>(ex.Message));
            }
        }
    }
}
