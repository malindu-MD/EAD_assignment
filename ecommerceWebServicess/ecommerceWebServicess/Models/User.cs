/*
    File: User.cs
    Author: [Your Name]
    Description: This file defines the User model that will be used in the application to store user details in MongoDB.
    The User model includes properties for Username, Email, PhoneNumber, PasswordHash, Role, IsActive, DeactivatedByUser, 
    and the date the user was created.
*/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ecommerceWebServicess.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }  // Added phone number field

        public string PasswordHash { get; set; }

        public string Role { get; set; }  // Administrator, Vendor, CSR, Customer

        public bool IsActive { get; set; } = false;  // Account needs activation by CSR or Administrator

        public bool DeactivatedByUser { get; set; } = false; // Set to true if the user deactivates their account

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
