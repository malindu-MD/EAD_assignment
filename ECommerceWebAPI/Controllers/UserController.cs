using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
             return await _service.GetAll();
        }

        [Authorize(Roles = "CSR")]
        [HttpGet("GetNewCustomers")]
        public async Task<IEnumerable<User>> GetCustomers()
        {
            return await _service.GetNewCustomers();
        }

        [Authorize]
        [HttpGet("UserDetails")]
        public async Task<ActionResult<User?>> GetById()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = _service.GetById(id);
            return response is not null? Ok(response) : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] User request)
        {
            _service.Add(request);            
            return Ok(new { Message = "User added successfully" });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] User request)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role.Equals("Customer"))
            {
                _service.Add(request);
                return Ok(new { Message = "Registration successfully" });
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] User request)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id.Equals(request._Id))
            {
                _service.Update(request);
                return Ok(new { Message = "User details updated successfully" });
            }
            
            return Unauthorized();
        }

        [Authorize(Roles = "CSR")]
        [HttpPost("{userName}/ActivateAccount")]
        public async Task<ActionResult> ActivateAccount(string userName)
        {
            _service.ActivateAccount(userName);
            return Ok(new { Message = userName + " Account Activated" });
        }

        [Authorize]
        [HttpPost("DeactivateAccount")]
        public async Task<ActionResult> DeactivateAccount()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _service.DeactivateAccount(id);
            return Ok(new { Message = "Account deactivated" });
        }
    }
}
