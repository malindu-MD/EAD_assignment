using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ecommerceWebServicess.Models
{
    [BsonIgnoreExtraElements]
    public class Product
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("productId")]
        public string ProductId { get; set; }

        [BsonElement("vendorId")]
        public string VendorId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("categoryId")]
        public string CategoryId { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; }

        [BsonElement("stockThreshold")]
        public int StockThreshold { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; }

        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; } = DateTime.UtcNow;

    }
}
