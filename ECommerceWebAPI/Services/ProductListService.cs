using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Utilities.Database;
using MongoDB.Driver;

namespace ECommerceWebAPI.Services
{
    public class ProductListService
    {
        private readonly IMongoCollection<ProductList> _collection;

        public ProductListService(MongoDBService mongoDBService)
        {
            _collection = mongoDBService.Database?.GetCollection<ProductList>("ProductList");
        }


        public async Task<IEnumerable<ProductList>> GetAll()
        {
            return await _collection.Find(FilterDefinition<ProductList>.Empty).ToListAsync();
        }
      
        public ProductList? GetById(string id)
        {
            var filter = Builders<ProductList>.Filter.Eq(x => x._id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Add(ProductList data)
        {
            _collection.InsertOneAsync(data);
            
        }

        public void Update(ProductList data)
        {
            var filter = Builders<ProductList>.Filter.Eq(x => x._id, data._id);
            _collection.ReplaceOneAsync(filter, data);

        }

        public void Delete(String id)
        {
            var filter = Builders<ProductList>.Filter.Eq(x => x._id, id);
            _collection.DeleteOneAsync(filter);
        }
    }
}
