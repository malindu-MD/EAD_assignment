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


        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Call the AuthenticateAsync method asynchronously
            var loginResponse = await _authService.AuthenticateAsync(loginDto);

            if (loginResponse == null)
            {
                // If authentication fails, return Unauthorized
                return Unauthorized("Invalid credentials.");
            }

            // If authentication succeeds, return the token in the response
            return Ok(new
            {
                id = loginResponse.Id,
                name = loginResponse.Username,
                email = loginResponse.Email,
                token = loginResponse.Token,
                role=loginResponse.Role,
            });
        }

    }
}
