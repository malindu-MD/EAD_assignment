using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceWebAPI.Entities
{
    public class OrderListDetails
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("orderId"), BsonRepresentation(BsonType.ObjectId)]
        public string? orderId { get; set; }

        [BsonElement("productId"), BsonRepresentation(BsonType.ObjectId)]
        public required string productId { get; set; }

        [BsonElement("quantity"), BsonRepresentation(BsonType.Int32)]
        public int quantity { get; set; } = 1;

        [BsonElement("price"), BsonRepresentation(BsonType.Double)]
        public double price { get; set; } = 0.00;
    }
}
