/***************************************************************************
 * File: Notification.cs
 * Description: Model representing a notification in the e-commerce system.
 *              Contains details about the user, product, and notification status.
 ***************************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace ecommerceWebServicess.Models
{
    public class Notification
    {
        // Unique identifier for the notification
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // ID of the user receiving the notification
        [BsonElement("userId")]
        public string UserId { get; set; }

        // ID of the product associated with the notification
        [BsonElement("productId")]
        public string ProductId { get; set; }

        // Notification message
        [BsonElement("message")]
        public string Message { get; set; }

        // Status indicating whether the notification has been read
        [BsonElement("isRead")]
        public bool IsRead { get; set; } = false;

        // Date when the notification was created
        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
