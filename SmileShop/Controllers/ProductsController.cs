using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileShop.DTOs;
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
        public async Task<IActionResult> getAll([FromQuery] int? Page = null, [FromQuery] PaginationDto pagination = null, [FromQuery] ProductFilterDTO filter = null, [FromQuery] DataOrderDTO ordering = null)
        {
            if (!(Page is null))
            {
                var paginationResult = await _Service.GetAll(pagination, filter, ordering);
                return Ok(paginationResult);
            }

            var result = await _Service.GetAll(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.Get(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductAddDTO addProduct)
        {
            var result = await _Service.Add(addProduct);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, ProductAddDTO editProduct)
        {
            var result = await _Service.Edit(id, editProduct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.Delete(id);
            return Ok(result);
        }
    }
}
