using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Helpers;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly JwtHelper _jwtHelper;
        private readonly PasswordHasher _passwordHasher;


        public AuthService(IMongoClient mongoClient, JwtHelper jwtHelper, PasswordHasher passwordHasher)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _users = database.GetCollection<User>("Users");
            _jwtHelper = jwtHelper;
            _passwordHasher = passwordHasher;
        }


        public async Task<LoginResponseDTO> AuthenticateAsync(LoginDTO loginDto)
        {
            var user = await _users.Find(u => u.Email == loginDto.Email).FirstOrDefaultAsync();
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash) || !user.IsActive)
            {
                return null;  // Invalid credentials or account not active
            }

            var token = _jwtHelper.GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                Token = token,
                Role = user.Role,
            };


        }
    }
}
