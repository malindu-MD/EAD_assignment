using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service;

        public OrderController(OrderService service)
        {
            _service = service;
        }

        // GET: api/<OrderController>
        [Authorize(Roles ="Admin, CSR, Customer")]
        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
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

        // GET api/<OrderController>/:id
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Order?>> GetById(String id)
        {
            var response = _service.GetOrderById(id);
            return response is not null? Ok(response) : NotFound();
        }

        // POST api/<OrderController>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Order request)
        {
            _service.Add(request);            
            return Ok(new { Message = "Order added successfully" });
        }

        // PUT api/<OrderController>/
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> UpdateStatus([FromBody] string orderId, string orderStatus)
        {
            _service.UpdateStatus(orderId, orderStatus);
            return Ok(new { Message = "Order status updated successfully" });
        }

        // DELETE api/<OrderController>/:id
        [Authorize(Roles = "Admin, CSR")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(String id)
        {
            _service.Delete(id);
            return Ok(new { Message = "Order deleted successfully" });
        }
    }
}
