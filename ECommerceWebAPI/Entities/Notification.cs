using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceWebAPI.Entities
{
    public class Notification
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("message"), BsonRepresentation(BsonType.String)] 
        public required string message { get; set; }

        [BsonElement("targetUserId"), BsonRepresentation(BsonType.ObjectId)]
        public string? targetUserId { get; set; }

    }
}
