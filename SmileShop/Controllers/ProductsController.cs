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
    public class ProductsController : ControllerBase
    {
        public IProductServices _Service { get; }

        public ProductsController(IProductServices service)
        {
            _Service = service;
        }


        [HttpGet]
        public async Task<IActionResult> getAll([FromQuery] PaginationDto pagination = null, [FromQuery] ProductFilterDTO filter = null, [FromQuery] DataOrderDTO ordering = null)
        {
            try
            {
                var paginationResult = await _Service.GetAll(pagination, filter, ordering);
                return Ok(paginationResult);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResultWithPagination.Failure<List<ProductDTO>>(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _Service.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductDTO>(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductAddDTO addProduct)
        {
            try
            {
                var result = await _Service.Add(addProduct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductDTO>(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, ProductAddDTO editProduct)
        {
            try
            {
                var result = await _Service.Edit(id, editProduct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductDTO>(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _Service.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Failure<ProductDTO>(ex.Message));
            }
        }
    }
}
