using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        // GET: api/<ProductController>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole.Equals("Vendor")) {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return await _service.GetAll(userId);
            }
            
            return await _service.GetAll();
        }

        // GET api/<ProductController>/:id
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product?>> GetById(String id)
        {
            var response = _service.GetById(id);
            return response is not null? Ok(response) : NotFound();
        }

        // POST api/<ProductController>
        [Authorize(Roles = "Admin, Vendor")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Product request)
        {
            _service.Add(request);            
            return Ok(new { Message = "Item added successfully" });
        }

        // PUT api/<ProductController>/
        [Authorize(Roles = "Admin, Vendor")]
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Product request)
        {
            _service.Update(request);
            return Ok(new { Message = "Item updated successfully" });
        }

        // PUT api/<ProductController>/
        [Authorize(Roles = "Vendor")]
        [HttpPut("Stock")]
        public async Task<ActionResult> UpdateStock([FromBody] Inventory request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _service.UpdateTotalStockAmount(request, userId);
            return Ok(new { Message = "Inventory updated successfully" });
        }

        // DELETE api/<ProductController>/:id
        [Authorize(Roles = "Admin, Vendor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(String id)
        {
            _service.Delete(id);
            return Ok(new { Message = "Item deleted successfully" });
        }
    }
}
