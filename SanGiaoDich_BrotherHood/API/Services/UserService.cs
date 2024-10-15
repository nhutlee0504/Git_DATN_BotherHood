﻿using API.Data;
using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserService : IUser
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        public async Task<Account> RegisterUser(RegisterDto registerDto)//Tạo tài khoản ngươi dùng
        {
            // Kiểm tra quy chuẩn mật khẩu
            if (!IsValidPassword(registerDto.Password))
            {
                throw new ArgumentException("Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.");
            }

            // Kiểm tra xem username đã tồn tại hay chưa
            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == registerDto.UserName);
            if (existingUser != null)
            {
                throw new ArgumentException("Tên người dùng đã tồn tại.");
            }
            // Kiểm tra nếu mật khẩu và xác nhận mật khẩu không khớp
            if (registerDto.Password != registerDto.ConformPassword)
            {
                throw new ArgumentException("Mật khẩu và xác nhận mật khẩu không khớp.");
            }
            var newAdmin = new Account
            {
                UserName = registerDto.UserName,
                Password = HashPassword(registerDto.Password),
                IsDelete = false,
                CreatedTime = DateTime.Now,
                Role = "Người dùng"
            };
            await _context.Accounts.AddAsync(newAdmin);
            await _context.SaveChangesAsync();
            return newAdmin;
        }
        public async Task<string> LoginUser(LoginDto loginDto)//Đăng nhập thường
        {
            var userInfo = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (userInfo.Role != "Người dùng")
            {
                throw new UnauthorizedAccessException("Bạn không thể đăng nhập vào hệ thống");
            }
            if (userInfo == null || !VerifyPassword(loginDto.Password, userInfo.Password))
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
            }
            // Kiểm tra nếu tài khoản đã bị xóa
            if (userInfo.IsDelete == true)
            {
                throw new UnauthorizedAccessException("Tài khoản này đã bị khóa vô thời hạn.");
            }

            // Kiểm tra nếu tài khoản bị cấm
            if (userInfo.TimeBanned.HasValue && userInfo.TimeBanned > DateTime.UtcNow)
            {
                var remainingDays = (userInfo.TimeBanned.Value - DateTime.UtcNow).TotalDays;
                throw new UnauthorizedAccessException($"Tài khoản này đã bị khóa. Số ngày còn lại: {Math.Ceiling(remainingDays)}.");
            }
            var token = GenerateJwtToken(userInfo);

            return token;
        }
        public async Task<Account> GetAccountInfo()//Lấy thông tin tài khoản đã đăng nhập vào hệ thống, bản thân
        {
            // Lấy thông tin người dùng từ claims
            var userClaims = GetUserInfoFromClaims();

            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userClaims.UserName);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
            }

            return user; // Trả về thông tin tài khoản
        }
        public async Task<Account> GetAccountByUserName(string userName)//Xem thông tin tài khoản người khác
        {
            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new NotImplementedException("Không tìm thấy người dùng.");
            }

            return user; // Trả về thông tin tài khoản
        }
        public async Task<Account> UpdateAccountInfo(InfoAccountDto infoAccountDto, IFormFile imageFile = null)//Cập nhật tài khoản
        {
            // Lấy thông tin người dùng từ claims
            var userClaims = GetUserInfoFromClaims();
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userClaims.UserName);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy người dùng.");
            }
            // Kiểm tra số điện thoại
            if (!IsValidPhone(infoAccountDto.Phone))
            {
                throw new ArgumentException("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại đúng định dạng.");
            }
            // Cập nhật thông tin tài khoản
            user.FullName = infoAccountDto.FullName;
            user.PhoneNumber = infoAccountDto.Phone;
            user.Gender = infoAccountDto.Gender;
            user.Birthday = infoAccountDto.Birthday;

            // Nếu có file hình ảnh, lưu vào thư mục và cập nhật tên ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhAvatar");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                user.ImageAccount = fileName; // Chỉ lưu tên ảnh
            }

            await _context.SaveChangesAsync();
            return user; // Trả về thông tin đã cập nhật
        }
        public async Task<Account> ChangePassword(string username, string password)//Cập nhật mật khẩu người dùng đăng nhập
        {
            var Pass = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
            Pass.Password = HashPassword(password);
            await _context.SaveChangesAsync();
            return Pass;
        }

        //Phương thức ngoài
        private string GenerateJwtToken(Account user)
        {
            // Kiểm tra người dùng không null
            if (user == null) throw new ArgumentNullException(nameof(user));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
        new Claim("FullName", user.FullName ?? string.Empty),
        new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty),
        new Claim("Gender", user.Gender ?? string.Empty),
        new Claim("Birthday", user.Birthday?.ToString("o") ?? string.Empty),
        new Claim("ImageAccount", user.ImageAccount ?? string.Empty),
        new Claim("IsDelete", user.IsDelete.ToString()),
        new Claim("TimeBanned", user.TimeBanned?.ToString("o") ?? string.Empty)
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string HashPassword(string password) // Băm mật khẩu
        {
            using (var sha256 = SHA256.Create())
            {
                // Băm mật khẩu
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Chỉ lấy 16 byte đầu tiên và chuyển đổi sang định dạng chuỗi hex
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length && i < 16; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString(); // Trả về mật khẩu băm
            }
        }
        private bool VerifyPassword(string password, string hashedPasswordWithSalt) // Kiểm tra mật khẩu khi đăng nhập
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Chỉ lấy 16 byte đầu tiên và chuyển đổi sang định dạng chuỗi hex
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length && i < 16; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                var hashedPassword = sb.ToString();

                // So sánh mật khẩu đã băm với mật khẩu đã lưu trong cơ sở dữ liệu
                return hashedPasswordWithSalt == hashedPassword; // So sánh trực tiếp
            }
        }
        private bool IsValidPassword(string password)//Bắt lỗi quy chuẩn password
        {
            return password.Length >= 6 && // Độ dài tối thiểu
                   password.Any(char.IsUpper) && // Có ít nhất một chữ hoa
                   password.Any(char.IsLower) && // Có ít nhất một chữ thường
                   password.Any(char.IsDigit) && // Có ít nhất một số
                   password.Any(ch => "!@#$%^&*()_-+=<>?/[]{}|~".Contains(ch)); // Có ít nhất một ký tự đặc biệt
        }
        public bool IsValidPhone(string phone)//Bắt lỗi số điện thoại
        {
            string pattern = @"^(?:\+84|0)(?:3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-9]|2[0-9]{1})\d{7}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phone);
        }

        private (string UserName, string Email, string FullName, string PhoneNumber, string Gender, string IDCard, DateTime? Birthday, string ImageAccount, string Role, bool IsDelete, DateTime? TimeBanned) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var userNameClaim = userClaim.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                var fullNameClaim = userClaim.FindFirst("FullName")?.Value ?? string.Empty;
                var phoneNumberClaim = userClaim.FindFirst("PhoneNumber")?.Value ?? string.Empty;
                var genderClaim = userClaim.FindFirst("Gender")?.Value ?? string.Empty;
                var idCardClaim = userClaim.FindFirst("IDCard")?.Value ?? string.Empty;
                var imageAccountClaim = userClaim.FindFirst("ImageAccount")?.Value ?? string.Empty;
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

                DateTime? birthday = null;
                var birthdayClaimValue = userClaim.FindFirst("Birthday")?.Value;
                if (!string.IsNullOrWhiteSpace(birthdayClaimValue))
                {
                    if (DateTime.TryParse(birthdayClaimValue, out DateTime parsedBirthday))
                    {
                        birthday = parsedBirthday;
                    }
                }

                bool isDelete = false;
                var isDeleteClaimValue = userClaim.FindFirst("IsDelete")?.Value;
                if (isDeleteClaimValue != null && bool.TryParse(isDeleteClaimValue, out bool parsedIsDeleted))
                {
                    isDelete = parsedIsDeleted;
                }

                DateTime? timeBanned = null;
                var timeBannedClaimValue = userClaim.FindFirst("TimeBanned")?.Value;
                if (!string.IsNullOrWhiteSpace(timeBannedClaimValue) && DateTime.TryParse(timeBannedClaimValue, out DateTime parsedTimeBanned))
                {
                    timeBanned = parsedTimeBanned;
                }

                return (
                    userNameClaim,
                    emailClaim,
                    fullNameClaim,
                    phoneNumberClaim,
                    genderClaim,
                    idCardClaim,
                    birthday,
                    imageAccountClaim,
                    roleClaim,
                    isDelete,
                    timeBanned
                );
            }

            throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
        }
    }
}
