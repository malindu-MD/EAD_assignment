using ecommerceWebServicess.DTOs;
using System.Security.Claims;
using ecommerceWebServicess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWebServicess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductService _productService;


        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }



            try
            {
                var product = await _productService.CreateProductAsync(createProductDto, vendorId);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        // PUT: api/Product/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProduct(string id, [FromForm] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vendorId = User.FindFirst("nameid")?.Value;


            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }

          

            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto, vendorId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var vendorId = User.FindFirst("nameid")?.Value;


            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }


            try
            {
                var success = await _productService.DeleteProductAsync(id, vendorId);
                if (success)
                    return NoContent();
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }


        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters parameters)
        {
            var products = await _productService.GetProductsAsync(parameters);
            return Ok(products);
        }



        // PUT: api/Product/Inventory/{id}
        [HttpPut("Inventory/{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateInventory(string id, [FromBody] InventoryUpdateDto inventoryUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vendorId = User.FindFirst("nameid")?.Value;

            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }


            try
            {
                var success = await _productService.UpdateInventoryAsync(id, inventoryUpdateDto, vendorId);
                if (success)
                    return NoContent();
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        // DELETE: api/Product/Stock/{id}
        [HttpDelete("Stock/{id}")]
        [Authorize(Roles = "Vendor,Administrator")]
        public async Task<IActionResult> RemoveProductStock(string id)
        {
            var vendorId = User.FindFirst("nameid")?.Value;
            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _productService.RemoveProductStockAsync(id, vendorId);
                if (success)
                    return NoContent();
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // GET: api/Product/Vendor/{vendorId}
        [HttpGet("Vendor/{vendorId}")]
        public async Task<IActionResult> GetProductsByVendorId(string vendorId)
        {
            var products = await _productService.GetProductsByVendorIdAsync(vendorId);
            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }


        [HttpGet("Search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {   

            if(string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            var products = await _productService.SearchProductsAsync(keyword);
            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }


        [HttpGet("All")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }



        [HttpGet("CheckLowStock")]
        public async Task<IActionResult> CheckAllProductsForLowStock()
        {
            try
            {
                // Call the product service method to check all products for low stock
                await _productService.CheckAllProductsForLowStockAsync();

                return Ok("Low stock check completed and notifications sent if required.");
            }
            catch (Exception ex)
            {
                // Return an error message if something goes wrong
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }









    }
}
