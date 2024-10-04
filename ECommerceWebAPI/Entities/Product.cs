using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceWebAPI.Entities
{
    public class Product
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("productListId"), BsonRepresentation(BsonType.ObjectId)]
        public string? productListId { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)] 
        public required string name { get; set; }

        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public string? description { get; set; }

        [BsonElement("unitPrice"), BsonRepresentation(BsonType.Double)]
        public required double unitPrice { get; set; }

        [BsonElement("totalAmount"), BsonRepresentation(BsonType.Int32)]
        public required int totalAmount { get; set; }

        [BsonElement("sales"), BsonRepresentation(BsonType.Int32)]
        public int sales { get; set; } = 0;

        [BsonElement("vendorName"), BsonRepresentation(BsonType.ObjectId)]
        public required string? vendorName { get; set; }

        [BsonElement("imgLink"), BsonRepresentation(BsonType.String)]
        public string? imgLink { get; set; }

    }
}
