using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Utilities.Database;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceWebAPI.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> collection;

        public AuthService(MongoDBService mongoDBService, IConfiguration configuration)
        {
            _configuration = configuration;            
            collection = mongoDBService.Database?.GetCollection<User>("User");
        }

        private string GenerateToken(string id, string role)
        {
            var key = _configuration.GetValue<string>("JwtConfig:Key");
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                 {
                 new Claim(ClaimTypes.NameIdentifier, id),
                 new Claim(ClaimTypes.Role, role)
                 }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration.GetValue<string>("JwtConfig:Issuer"),
                Audience = _configuration.GetValue<string>("JwtConfig:Audience"),
                SigningCredentials = new SigningCredentials(
                                      new SymmetricSecurityKey(keyBytes),
                                      SecurityAlgorithms.HmacSha256Signature),

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string? Login(Login login)
        {
            string? token = null;

            var filter = Builders<User>.Filter.And(                
                Builders<User>.Filter.Eq(x => x.userName, login.userName),
                Builders<User>.Filter.Eq(x => x.credential, login.credential)       
            );
            var user = collection.Find(filter).FirstOrDefault();            

            if (user.isActive && !user.isLoggedIn)
            {
                var update = Builders<User>.Update.Set(x => x.isLoggedIn, true);
                collection.UpdateOneAsync(filter, update);
                token = GenerateToken(user._Id, user.userType);                
            }

            return token;
        }

        public bool Logout(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x._Id, id);
            var update = Builders<User>.Update.Set(x => x.isLoggedIn, false);
            collection.UpdateOneAsync(filter, update);
            return true;
        }
    }
}
