using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Helpers;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class VendorService : IVendorService
    {

        private readonly IMongoCollection<Vendor> _vendorCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly PasswordHasher _passwordHasher;


        public VendorService(IMongoClient mongoClient, PasswordHasher passwordHasher) {

            var database = mongoClient.GetDatabase("ECommerceDB");
            _vendorCollection = database.GetCollection<Vendor>("Vendors");
            _userCollection = database.GetCollection<User>("Users");
            _passwordHasher = passwordHasher;

        }


        public async Task<Vendor> CreateVendorAsync(CreateVendorDto vendorDto)
        {
            var hashedPassword = _passwordHasher.HashPassword(vendorDto.PasswordHash);


            var user = new User
            {
                Username = vendorDto.Username,
                Email = vendorDto.Email,
                PhoneNumber = vendorDto.PhoneNumber,
                PasswordHash = hashedPassword,  // Ensure password is hashed
                Role = "Vendor",
                IsActive = true,  // Automatically activate the vendor
                CreatedAt = DateTime.UtcNow
            };

            await _userCollection.InsertOneAsync(user);

            // 2. Insert into the Vendor collection (with a reference to the UserId)
            var vendor = new Vendor
            {
                UserId = user.Id.ToString(),  // Reference the User ID from the User collection
                BusinessName = vendorDto.BusinessName,
                AverageRating = 0.0,  // New vendor starts with no rating
                Comments = new List<VendorComment>(),
               
            };

            await _vendorCollection.InsertOneAsync(vendor);  // Insert into the Vendor collection

            return vendor;

        }


        // Get all vendors
        public async Task<IEnumerable<VendorWithUserDetailsDto>> GetAllVendorsAsync()
        {
            var vendors = await _vendorCollection.Find(Builders<Vendor>.Filter.Empty).ToListAsync();

            // List to hold vendor details combined with user details
            var vendorWithUserDetailsList = new List<VendorWithUserDetailsDto>();

            foreach (var vendor in vendors)
            {
                // Find the associated user from the User collection using the UserId
                var user = await _userCollection.Find(u => u.Id == ObjectId.Parse(vendor.UserId)).FirstOrDefaultAsync();
                if (user != null)
                {
                    var vendorWithUserDetails = new VendorWithUserDetailsDto
                    {
                        VendorId = vendor.UserId,
                        BusinessName = vendor.BusinessName,
                        AverageRating = vendor.AverageRating,
                        Comments = vendor.Comments,
                       

                        // User details
                        Username = user.Username,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        IsActive = user.IsActive
                    };
                    vendorWithUserDetailsList.Add(vendorWithUserDetails);
                }
            }

            return vendorWithUserDetailsList;
        }



        public async Task<VendorWithUserDetailsDto> GetVendorByIdAsync(string vendorId)
        {
            var objectId = ObjectId.Parse(vendorId);
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            if (vendor == null) return null;

            // Fetch the associated user
            var user = await _userCollection.Find(u => u.Id == ObjectId.Parse(vendor.UserId)).FirstOrDefaultAsync();

            // Combine vendor details with user details
            if (user != null)
            {
                return new VendorWithUserDetailsDto
                {
                    VendorId = vendor.UserId,
                    BusinessName = vendor.BusinessName,
                    AverageRating = vendor.AverageRating,
                    Comments = vendor.Comments,

                    // User details
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive
                };
            }

            return null;
        }

        // Update vendor information
        public async Task<bool> UpdateVendorAsync(string vendorId, UpdateVendorDto vendorDto)
        {
            var update = Builders<Vendor>.Update
                .Set(v => v.BusinessName, vendorDto.BusinessName);

            var objectId = ObjectId.Parse(vendorId);
            var result = await _vendorCollection.UpdateOneAsync(v => v.UserId == vendorId, update);
            return result.ModifiedCount > 0;
        }



        // Add a comment and rating for a vendor
        public async Task<bool> AddCommentAndRatingAsync(string vendorId, AddVendorCommentDto commentDto)
        {
           
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            if (vendor == null) return false;

            var comment = new VendorComment
            {
                UserId = commentDto.UserId,
                DisplayName = commentDto.DisplayName,
                Comment = commentDto.Comment,
                Rating = commentDto.Rating,
                DatePosted = System.DateTime.UtcNow
            };

            vendor.Comments.Add(comment);
            vendor.AverageRating = vendor.Comments.Average(c => c.Rating);

            var update = Builders<Vendor>.Update
                .Set(v => v.Comments, vendor.Comments)
                .Set(v => v.AverageRating, vendor.AverageRating);

            var result = await _vendorCollection.UpdateOneAsync(v => v.UserId == vendorId, update);
            return result.ModifiedCount > 0;
        }


        public async Task<IEnumerable<VendorComment>> GetVendorCommentsAsync(string vendorId)
        {
            var objectId = ObjectId.Parse(vendorId);
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            return vendor?.Comments ?? new List<VendorComment>();
        }


        public async Task<double?> GetVendorRatingAsync(string vendorId)
        {
            var objectId = ObjectId.Parse(vendorId);
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            return vendor?.AverageRating;
        }

        public async Task<bool> EditCommentAndRatingAsync(string vendorId, string userId, AddVendorCommentDto updatedCommentDto)
        {
            var objectId = ObjectId.Parse(vendorId);
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            if (vendor == null) return false;

            // Find the customer's comment
            var comment = vendor.Comments.FirstOrDefault(c => c.UserId == userId);
            if (comment == null) return false;

            // Update the comment and rating
            comment.Comment = updatedCommentDto.Comment;
            comment.DisplayName = updatedCommentDto.DisplayName;
            comment.Rating = updatedCommentDto.Rating;
            comment.DatePosted = DateTime.UtcNow;  // Update the timestamp

            // Recalculate the average rating
            vendor.AverageRating = vendor.Comments.Average(c => c.Rating);

            var update = Builders<Vendor>.Update
                .Set(v => v.Comments, vendor.Comments)
                .Set(v => v.AverageRating, vendor.AverageRating);

            var result = await _vendorCollection.UpdateOneAsync(v => v.UserId == vendorId, update);
            return result.ModifiedCount > 0;
        }

    }
}
