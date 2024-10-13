/***************************************************************************
 * File: Product.cs
 * Description: Model representing a product in the e-commerce system.
 *              Includes product details, vendor reference, stock tracking,
 *              and status flags.
 ***************************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ecommerceWebServicess.Models
{
    [BsonIgnoreExtraElements]
    public class Product
    {
        // Unique identifier for the product
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // External product ID (optional)
        [BsonElement("productId")]
        public string ProductId { get; set; }

        // Reference to the vendor ID
        [BsonElement("vendorId")]
        public string VendorId { get; set; }

        // Name of the product
        [BsonElement("name")]
        public string Name { get; set; }

        // Description of the product
        [BsonElement("description")]
        public string Description { get; set; }

        // Reference to the category ID
        [BsonElement("categoryId")]
        public string CategoryId { get; set; }

        // Price of the product
        [BsonElement("price")]
        public double Price { get; set; }

        // Current stock quantity
        [BsonElement("stock")]
        public int Stock { get; set; }

        // Minimum stock threshold before alert
        [BsonElement("stockThreshold")]
        public int StockThreshold { get; set; }

        // Flag indicating if the product is active
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        // URL of the product image
        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; }

        // Date the product was created
        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Date the product was last modified
        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
    }
}
