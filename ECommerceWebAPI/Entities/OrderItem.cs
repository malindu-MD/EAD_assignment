using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceWebAPI.Entities
{
    public class OrderItem
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("productId"), BsonRepresentation(BsonType.ObjectId)]
        public required string productId { get; set; }

        [BsonElement("quantity"), BsonRepresentation(BsonType.Int32)]
        public required int quantity { get; set; }

        [BsonElement("price"), BsonRepresentation(BsonType.Double)]
        public double price { get; set; } = 0.00;
    }
}
