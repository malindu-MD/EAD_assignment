using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Utilities.Database;
using MongoDB.Driver;

namespace ECommerceWebAPI.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _collection;
        private readonly ProductService _productService;
        private readonly UserService _userService;

        public OrderService(MongoDBService mongoDBService, ProductService productService, UserService userService)
        {
            _collection = mongoDBService.Database?.GetCollection<Order>("Order");
            _productService = productService;            
        }

        public async Task<IEnumerable<Order>> GetAll()
        {            
            return await _collection.Find(FilterDefinition<Order>.Empty).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAll(string id)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.createdBy, id);
            return await _collection.Find(filter).ToListAsync();           
        }

        public Order? GetOrderById(string id)
        {
            var filter = Builders<Order>.Filter.Eq(x => x._id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Add(Order order)
        {
            _collection.InsertOneAsync(order);            
        }

        public void UpdateStatus(string id, string status)
        {
            var filter = Builders<Order>.Filter.Eq(i => i._id, id);
            var update = Builders<Order>.Update.Set(i => i.status, status);           
            _collection.UpdateOneAsync(filter, update);
            Order order = GetOrderById(id);

            if (status.Equals("Dispatched") && order is not null)
            {
                foreach (OrderItem orderItem in order.orderItems)
                {
                    _productService.UpdateSales(orderItem.quantity, orderItem.productId);
                }
            }
        }

        public void Delete(string id)
        {
            var filter = Builders<Order>.Filter.Eq(x => x._id, id);
            _collection.DeleteOneAsync(filter);
        }
    }
}
