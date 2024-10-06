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

        public async Task DeleteUserAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _users.DeleteOneAsync(u => u.Id == objectId);
        }

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

        public async Task<UserDTO> RegisterUserAsync(RegisterDTO registerDto)
        {
            var existingUser = await _users.Find(u => u.Email == registerDto.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return null;  // Email already in use
            }

            var hashedPassword = _passwordHasher.HashPassword(registerDto.Password);

            // Validate the role if required (Optional)
            var role = registerDto.Role;
            if (string.IsNullOrEmpty(role) || !IsValidRole(role))
            {
                return null;  // Invalid role
            }

            var newUser = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = registerDto.PhoneNumber,  // Store phone number
                Role = role,  // Set role based on registration
                IsActive = role != "Customer"  // Customers need CSR/Admin approval, others might not
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

        // Optional helper method to check if the role is valid
        private static bool IsValidRole(string role)
        {
            return role == "Customer" || role == "Vendor" || role == "CSR" || role == "Administrator";
        }


        public async Task<UserDTO> UpdateUserAsync(string id, UserDTO userDto)
        {
            var objectId = ObjectId.Parse(id);

            var user = await _users.Find(u => u.Id == objectId).FirstOrDefaultAsync();
            if (user == null) return null;

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;  // Update phone number
            user.Role = userDto.Role;

            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return userDto;
        }
    }
}
