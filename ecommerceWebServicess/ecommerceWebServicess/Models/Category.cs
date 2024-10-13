/***************************************************************************
 * File: Category.cs
 * Description: Model representing a category in the e-commerce system. 
 *              Stores category details such as name, status, and timestamps.
 ***************************************************************************/

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace ecommerceWebServicess.Models
{
    public class Category
    {
        /// <summary>
        /// Unique identifier for the category.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Name of the category.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the category is active.
        /// </summary>
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date when the category was created.
        /// </summary>
        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the category was last modified.
        /// </summary>
        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
    }
}
