/***************************************************************************
 * File: ProductController.cs
 * Description: Controller responsible for handling product-related operations 
 *              such as creating, updating, deleting, and managing products, 
 *              as well as inventory and stock-related actions for vendors.
 ***************************************************************************/

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

        // Constructor to inject the product service
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // POST: Create a new product
        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
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

        // PUT: Update an existing product
        [HttpPut("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProduct(string id, UpdateProductDto updateProductDto)
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
                var product = await _productService.UpdateProductAsync(id, updateProductDto, vendorId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: Delete a product
        [HttpDelete("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }

            try
            {
                var success = await _productService.DeleteProductAsync(id, vendorId);
                if (success) return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: Get product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // GET: Get all products with query parameters
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters parameters)
        {
            var products = await _productService.GetProductsAsync(parameters);
            return Ok(products);
        }

        // PUT: Update product inventory
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
                if (success) return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: Remove product stock
        [HttpDelete("Stock/{id}")]
        [Authorize(Roles = "Vendor")]
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
                if (success) return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: Get products by vendor ID
        [HttpGet("Vendor")]
        public async Task<IActionResult> GetProductsByVendorId()
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }

            var products = await _productService.GetProductsByVendorIdAsync(vendorId);
            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }

        // GET: Search products by keyword
        [HttpGet("Search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
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

        // GET: Get all products
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

        // GET: Check all products for low stock
        [HttpGet("CheckLowStock")]
        public async Task<IActionResult> CheckAllProductsForLowStock()
        {
            try
            {
                await _productService.CheckAllProductsForLowStockAsync();
                return Ok("Low stock check completed and notifications sent if required.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PATCH: Activate a product
        [HttpPatch("{productId}/activate")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> ActivateProduct(string productId)
        {
            var result = await _productService.ActivateProductAsync(productId);
            if (result)
            {
                return Ok(new { success = true, message = "Product activated successfully." });
            }
            return NotFound(new { success = false, message = "Product not found." });
        }

        // PATCH: Deactivate a product
        [HttpPatch("{productId}/deactivate")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeactivateProduct(string productId)
        {
            var result = await _productService.DeactivateProductAsync(productId);
            if (result)
            {
                return Ok(new { success = true, message = "Product deactivated successfully." });
            }
            return NotFound(new { success = false, message = "Product not found." });
        }
    }
}
