using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notificationCollection;


        public NotificationService(IMongoClient mongoClient)
        {
            // Initialize the MongoDB notification collection

            var database = mongoClient.GetDatabase("ECommerceDB");
         
            _notificationCollection = database.GetCollection<Notification>("Notifications");
        }

        public async Task<IEnumerable<Notification>> GetNotificationByUserID(string userId)
        {
            // Fetch all notifications for the given userId
            var filter = Builders<Notification>.Filter.Eq(n => n.UserId, userId);
            var notifications = await _notificationCollection.Find(filter).ToListAsync();

            return notifications;
        }

        public async Task SendNotificationAsync(string userId, string message, string productId)
        {
            // Check if a notification for the same product already exists for this user
            var existingNotification = await _notificationCollection
                .Find(n => n.UserId == userId && n.ProductId == productId)
                .FirstOrDefaultAsync();

            // If a notification with the same productId already exists, don't insert a new one
            if (existingNotification != null)
            {
                Console.WriteLine($"Notification for product {productId} already exists for user {userId}. Skipping insert.");
                return;
            }

            // Create a new notification object
            var notification = new Notification
            {
                UserId = userId,
                ProductId = productId,
                Message = message,
                IsRead = false, // Set to unread when the notification is created
                DateCreated = DateTime.UtcNow
            };

            // Insert the notification into the MongoDB collection
            await _notificationCollection.InsertOneAsync(notification);

            // Optionally, you can log or perform any other necessary operations here
           
        }

        public Task SendNotificationAsync(string userId, string message)
        {
            throw new NotImplementedException();
        }
    }
}
