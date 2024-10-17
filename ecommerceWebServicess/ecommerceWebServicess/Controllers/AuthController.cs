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
            try
            {
                // Call authentication service
                var loginResponse = await _authService.AuthenticateAsync(loginDto);

                // If an error message is returned, handle it
                if (!string.IsNullOrEmpty(loginResponse.ErrorMessage))
                {
                    // Handle specific errors based on the error message
                    if (loginResponse.ErrorMessage == "Invalid credentials")
                    {
                        return Unauthorized("Invalid email or password.");
                    }
                    else if (loginResponse.ErrorMessage == "Invalid password")
                    {
                        return Unauthorized("The password you entered is incorrect.");
                    }
                    else if (loginResponse.ErrorMessage == "Account is not active")
                    {
                        return Forbid("Your account is inactive. Please contact support.");
                    }
                }

                // If authentication is successful, return token and user information
                return Ok(new
                {
                    id = loginResponse.Id,
                    name = loginResponse.Username,
                    email = loginResponse.Email,
                    token = loginResponse.Token,
                    role = loginResponse.Role,
                });
            }
            catch (Exception ex)
            {
                // Log the exception (if necessary) and return a generic error message
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
