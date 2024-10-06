using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ecommerceWebServicess.Models
{
    public class Notification
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("productId")]
        public string ProductId { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("isRead")]
        public bool IsRead { get; set; } = false;

        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
