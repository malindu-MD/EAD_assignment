using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Utilities.Database;
using MongoDB.Driver;

namespace ECommerceWebAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _collection;
        private readonly NotificationService _notificationService;

        public UserService(MongoDBService mongoDBService, NotificationService notificationService)
        {
            _collection = mongoDBService.Database?.GetCollection<User>("User");
            _notificationService = notificationService;
        }


        public async Task<IEnumerable<User>> GetAll()
        {            
            return await _collection.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetNewCustomers()
        {
            var filter = Builders<User>.Filter.And(
               Builders<User>.Filter.Eq(x => x.userType, "Customer"),
               Builders<User>.Filter.Eq(x => x.isActive, false)
            ); 
            
            return await _collection.Find(filter).ToListAsync();           
        }

        public User? GetById(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x._Id, id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Add(User data)
        {
            if (data.userType.Equals("Customer"))
            {
                data.isActive =  false;
                SendActivateAccountAlert(data.userName);
            }
            
            _collection.InsertOneAsync(data);            
        }

        public void Update(User data)
        {
            var filter = Builders<User>.Filter.Eq(x => x._Id, data._Id);
            _collection.ReplaceOneAsync(filter, data);

        }

        public void ActivateAccount(string userName)
        {
            var filter = Builders<User>.Filter.And(
               Builders<User>.Filter.Eq(x => x.userType, "Customer"),
               Builders<User>.Filter.Eq(x => x.userName, userName)
            ); 
            var update = Builders<User>.Update.Set(x => x.isActive, true);
            _collection.UpdateOneAsync(filter, update);
        }

        public void DeactivateAccount(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x._Id, id);
            var update = Builders<User>.Update.Set(x => x.isActive, false);
            _collection.UpdateOneAsync(filter, update);            
        }

        public string? findVendorName(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x._Id, id);
            return _collection.Find(filter).FirstOrDefault().userName;
        }

        public string? findVendorId(string userName)
        {
            var filter = Builders<User>.Filter.Eq(x => x.userName, userName);
            return _collection.Find(filter).FirstOrDefault()._Id;
        }

        private void SendActivateAccountAlert(string userName) 
        {
            string msg = "Activate user account: " + userName + ".";
            _notificationService.Add(null, msg);
        }       
    }
}
