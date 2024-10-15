using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUser
    {
        public Task<Account> RegisterUser(RegisterDto registerDto);
        public Task<string> LoginUser(LoginDto loginDto);
        public Task<Account> GetAccountInfo();
        public Task<Account> GetAccountByUserName(string userName);
        public Task<Account> UpdateAccountInfo(InfoAccountDto infoAccountDto, IFormFile imageFile = null);
        public Task<Account> ChangePassword(string username, InfoAccountDto info);
    }
}
