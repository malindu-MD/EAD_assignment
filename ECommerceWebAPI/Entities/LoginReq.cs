using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ECommerceWebAPI.Entities
{
    public class Login
    {

        public required string? userName { get; set; }

        public required string? credential { get; set; }

    }
}
