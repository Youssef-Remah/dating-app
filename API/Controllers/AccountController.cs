using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(AppDbContext dbContext, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await EmailExists(registerDto.Email)) return BadRequest("Email Taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
            };

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();

            return new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Id = user.Id,
                Token = tokenService.CreateToken(user)
            };
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            AppUser? user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (user == null) return Unauthorized("Invalid Email Address.");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < user.PasswordHash.Length; i++)
            {
                if (user.PasswordHash[i] != computedPassword[i]) return Unauthorized("Invalid Password.");
            }

            return new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Id = user.Id,
                Token = tokenService.CreateToken(user)
            };
        }

        private async Task<bool> EmailExists(string email)
        {
            return await dbContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}