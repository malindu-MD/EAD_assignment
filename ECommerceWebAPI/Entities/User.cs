using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ECommerceWebAPI.Entities
{
    public class User
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? _Id { get; set; }
        
        [BsonElement("nic"), BsonRepresentation(BsonType.String)]
        public string? nic { get; set; }

        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public required string email { get; set; }

        [BsonElement("firstName"), BsonRepresentation(BsonType.String)]
        public string? firstName { get; set; }

        [BsonElement("lastName"), BsonRepresentation(BsonType.String)]
        public string? lastName { get; set; }

        [BsonElement("userName"), BsonRepresentation(BsonType.String)]
        public required string? userName { get; set; }

        [BsonElement("address"), BsonRepresentation(BsonType.String)]
        public string? address { get; set; }

        [BsonElement("credential"), BsonRepresentation(BsonType.String)]
        public required string? credential { get; set; }

        [BsonElement("userType"), BsonRepresentation(BsonType.String)]
        public required string? userType { get; set; }

        [BsonElement("isActive"), BsonRepresentation(BsonType.String)]
        public Boolean isActive { get; set; } = true;

        [BsonElement("isLoggedIn"), BsonRepresentation(BsonType.String)]
        public Boolean isLoggedIn { get; set; } = false;
        
    }
}
