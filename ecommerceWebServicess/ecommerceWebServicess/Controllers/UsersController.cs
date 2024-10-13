/***************************************************************************
 * File: UsersController.cs
 * Description: Controller responsible for handling user-related operations 
 *              such as registration, user management, activation, deactivation, 
 *              and retrieval of users and customers.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWebServicess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        // Constructor to inject the user service
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var user = await _userService.RegisterUserAsync(registerDto);
            if (user == null)
            {
                return BadRequest("User registration failed. Email already exists.");
            }
            return Ok(user);
        }

        // PUT: Deactivate a user
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            await _userService.DeactivateUserAsync(id);
            return NoContent();
        }

        // PUT: Reactivate a user
        [HttpPut("reactivate/{id}")]
        public async Task<IActionResult> ReactivateUser(string id)
        {
            await _userService.ReactivateUserAsync(id);
            return NoContent();
        }

        // PUT: Update a user's information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDTO userDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, userDto);
            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }
            return Ok(updatedUser);
        }

        // DELETE: Delete a user by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        // GET: Get a user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        // GET: Get all users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: Get all customers
        [HttpGet("customer")]
        //[Authorize(Roles = "Administrator,CSR")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
        {
            var customers = await _userService.GetAllCustomersAsync();
            if (customers == null || !customers.Any())
            {
                return NotFound("No customers found");
            }
            return Ok(customers);
        }
    }
}
