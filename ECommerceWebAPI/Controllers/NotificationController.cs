using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _service;

        public NotificationController(NotificationService service)
        {
            _service = service;
        }

        // GET: api/<NotificationController>
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<Notification>> Get()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role.Equals("Admin") || role.Equals("CSR"))
            { 
                return await _service.GetAll();
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return await _service.GetAll(userId);
            }  
        }      

        // DELETE api/<NotificationController>/:id
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(String id)
        {
            _service.Delete(id);
            return Ok(new { Message = "Item deleted successfully" });
        }
    }
}
