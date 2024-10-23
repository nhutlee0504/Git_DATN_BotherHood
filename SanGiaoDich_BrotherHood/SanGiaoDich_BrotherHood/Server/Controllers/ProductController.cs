using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _productService;

        // Inject service thông qua constructor
        public ProductController(IProduct productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet("{id}")]
        public async Task<Product> GetProductById(int id)
        {
            return await _productService.GetProductById(id);
        }

        [HttpGet("name/{name}")]
        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            return await _productService.GetProductByName(name);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
        {
            try
            {
                var product = await _productService.AddProduct(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.IDProduct }, product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductById(int id, [FromForm] ProductDto productDto)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductById(id, productDto);
                return Ok(updatedProduct);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    }
}
