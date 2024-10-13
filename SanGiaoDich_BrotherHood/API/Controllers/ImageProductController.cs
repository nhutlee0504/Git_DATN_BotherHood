﻿using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageProductController : ControllerBase
    {
        private readonly IImageProduct _imageProduct;
        public ImageProductController(IImageProduct imageProduct)
        {
            _imageProduct = imageProduct;
        }

        [HttpPost("{productId}/upload")]
        public async Task<IActionResult> UploadImages(int productId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Không có tệp nào được chọn.");
            }

            try
            {
                var uploadedImages = await _imageProduct.UploadImages(files, productId);
                return Ok(uploadedImages);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        [HttpDelete("{productId}/{imageId}")]
        public async Task<IActionResult> DeleteImage(int productId, int imageId)
        {
            try
            {
                await _imageProduct.DeleteImage(productId, imageId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        [HttpGet("GetImageProduct")]
        public async Task<IActionResult> GetImageProduct(int id)
        {
            try
            {
                return Ok(await _imageProduct.GetImageProducts(id));
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
