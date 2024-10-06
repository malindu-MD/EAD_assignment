using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ecommerceWebServicess.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }  // Reference to the User (Customer)

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [BsonElement("status")]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;  // Use enum directly instead of string

        [Required]
        [BsonElement("totalAmount")]
        [Range(0, Double.MaxValue, ErrorMessage = "Total amount must be a positive value.")]
        public double TotalAmount { get; set; }

        [Required]
        [BsonElement("shippingAddress")]
        public Address ShippingAddress { get; set; }

        [Required]
        [BsonElement("orderItems")]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class Address
    {
        [Required]
        [BsonElement("street")]
        public string Street { get; set; }

        [Required]
        [BsonElement("city")]
        public string City { get; set; }

        [Required]
        [BsonElement("zip")]
        [StringLength(10, ErrorMessage = "ZIP code must be up to 10 characters.")]
        public string Zip { get; set; }
    }



    public class OrderItem
    {
        [Required]
        [BsonElement("productId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }  // Reference to Product

        [Required]
        [BsonElement("productName")]
        public string ProductName { get; set; }  // Denormalized for fast retrieval

        [Required]
        [BsonElement("productPrice")]
        [Range(0, Double.MaxValue, ErrorMessage = "Product price must be a positive value.")]
        public double ProductPrice { get; set; }  // Denormalized

        [Required]
        [BsonElement("quantity")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [BsonElement("vendorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }  // Reference to Vendor

        [Required]
        [BsonElement("vendorName")]
        public string VendorName { get; set; }  // Denormalized for fast retrieval

        [Required]
        [BsonElement("fulfillmentStatus")]
        [EnumDataType(typeof(FulfillmentStatusEnum))]
        public FulfillmentStatusEnum FulfillmentStatus { get; set; } = FulfillmentStatusEnum.Pending;  // Use enum instead of string
    }


     public enum OrderStatus
      {
        Pending,
        PartiallyFulfilled,
        Fulfilled,
        Cancelled
      }

    public enum FulfillmentStatusEnum
    {
        Pending,
        Fulfilled,
        Shipped,
        Cancelled
    }
}


