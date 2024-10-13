/***************************************************************************
 * File: AuthController.cs
 * Description: Handles user authentication, including login and JWT token 
 *              generation.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWebServicess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Constructor to inject the AuthService dependency
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/login
        // Authenticates the user based on login credentials
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Call authentication service
            var loginResponse = await _authService.AuthenticateAsync(loginDto);

            // If credentials are invalid, return unauthorized response
            if (loginResponse == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // Return token and user information on successful login
            return Ok(new
            {
                id = loginResponse.Id,
                name = loginResponse.Username,
                email = loginResponse.Email,
                token = loginResponse.Token,
                role = loginResponse.Role,
            });
        }
    }
}
