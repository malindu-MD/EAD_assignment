using ECommerceWebAPI.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceWebAPI.Utilities.Auth
{
    public class JwtTokenManager
    {
        /*
        private readonly IConfiguration _configuration;

        public JwtTokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string Authenticate(Login user)
        {
            var key = _configuration.GetValue<string>("JwtConfig:Key");
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                 {
                 new Claim(ClaimTypes.NameIdentifier, user),
                 new Claim(ClaimTypes.Role, user.role)
                 }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration.GetValue<string>("JwtConfig:Issuer"),
                Audience = _configuration.GetValue<string>("JwtConfig:Audience"),
                SigningCredentials = new SigningCredentials(
                                      new SymmetricSecurityKey(keyBytes),
                                      SecurityAlgorithms.HmacSha256Signature),

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        */
    }
}
