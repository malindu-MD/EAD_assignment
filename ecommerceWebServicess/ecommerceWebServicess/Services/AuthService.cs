/***************************************************************************
 * File: AuthService.cs
 * Description: Service for handling user authentication, token generation,
 *              and password verification in the e-commerce system.
 ***************************************************************************/

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

        // Constructor to inject dependencies
        public AuthService(IMongoClient mongoClient, JwtHelper jwtHelper, PasswordHasher passwordHasher)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _users = database.GetCollection<User>("Users");
            _jwtHelper = jwtHelper;
            _passwordHasher = passwordHasher;
        }

        // Authenticate the user and return a login response with a JWT
        public async Task<LoginResponseDTO> AuthenticateAsync(LoginDTO loginDto)
        {
            var user = await _users.Find(u => u.Email == loginDto.Email).FirstOrDefaultAsync();

            if (user == null)
            {
                return new LoginResponseDTO { ErrorMessage = "Invalid credentials" };
            }

            if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return new LoginResponseDTO { ErrorMessage = "Invalid password" };
            }

            if (!user.IsActive)
            {
                return new LoginResponseDTO { ErrorMessage = "Account is not active" };
            }

            var token = _jwtHelper.GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                Token = token,
                Role = user.Role,
                ErrorMessage = null  // No errors if successful
            };
        }

    }
}
