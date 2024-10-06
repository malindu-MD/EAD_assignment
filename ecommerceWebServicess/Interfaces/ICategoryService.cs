using ecommerceWebServicess.DTOs;

namespace ecommerceWebServicess.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(string id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto?> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(string id);

        Task<bool> ActivateCategoryAsync(string categoryId);
        Task<bool> DeactivateCategoryAsync(string categoryId);

       Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    }
}
