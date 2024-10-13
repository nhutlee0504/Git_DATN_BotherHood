﻿
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class ImageProductResponse : IImageProduct
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        private readonly string _imagePath;

        public ImageProductResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhSanPham");
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task DeleteImage(int idProd, int idImage)
        {
            var user = GetUserInfoFromClaims();

            // Tìm sản phẩm tương ứng với idProd
            var product = await _context.Products.FindAsync(idProd);

            if (product == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại.");
            }

            // Kiểm tra xem người dùng có phải là người tạo sản phẩm không
            if (product.UserName != user.UserName)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa ảnh này.");
            }

            // Tìm ảnh theo idImage
            var imageProduct = await _context.ImageProducts.FindAsync(idImage);

            if (imageProduct != null)
            {
                _context.ImageProducts.Remove(imageProduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Ảnh không tồn tại.");
            }
        }


        public async Task<IEnumerable<ImageProduct>> GetImageProducts(int id)
        {
            var Image = await _context.ImageProducts.Where(ip => ip.IDProduct == id).ToListAsync();
            if (Image == null)
            {
                throw new NotImplementedException("Sản phẩm không có ảnh");
            }
            return Image;
        }

        public async Task<IEnumerable<ImageProduct>> UploadImages(List<IFormFile> files, int productId)
        {
            var user = GetUserInfoFromClaims();

            // Lấy sản phẩm tương ứng với productId
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại.");
            }

            // Kiểm tra xem người dùng có phải là người tạo sản phẩm không
            if (product.UserName != user.UserName)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thêm ảnh cho sản phẩm này.");
            }

            var imageProducts = new List<ImageProduct>();

            if (files.Count > 3)
                throw new System.Exception("Bạn chỉ được chọn tối đa 3 ảnh.");

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_imagePath, fileName);

                // Lưu tệp vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Tạo đối tượng ImageProduct với ID sản phẩm
                var imageProduct = new ImageProduct
                {
                    Image = fileName,
                    IDProduct = productId // Gán ID sản phẩm
                };

                // Thêm hình ảnh vào cơ sở dữ liệu
                imageProducts.Add(await AddImage(imageProduct));
            }

            return imageProducts;
        }


        public async Task<ImageProduct> AddImage(ImageProduct imageProduct)
        {
            _context.ImageProducts.Add(imageProduct);
            await _context.SaveChangesAsync();
            return imageProduct;
        }

        //Phương thức ngoài
        private (string UserName, string Email, string FullName, string PhoneNumber, string Gender, string IDCard, DateTime? Birthday, string ImageAccount, string Role, bool IsDelete, DateTime? TimeBanned) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var userNameClaim = userClaim.FindFirst(ClaimTypes.Name);
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
                var fullNameClaim = userClaim.FindFirst("FullName");
                var phoneNumberClaim = userClaim.FindFirst("PhoneNumber");
                var genderClaim = userClaim.FindFirst("Gender");
                var idCardClaim = userClaim.FindFirst("IDCard");
                var birthdayClaim = userClaim.FindFirst("Birthday");
                var imageAccountClaim = userClaim.FindFirst("ImageAccount");
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role);
                var isDeleteClaim = userClaim.FindFirst("IsDelete");
                var timeBannedClaim = userClaim.FindFirst("TimeBanned");

                DateTime? birthday = null;
                if (!string.IsNullOrWhiteSpace(birthdayClaim?.Value))
                {
                    if (DateTime.TryParse(birthdayClaim.Value, out DateTime parsedBirthday))
                    {
                        birthday = parsedBirthday;
                    }
                    else
                    {
                        // Log or handle the invalid date format here if needed
                    }
                }

                return (
                    userNameClaim?.Value,
                    emailClaim?.Value,
                    fullNameClaim?.Value,
                    phoneNumberClaim?.Value,
                    genderClaim?.Value,
                    idCardClaim?.Value,
                    birthday,
                    imageAccountClaim?.Value,
                    roleClaim?.Value,
                    isDeleteClaim != null && bool.TryParse(isDeleteClaim.Value, out bool isDeleted) && isDeleted,
                    timeBannedClaim != null ? DateTime.TryParse(timeBannedClaim.Value, out DateTime parsedTimeBanned) ? parsedTimeBanned : (DateTime?)null : (DateTime?)null
                );
            }
            throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
        }

    }
}
