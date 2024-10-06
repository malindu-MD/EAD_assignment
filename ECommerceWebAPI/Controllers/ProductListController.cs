using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductListController : ControllerBase
    {
        private readonly ProductListService _service;

        public ProductListController(ProductListService service)
        {
            _service = service;
        }

        // GET: api/<ProductListController>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<ProductList>> Get()
        {
            return await _service.GetAll();
        }

        // GET api/<ProductListController>/:id
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductList?>> GetById(String id)
        {
            var response = _service.GetById(id);
            return response is not null? Ok(response) : NotFound();
        }

        // POST api/<ProductListController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ProductList request)
        {
            _service.Add(request);            
            return Ok(new { Message = "Item added successfully" });
        }

        // PUT api/<ProductListController>/
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] ProductList request)
        {
            _service.Update(request);
            return Ok(new { Message = "Item updated successfully" });
        }

        // DELETE api/<ProductListController>/:id
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(String id)
        {
            _service.Delete(id);
            return Ok(new { Message = "Item deleted successfully" });
        }
    }
}
