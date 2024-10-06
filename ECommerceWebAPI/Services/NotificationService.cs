using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Utilities.Database;
using MongoDB.Driver;

namespace ECommerceWebAPI.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<Notification> _collection;

        public NotificationService(MongoDBService mongoDBService)
        {
            _collection = mongoDBService.Database?.GetCollection<Notification>("Notification");
        }


        public async Task<IEnumerable<Notification>> GetAll(string id)
        {
            var filter = Builders<Notification>.Filter.Eq(x => x.targetUserId, id);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetAll()
        {
            var filter = Builders<Notification>.Filter.Eq(x => x.targetUserId, null);
            return await _collection.Find(filter).ToListAsync();
        }

        public void Add(Notification data)
        {
            _collection.InsertOneAsync(data);
            
        }

        public void Add(String targetUserId, String msg)
        {
            Notification data = new Notification
            {
                message = msg,
                targetUserId = targetUserId
            };
            _collection.InsertOneAsync(data);

        }

        public void Delete(String id)
        {
            var filter = Builders<Notification>.Filter.Eq(x => x._id, id);
            _collection.DeleteOneAsync(filter);
        }
    }
}
