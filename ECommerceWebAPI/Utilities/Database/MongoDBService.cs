using MongoDB.Driver;

namespace ECommerceWebAPI.Utilities.Database
{
    public class MongoDBService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? _database;

        public MongoDBService(IConfiguration configuration)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("DbConnection");
            var mongoURL = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoURL);
            _database = mongoClient.GetDatabase(mongoURL.DatabaseName);
        }

        public IMongoDatabase? Database => _database;
    }
}
