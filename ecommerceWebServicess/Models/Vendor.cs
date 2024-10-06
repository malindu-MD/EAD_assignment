using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ecommerceWebServicess.Models
{
    public class Vendor
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }  // Reference to the User collection

        public string BusinessName { get; set; }
        public double AverageRating { get; set; } = 0.0;

        public List<VendorComment> Comments { get; set; } = new List<VendorComment>();
    }

    public class VendorComment
    {
        public string UserId { get; set; }

        public string DisplayName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
    }
}
