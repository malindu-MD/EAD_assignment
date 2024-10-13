/**************************************************************************
 * File: NotificationService.cs
 * Description: Service for handling notifications, including sending and 
 *              retrieving notifications for users.
 **************************************************************************/

using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notificationCollection;

        // Constructor to initialize MongoDB notification collection
        public NotificationService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _notificationCollection = database.GetCollection<Notification>("Notifications");
        }

        // Get all notifications by user ID
        public async Task<IEnumerable<Notification>> GetNotificationByUserID(string userId)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.UserId, userId);
            return await _notificationCollection.Find(filter).ToListAsync();
        }

        // Send a notification for a specific product
        public async Task SendNotificationAsync(string userId, string message, string productId)
        {
            // Check if notification already exists for the product
            var existingNotification = await _notificationCollection
                .Find(n => n.UserId == userId && n.ProductId == productId)
                .FirstOrDefaultAsync();

            if (existingNotification != null)
            {
                Console.WriteLine($"Notification for product {productId} already exists for user {userId}. Skipping insert.");
                return;
            }

            // Create and insert new notification
            var notification = new Notification
            {
                UserId = userId,
                ProductId = productId,
                Message = message,
                IsRead = false,
                DateCreated = DateTime.UtcNow
            };

            await _notificationCollection.InsertOneAsync(notification);
        }

        // Method not yet implemented
        public Task SendNotificationAsync(string userId, string message)
        {
            throw new NotImplementedException();
        }
    }
}
