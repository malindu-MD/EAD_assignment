/**************************************************************************
 * File: ProductService.cs
 * Description: Service to manage product creation, updates, and stock.
 **************************************************************************/

using AutoMapper;
using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public ProductService(IMongoClient mongoClient, INotificationService notificationService, IMapper mapper)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _products = database.GetCollection<Product>("Products");
            _categoryCollection = database.GetCollection<Category>("Categories");
            _notificationService = notificationService;
            _mapper = mapper;
        }

        // Create a new product
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, string vendorId)
        {
            var product = new Product
            {
                ProductId = string.Concat("P", Guid.NewGuid().ToString("N").AsSpan(0, 7)),
                VendorId = vendorId,
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                CategoryId = createProductDto.CategoryId,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                StockThreshold = createProductDto.StockThreshold,
                ImageUrl = createProductDto.ImageUrl,
                IsActive = true,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            await _products.InsertOneAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        // Update a product by vendor
        public async Task<ProductDto> UpdateProductAsync(string productId, UpdateProductDto updateProductDto, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId) throw new Exception("Product not found or access denied.");

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.CategoryId = updateProductDto.CategoryId;
            product.Price = updateProductDto.Price;
            product.Stock=updateProductDto.Stock;
            product.StockThreshold = updateProductDto.StockThreshold;
            product.ImageUrl = updateProductDto.ImageUrl;
            product.DateModified = DateTime.UtcNow;
            product.IsActive = updateProductDto.IsActive;

            await _products.ReplaceOneAsync(p => p.Id == productId, product);
            return _mapper.Map<ProductDto>(product);
        }

        // Delete a product by vendor
        public async Task<bool> DeleteProductAsync(string productId, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId) throw new Exception("Product not found or access denied.");

            var result = await _products.DeleteOneAsync(p => p.Id == productId);
            return result.DeletedCount > 0;
        }

        // Get a product by ID
        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }

        // Get products with filters
        public async Task<IEnumerable<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
        {
            var filter = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrEmpty(parameters.CategoryId))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.CategoryId, parameters.CategoryId));

            if (!string.IsNullOrEmpty(parameters.VendorId))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.VendorId, parameters.VendorId));

            if (!string.IsNullOrEmpty(parameters.Name))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(parameters.Name, "i")));

            if (parameters.MinPrice.HasValue)
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Gte(p => p.Price, (double)parameters.MinPrice.Value));

            if (parameters.MaxPrice.HasValue)
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Lte(p => p.Price, (double)parameters.MaxPrice.Value));

            if (parameters.IsActive.HasValue)
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.IsActive, parameters.IsActive.Value));

            var skip = (parameters.PageNumber <= 0 ? 0 : (parameters.PageNumber - 1)) * parameters.PageSize;
            var products = await _products.Find(filter).Skip(skip).Limit(parameters.PageSize).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        // Activate a product
        public async Task<bool> ActivateProductAsync(string productId)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }

        // Deactivate a product
        public async Task<bool> DeactivateProductAsync(string productId)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }

        // Update inventory
        public async Task<bool> UpdateInventoryAsync(string productId, InventoryUpdateDto inventoryUpdateDto, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId) throw new Exception("Product not found or access denied.");

            product.Stock = inventoryUpdateDto.Stock;
            product.StockThreshold = inventoryUpdateDto.StockThreshold;
            product.DateModified = DateTime.UtcNow;

            var result = await _products.ReplaceOneAsync(p => p.Id == productId, product);
            await CheckLowStockAsync(productId);
            return result.ModifiedCount > 0;
        }

        // Check stock levels and send low stock notifications
        public async Task CheckLowStockAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product != null && product.Stock <= product.StockThreshold)
            {
                var message = $"Low stock alert: Product '{product.Name}' is low on stock.";
                await _notificationService.SendNotificationAsync(product.VendorId, message);
            }
        }

        // Remove stock from a product
        public async Task<bool> RemoveProductStockAsync(string productId, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId) throw new Exception("Product not found or access denied.");

            var update = Builders<Product>.Update.Set(p => p.Stock, 0);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }

        // Get all active products
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var activeCategoryIds = await _categoryCollection.Find(c => c.IsActive).Project(c => c.Id).ToListAsync();
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.In(p => p.CategoryId, activeCategoryIds),
                Builders<Product>.Filter.Eq(p => p.IsActive, true)
            );
            var products = await _products.Find(filter).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        // Search products by keyword
        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword)
        {
            var filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(keyword, "i")),
                Builders<Product>.Filter.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(keyword, "i"))
            );

            var products = await _products.Find(filter).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        // Get all products for a vendor
        public async Task<IEnumerable<ProductDto>> GetProductsByVendorIdAsync(string vendorId)
        {
            var products = await _products.Find(p => p.VendorId == vendorId).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        // Reduce stock for a product
        public async Task<bool> ReduceStockAsync(string productId, int quantity)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null) return false;
            if (product.Stock < quantity) throw new InvalidOperationException("Not enough stock available.");

            var update = Builders<Product>.Update.Inc(p => p.Stock, -quantity);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }

        // Increase stock for a product
        public async Task<bool> IncreaseStockAsync(string productId, int quantity)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null) return false;

            var update = Builders<Product>.Update.Inc(p => p.Stock, quantity);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }

        // Check low stock for all products
        public async Task CheckAllProductsForLowStockAsync()
        {
            var products = await _products.Find(Builders<Product>.Filter.Empty).ToListAsync();
            foreach (var product in products)
            {
                if (product.Stock <= product.StockThreshold)
                {
                    var message = $"Low stock alert: Product '{product.Name}' is low on stock.";
                    await _notificationService.SendNotificationAsync(product.VendorId, message, product.Id);
                }
            }
        }
    }
}
