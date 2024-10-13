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

            // Check if user exists, password is valid, and the account is active
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash) || !user.IsActive)
            {
                return null;  // Invalid credentials or account not active
            }

            // Generate a JWT token for the authenticated user
            var token = _jwtHelper.GenerateJwtToken(user);

            // Return the login response with user details and token
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
