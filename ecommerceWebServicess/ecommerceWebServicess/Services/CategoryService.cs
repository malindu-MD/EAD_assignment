/**************************************************************************
 * File: CategoryService.cs
 * Description: Service for managing categories, including creating,
 *              updating, deleting, and activating/deactivating categories.
 **************************************************************************/

using AutoMapper;
using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;

        // Constructor to initialize MongoDB collections and AutoMapper
        public CategoryService(IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _categoryCollection = database.GetCollection<Category>("Categories");
            _productCollection = database.GetCollection<Product>("Products");
            _mapper = mapper;
        }

        // Create a new category
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            category.DateCreated = DateTime.UtcNow;
            category.DateModified = DateTime.UtcNow;

            await _categoryCollection.InsertOneAsync(category);
            return _mapper.Map<CategoryDto>(category);
        }

        // Update an existing category by ID
        public async Task<CategoryDto?> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null) return null;

            category.Name = updateCategoryDto.Name;
            category.IsActive = updateCategoryDto.IsActive;
            category.DateModified = DateTime.UtcNow;

            await _categoryCollection.ReplaceOneAsync(c => c.Id == id, category);
            return _mapper.Map<CategoryDto?>(category);
        }

        // Delete a category if no products are linked to it
        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var productsInCategory = await _productCollection.CountDocumentsAsync(p => p.CategoryId == id);

            if (productsInCategory > 0)
            {
                throw new Exception("Cannot delete category because products are associated with it.");
            }

            var deleteResult = await _categoryCollection.DeleteOneAsync(c => c.Id == id);
            return deleteResult.DeletedCount > 0;
        }

        // Get all categories
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryCollection.Find(c => true).ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        // Get a category by ID
        public async Task<CategoryDto?> GetCategoryByIdAsync(string id)
        {
            var category = await _categoryCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
            return category == null ? null : _mapper.Map<CategoryDto?>(category);
        }

        // Activate a category by ID
        public async Task<bool> ActivateCategoryAsync(string categoryId)
        {
            var update = Builders<Category>.Update
                .Set(c => c.IsActive, true)
                .Set(c => c.DateModified, DateTime.UtcNow);

            var result = await _categoryCollection.UpdateOneAsync(
                c => c.Id == categoryId, update);

            return result.ModifiedCount > 0;
        }

        // Deactivate a category by ID
        public async Task<bool> DeactivateCategoryAsync(string categoryId)
        {
            var update = Builders<Category>.Update
                .Set(c => c.IsActive, false)
                .Set(c => c.DateModified, DateTime.UtcNow);

            var result = await _categoryCollection.UpdateOneAsync(
                c => c.Id == categoryId, update);

            return result.ModifiedCount > 0;
        }

        // Get all active categories
        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            var activeCategories = await _categoryCollection
                .Find(c => c.IsActive)  // Only active categories
                .ToListAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(activeCategories);
        }
    }
}
