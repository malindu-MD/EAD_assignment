using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ecommerceWebServicess.Models
{
    public class Category
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
    }
}
