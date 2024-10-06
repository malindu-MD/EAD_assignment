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

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, string vendorId)
        {
          

            var product = new Product
            {
                ProductId = string.Concat("P", Guid.NewGuid().ToString("N").AsSpan(0, 7)),// First letter "P" + 7-character GUID

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






        public async Task<ProductDto> UpdateProductAsync(string productId, UpdateProductDto updateProductDto, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId)
            {
                throw new Exception("Product not found or access denied.");
            }

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.CategoryId = updateProductDto.CategoryId;
            product.Price = updateProductDto.Price;
            product.ImageUrl = updateProductDto.ImageUrl;
            product.DateModified = DateTime.UtcNow;
            product.IsActive = updateProductDto.IsActive;


            await _products.ReplaceOneAsync(p => p.Id == productId, product);
            return _mapper.Map<ProductDto>(product);
        }



        public async Task<bool> DeleteProductAsync(string productId, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId)
            {
                throw new Exception("Product not found or access denied.");
            }

            
            var result = await _products.DeleteOneAsync(p => p.Id == productId);
            return result.DeletedCount > 0;
        }



        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
        {
            // Start with an empty filter
            var filter = Builders<Product>.Filter.Empty;

            // Filter by CategoryId if provided
            if (!string.IsNullOrEmpty(parameters.CategoryId))
            {
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.CategoryId, parameters.CategoryId));
            }

            // Filter by VendorId if provided
            if (!string.IsNullOrEmpty(parameters.VendorId))
            {
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.VendorId, parameters.VendorId));
            }

            // Filter by Name if provided (partial match search)
            if (!string.IsNullOrEmpty(parameters.Name))
            {
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(parameters.Name, "i"))); // case-insensitive
            }

            // Filter by MinPrice if provided
            if (parameters.MinPrice.HasValue)
            {
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Gte(p => p.Price, (double)parameters.MinPrice.Value));
            }

            // Filter by MaxPrice if provided
            if (parameters.MaxPrice.HasValue)
            {
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Lte(p => p.Price, (double)parameters.MaxPrice.Value));
            }

            // Filter by IsActive if provided
            if (parameters.IsActive.HasValue)
            {
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.IsActive, parameters.IsActive.Value));
            }

            // Apply pagination (skip and limit)
            var pageNumber = parameters.PageNumber <= 0 ? 1 : parameters.PageNumber;  // Default to 1 if PageNumber is not provided or invalid
            var pageSize = parameters.PageSize <= 0 ? 10 : parameters.PageSize;       // Default to 10 if PageSize is not provided or invalid

            // Apply pagination
            var skip = (pageNumber - 1) * pageSize;
            var products = await _products.Find(filter)
                                          .Skip(skip)
                                          .Limit(pageSize)
                                          .ToListAsync();


            // Map the results to ProductDto and return
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }


        public async Task<bool> ActivateProductAsync(string productId)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeactivateProductAsync(string productId)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }


        public async Task<bool> UpdateInventoryAsync(string productId, InventoryUpdateDto inventoryUpdateDto, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId)
            {
                throw new Exception("Product not found or access denied.");
            }

            product.Stock = inventoryUpdateDto.Stock;
            product.StockThreshold = inventoryUpdateDto.StockThreshold;
            product.DateModified = DateTime.UtcNow;

            var result = await _products.ReplaceOneAsync(p => p.Id == productId, product);

            await CheckLowStockAsync(productId);

            return result.ModifiedCount > 0;
        }

        public async Task CheckLowStockAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product != null && product.Stock <= product.StockThreshold)
            {
                var message = $"Low stock alert: Product '{product.Name}' is low on stock.";
                await _notificationService.SendNotificationAsync(product.VendorId, message);
            }
        }


        public async Task<bool> RemoveProductStockAsync(string productId, string vendorId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null || product.VendorId != vendorId)
            {
                throw new Exception("Product not found or access denied.");
            }

            var update = Builders<Product>.Update.Set(p => p.Stock, 0);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            // Step 1: Fetch all active categories from the Category collection
            var activeCategoryIds = await _categoryCollection
                .Find(c => c.IsActive)  // Fetch only active categories
                .Project(c => c.Id)     // Project only the Category IDs
                .ToListAsync();

            // Step 2: Create a filter to match products with categories in the activeCategoryIds list AND where the product itself is active
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.In(p => p.CategoryId, activeCategoryIds),  // Product's category is active
                Builders<Product>.Filter.Eq(p => p.IsActive, true)                  // Product itself is active
            );

            // Step 3: Fetch products with active categories and that are active themselves
            var products = await _products.Find(filter).ToListAsync();

            // Step 4: Map the result to ProductDto and return
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }


        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword)
        {
            var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(keyword, "i")),
            Builders<Product>.Filter.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(keyword, "i"))
             );

            // Get the list of products that match the keyword search
            var products = await _products.Find(filter).ToListAsync();

            // Map the list of products to a list of DTOs and return
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByVendorIdAsync(string vendorId)
        {
            // Find products that match the vendor ID
            var products = await _products.Find(p => p.VendorId == vendorId).ToListAsync();

            // Map the list of products to a list of DTOs and return
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }


        public async Task<bool> ReduceStockAsync(string productId, int quantity)
        {
            // Find the product
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();

            if (product == null)
            {
                return false; // Product not found
            }

            // Check if there is enough stock
            if (product.Stock < quantity)
            {
                throw new InvalidOperationException("Not enough stock available.");
            }

            // Reduce the stock
            var update = Builders<Product>.Update.Inc(p => p.Stock, -quantity);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);

            return result.ModifiedCount > 0;
        }


        public async Task<bool> IncreaseStockAsync(string productId, int quantity)
        {
            // Find the product
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();

            if (product == null)
            {
                return false; // Product not found
            }

            // Increase the stock
            var update = Builders<Product>.Update.Inc(p => p.Stock, quantity);
            var result = await _products.UpdateOneAsync(p => p.Id == productId, update);

            return result.ModifiedCount > 0;
        }


        public async Task CheckAllProductsForLowStockAsync()
        {
            // Fetch all products
            var products = await _products.Find(Builders<Product>.Filter.Empty).ToListAsync();

            

            foreach (var product in products)
            {

               
                // Check if stock is below or equal to the threshold
                if (product.Stock <= product.StockThreshold)
                {
                   
                    var message = $"Low stock alert: Product '{product.Name}' is low on stock.";
                    Console.WriteLine(message);
                    await _notificationService.SendNotificationAsync(product.VendorId, message,product.Id);
                }
            }
        }



    }
}
