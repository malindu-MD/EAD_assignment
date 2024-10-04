using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceWebAPI.Entities
{
    public class ProductList
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("visibleName"), BsonRepresentation(BsonType.String)] 
        public required string visibleName { get; set; }

        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public string? description { get; set; }

    }
}
