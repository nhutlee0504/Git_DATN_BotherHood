﻿using API.Dto;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUser user;
        public UserController(IUser user) 
        {
            this.user = user;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegisterDto registerDto)//Đăng ký tài khoản người dùng
        {
            try
            {
                var acc = await user.RegisterUser(registerDto);
                return Ok(acc);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)//Đăng nhập người dùng
        {
            try
            {
                var token = await user.LoginUser(loginDto);
                //return Ok(new { Token = token, Message = "Đăng nhập thành công." });
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMyInfo")]
        public async Task<IActionResult> GetAccountInfo()//Lấy thông tim bản thân
        {
            try
            {
                var acc = await user.GetAccountInfo();
                return Ok(acc);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAccountInfoByName")]
        public async Task<IActionResult> GetAccountByName(string username)//Xem thông tin tài khoản người khác
        {
            try
            {
                var acc = await user.GetAccountByUserName(username);
                return Ok(acc);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateInfoAccount")]
        public async Task<IActionResult> UpdateAccount([FromForm] InfoAccountDto infoAccountDto, IFormFile imageFile = null)
        {
            try
            {
                var updatedUser = await user.UpdateAccountInfo(infoAccountDto, imageFile);
                return Ok(updatedUser); // Trả về thông tin người dùng đã cập nhật
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
