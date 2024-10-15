/***************************************************************************
 * File: Order.cs
 * Description: Model representing an order in the e-commerce system. 
 *              Contains order details, shipping address, order items, 
 *              and status tracking.
 ***************************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ecommerceWebServicess.Models
{
    public class Order
    {
        // Unique identifier for the order
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("orderId")]
        public string OrderId { get; set; }



        // Reference to the customer (user ID)
        [Required]
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        // Timestamp when the order was created
        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Current status of the order
        [Required]
        [BsonElement("status")]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Total amount of the order
        [Required]
        [BsonElement("totalAmount")]
        [Range(0, Double.MaxValue, ErrorMessage = "Total amount must be a positive value.")]
        public double TotalAmount { get; set; }

        // Shipping address for the order
        [Required]
        [BsonElement("shippingAddress")]
        public Address ShippingAddress { get; set; }

        // List of items in the order
        [Required]
        [BsonElement("orderItems")]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class Address
    {
        // Street address
        [Required]
        [BsonElement("street")]
        public string Street { get; set; }

        // City name
        [Required]
        [BsonElement("city")]
        public string City { get; set; }

        // ZIP code (up to 10 characters)
        [Required]
        [BsonElement("zip")]
        [StringLength(10, ErrorMessage = "ZIP code must be up to 10 characters.")]
        public string Zip { get; set; }
    }

    public class OrderItem
    {
        // Reference to the product ID
        [Required]
        [BsonElement("productId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        // Product code (denormalized for faster access)
        [Required]
        [BsonElement("productCode")]
        public string ProductCode { get; set; }

        // Product name (denormalized for faster access)
        [Required]
        [BsonElement("productName")]
        public string ProductName { get; set; }

        // Price of the product
        [Required]
        [BsonElement("productPrice")]
        [Range(0, Double.MaxValue, ErrorMessage = "Product price must be a positive value.")]
        public double ProductPrice { get; set; }

        // Quantity ordered
        [Required]
        [BsonElement("quantity")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        // Reference to the vendor ID
        [Required]
        [BsonElement("vendorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        // Vendor name (denormalized for faster access)
        [Required]
        [BsonElement("vendorName")]
        public string VendorName { get; set; }

        // Fulfillment status of the order item
        [Required]
        [BsonElement("fulfillmentStatus")]
        [EnumDataType(typeof(FulfillmentStatusEnum))]
        public FulfillmentStatusEnum FulfillmentStatus { get; set; } = FulfillmentStatusEnum.Pending;

        // URL of the product image
        [BsonElement("imageUrl")]
        [Required]
        public string ImageUrl { get; set; }
       

    }

    // Enumeration for order status
    public enum OrderStatus
    {
        Pending,
        PartiallyFulfilled,
        Fulfilled,
        Cancelled
       
    }

    // Enumeration for fulfillment status of order items
    public enum FulfillmentStatusEnum
    {
        Pending,
        Delivered,
        Shipped,
        Cancelled
    }
}
