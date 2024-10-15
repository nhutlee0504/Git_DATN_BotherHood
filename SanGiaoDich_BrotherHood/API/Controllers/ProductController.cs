using API.Dto;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProduct prod;
        public ProductController(IProduct prods)
        {
            prod = prods;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                var products = await prod.GetAllProductsAsync();
                return Ok(products);
            }
            catch (NotImplementedException ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetProductId")]
        public async Task<Product> GetProductById(int id)
        {
            return await prod.GetProductById(id);
        }

        [HttpGet("name")]
        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            return await prod.GetProductByName(name);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
        {
            try
            {
                var product = await prod.AddProduct(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.IDProduct }, product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log the exception as needed
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")] // Specify that {id} is a route parameter for the update method
        public async Task<IActionResult> UpdateProductById(int id, [FromForm] ProductDto productDto)//Cập nhật product
        {

            try
            {
                var updatedProduct = await prod.UpdateProductById(id, productDto);
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
                // Log the exception as needed
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }



    }
}
