using DnsClient;
using ECommerceWebAPI.Auth;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;
        public AuthController(AuthService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] Login request)
        {
            string? token = _service.Login(request);
            return token is not null ? Ok(new { Token = token }) : Unauthorized();
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _service.Logout(userId);
            return Ok(new { Message = "Logout Successful"});
        }
    }
}
