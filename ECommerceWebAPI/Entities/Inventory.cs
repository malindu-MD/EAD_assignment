using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceWebAPI.Entities
{
    public class Inventory
    {
        [BsonElement("productId"), BsonRepresentation(BsonType.ObjectId)]
        public string? productId { get; set; }

        [BsonElement("amount"), BsonRepresentation(BsonType.Int32)]
        public required int amount { get; set; }
    }
}
