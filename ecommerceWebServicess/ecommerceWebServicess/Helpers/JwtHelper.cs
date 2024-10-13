/***************************************************************************
 * File: JwtHelpers.cs
 * Description: Jwt implement 
 ***************************************************************************/

using ecommerceWebServicess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ecommerceWebServicess.Helpers
{
    // Helper class for generating JWT tokens based on user data.
    public class JwtHelper
    {
        private readonly string _secret;

        // Constructor to initialize JwtHelper with the secret key for signing the token.
        public JwtHelper(string secret)
        {
            _secret = secret;
        }

        // Generates a JWT token that includes user-specific claims.
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            // Define token claims and signing credentials.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // User's unique identifier.
                    new Claim(ClaimTypes.Email, user.Email),                    // User's email.
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),        // User's phone number.
                    new Claim(ClaimTypes.Role, user.Role)                       // User's role (e.g., Admin, Customer).
                }),
                Expires = DateTime.UtcNow.AddDays(7),  // Token expiration set to 7 days.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  // Token signing using HMAC SHA256.
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);  // Return the serialized JWT token.
        }
    }
}
