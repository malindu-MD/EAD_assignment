/***************************************************************************
 * File: Vendor.cs
 * Description: Model representing a vendor in the e-commerce system.
 *              Includes vendor details, business name, ratings, and comments.
 ***************************************************************************/

using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ecommerceWebServicess.Models
{
    public class Vendor
    {
        // Unique identifier for the vendor
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Reference to the user who owns the vendor
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        // Business name of the vendor
        public string BusinessName { get; set; }

        // Average rating of the vendor
        public double AverageRating { get; set; } = 0.0;

        // List of comments and ratings associated with the vendor
        public List<VendorComment> Comments { get; set; } = new List<VendorComment>();
    }

    public class VendorComment
    {
        // Reference to the user who left the comment
        public string UserId { get; set; }

        // Display name of the user
        public string DisplayName { get; set; }

        // Comment text
        public string Comment { get; set; }

        // Rating given by the user
        public int Rating { get; set; }

        // Date the comment was posted
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
    }
}
