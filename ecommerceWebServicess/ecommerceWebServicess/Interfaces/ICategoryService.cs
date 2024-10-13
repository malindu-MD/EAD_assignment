/***************************************************************************
 * File: ICategoryService.cs
 * Description: Interface for category services. Defines methods for 
 *              managing categories such as creating, updating, and deleting.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerceWebServicess.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A collection of CategoryDto objects.</returns>
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>A CategoryDto object or null if not found.</returns>
        Task<CategoryDto?> GetCategoryByIdAsync(string id);

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="createCategoryDto">The category data to create.</param>
        /// <returns>The created CategoryDto.</returns>
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="updateCategoryDto">The updated category data.</param>
        /// <returns>The updated CategoryDto or null if not found.</returns>
        Task<CategoryDto?> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto);

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        Task<bool> DeleteCategoryAsync(string id);

        /// <summary>
        /// Activates a category by its ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to activate.</param>
        /// <returns>True if activation was successful, otherwise false.</returns>
        Task<bool> ActivateCategoryAsync(string categoryId);

        /// <summary>
        /// Deactivates a category by its ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to deactivate.</param>
        /// <returns>True if deactivation was successful, otherwise false.</returns>
        Task<bool> DeactivateCategoryAsync(string categoryId);

        /// <summary>
        /// Retrieves only active categories.
        /// </summary>
        /// <returns>A collection of active CategoryDto objects.</returns>
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    }
}
