/**************************************************************************
 * File: UserService.cs
 * Description: Service for managing user registration, updates, and status.
 **************************************************************************/

using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Helpers;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly PasswordHasher _passwordHasher;

        public UserService(IMongoClient mongoClient, PasswordHasher passwordHasher)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _users = database.GetCollection<User>("Users");
            _passwordHasher = passwordHasher;
        }

        // Deactivate a user
        public async Task DeactivateUserAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var user = await _users.Find(u => u.Id == objectId).FirstOrDefaultAsync();
            if (user != null)
            {
                user.DeactivatedByUser = true;
                user.IsActive = false;
                await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            }
        }

        // Delete a user
        public async Task DeleteUserAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _users.DeleteOneAsync(u => u.Id == objectId);
        }

        // Get all users
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _users.Find(_ => true).ToListAsync();
            return users.Select(user => new UserDTO
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            });
        }

        // Get user by ID
        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var user = await _users.Find(u => u.Id == objectId).FirstOrDefaultAsync();
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };
        }

        // Reactivate a user
        public async Task ReactivateUserAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var user = await _users.Find(u => u.Id == objectId).FirstOrDefaultAsync();
            if (user != null)
            {
                user.DeactivatedByUser = false;
                user.IsActive = true;  // Only CSR/Admin can reactivate
                await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            }
        }

        // Register a new user
        public async Task<UserDTO> RegisterUserAsync(RegisterDTO registerDto)
        {
            var existingUser = await _users.Find(u => u.Email == registerDto.Email).FirstOrDefaultAsync();
            if (existingUser != null) return null;  // Email already in use

            var hashedPassword = _passwordHasher.HashPassword(registerDto.Password);
            var role = registerDto.Role;
            if (string.IsNullOrEmpty(role) || !IsValidRole(role)) return null;  // Invalid role

            var newUser = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = registerDto.PhoneNumber,
                Role = role,
                IsActive = role != "Customer"  // Customers need CSR/Admin approval
            };

            await _users.InsertOneAsync(newUser);

            return new UserDTO
            {
                Id = newUser.Id.ToString(),
                Username = newUser.Username,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                Role = newUser.Role
            };
        }

        // Helper method to validate roles
        private static bool IsValidRole(string role)
        {
            return role == "Customer" || role == "Vendor" || role == "CSR" || role == "Administrator";
        }

        // Update an existing user
        public async Task<UserDTO> UpdateUserAsync(string id, UserDTO userDto)
        {
            var objectId = ObjectId.Parse(id);
            var user = await _users.Find(u => u.Id == objectId).FirstOrDefaultAsync();
            if (user == null) return null;

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Role = userDto.Role;

            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return userDto;
        }

        // Get all customers
        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customerUsers = await _users.Find(u => u.Role == "Customer").ToListAsync();
            return customerUsers.Select(user => new CustomerDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
