using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Utilities.Database;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerceWebAPI.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _collection;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public ProductService(MongoDBService mongoDBService, NotificationService notificationService, UserService userService)
        {
            _collection = mongoDBService.Database?.GetCollection<Product>("Product");
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {            
            return await _collection.Find(FilterDefinition<Product>.Empty).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAll(string id)
        {
            var name = _userService.findVendorName(id);
            var filter = Builders<Product>.Filter.Eq(x => x.vendorName, name);
            return await _collection.Find(filter).ToListAsync();           
        }

        public Product? GetById(string id)
        {
            var filter = Builders<Product>.Filter.Eq(x => x._id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Add(Product data)
        {
            _collection.InsertOneAsync(data);         
        }

        public void Update(Product data)
        {
            var filter = Builders<Product>.Filter.Eq(x => x._id, data._id);
            _collection.ReplaceOneAsync(filter, data);
        }

        public void UpdateTotalStockAmount(Inventory data, string vendorId)
        {
            var filter = Builders<Product>.Filter.Eq(x => x._id, data.productId);
            var product = _collection.Find(filter).FirstOrDefault();

            if ((product is not null) && ((product.totalAmount + data.amount) < 0) )            
                throw new InvalidOperationException("Total amount for the product cannot be less than 0.");            

            var update = Builders<Product>.Update.Inc(x => x.totalAmount, data.amount);
            _collection.UpdateOneAsync(filter, update);

            if ((product.totalAmount + data.amount) < 5)
                SetLowStockAlert(vendorId, product.name);
        }        

        public void UpdateSales(int amount, string productId)
        {
            var filter = Builders<Product>.Filter.Eq(x => x._id, productId);
            var product = _collection.Find(filter).FirstOrDefault();

            if ((product is not null) && ((product.sales + amount) < 0))
                throw new InvalidOperationException("Total sales for the product cannot be less than 0.");

            var update = Builders<Product>.Update
                                                .Inc(x => x.sales, amount)
                                                .Inc(x => x.totalAmount, (-amount));
            _collection.UpdateOneAsync(filter, update);

            if ((product.totalAmount + amount) < 5)
                SetLowStockAlert(_userService.findVendorId(product.vendorName), product.name);
        }

        public void Delete(string id)
        {
            var filter = Builders<Product>.Filter.Eq(x => x._id, id);
            _collection.DeleteOneAsync(filter);
        }

        private void SetLowStockAlert(string vendorId, string product) 
        {
            string msg = "Stock is low for " + product + ". Please add more stock.";
            _notificationService.Add(vendorId, msg);
        }           
    }
}
