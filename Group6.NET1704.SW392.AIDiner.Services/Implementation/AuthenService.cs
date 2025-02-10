﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Group6.NET1704.SW392.AIDiner.Common.DTO;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Implementation;
using Group6.NET1704.SW392.AIDiner.DAL.Models;
using Group6.NET1704.SW392.AIDiner.DAL.ViewModel;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Group6.NET1704.SW392.AIDiner.DAL.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private static readonly HashSet<string> RevokedTokens = new HashSet<string>();

        public AuthenService(IGenericRepository<User> userRepository, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Login(LoginDTO model)
        {


            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                return null;
            }

            var user = await _userRepository.GetByExpression(
                u => u.Username == model.UserName && u.Status == true,
                u => u.Role);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return null;
            }

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Username.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("RoleId", user.RoleId.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["JwtConfig:Issuer"],
            audience: _configuration["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<string> Register(RegisterLoginModel model)
        {
            try
            {
                if (await _userRepository.ExistsAsync(u => u.Username == model.Username))
                    return "User already exists";

                if (await _userRepository.ExistsAsync(u => u.Email == model.Email))
                    return "Email already exists";

                if (await _userRepository.ExistsAsync(u => u.PhoneNumber == model.PhoneNumber))
                    return "c";

                var newUser = new User
                {
                    Username = model.Username,
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Dob = model.Dob,
                    PhoneNumber = model.PhoneNumber,
                    RoleId = 1,
                    CreateAt = DateTime.UtcNow,
                    Address = model.Address,
                    Status = true,
                    Avatar = model.Avatar
                };


                await _userRepository.Insert(newUser);
                await _unitOfWork.SaveChangeAsync();
                return "User registered successfully.";
            }
            catch (Exception ex)
            {
                return $"Internal server error: {ex.Message}";
            }
        }

        public string GenerateJwtToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "User" : user.RoleId == 2 ? "Manager" : "Admin"),
                new Claim("RoleId", user.RoleId.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["JwtConfig:Issuer"],
            audience: _configuration["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            RevokedTokens.Add(token);
            return true;
        }
        public bool IsTokenRevoked(string token)
        {
            return RevokedTokens.Contains(token);
        }
    }
}