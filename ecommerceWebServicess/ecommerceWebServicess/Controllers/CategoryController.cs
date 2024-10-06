using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWebServicess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/category
        [HttpGet]
        [Authorize(Roles = "CSR, Administrator, Vendor")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/activeCategory
        [HttpGet("activecategory")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetActiveCategories()
        {
            // Fetch only active categories
            var activeCategories = await _categoryService.GetActiveCategoriesAsync();

            if (activeCategories == null || !activeCategories.Any())
            {
                return NotFound("No active categories found.");
            }

            return Ok(activeCategories);
        }



        // GET: api/category/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "CSR, Administrator, Vendor")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(string id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }


        // POST: api/category
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }


        // PUT: api/category/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);

            if (updatedCategory == null)
            {
                return NotFound();
            }

            return Ok(updatedCategory);
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }



        [HttpPost("Activate/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ActivateCategory(string id)
        {
            var result = await _categoryService.ActivateCategoryAsync(id);
            if (result) return Ok(new { message = "Category activated successfully" });
            return NotFound(new { message = "Category not found" });
        }

        [HttpPost("Deactivate/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeactivateCategory(string id)
        {

            var result = await _categoryService.DeactivateCategoryAsync(id);
            if (result) return Ok(new { message = "Category deactivated successfully" });
            return NotFound(new { message = "Category not found" });
        }






    }
}
