using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceWebAPI.Entities
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("createdBy"), BsonRepresentation(BsonType.ObjectId)]
        public required string createdBy { get; set; }

        [BsonElement("totalPrice"), BsonRepresentation(BsonType.Double)]
        public double totalPrice { get; set; } = 0.00;

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? status { get; set; } = "Processing";

        [BsonElement("orderItems")]
        public required List<OrderItem> orderItems { get; set; } = new List<OrderItem>();
    }
}
