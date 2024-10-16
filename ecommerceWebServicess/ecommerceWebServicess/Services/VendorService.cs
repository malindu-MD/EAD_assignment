/**************************************************************************
 * File: VendorService.cs
 * Description: Handles vendor management, comments, and ratings.
 **************************************************************************/

using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Helpers;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ecommerceWebServicess.Services
{
    public class VendorService : IVendorService
    {
        private readonly IMongoCollection<Vendor> _vendorCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly PasswordHasher _passwordHasher;

        public VendorService(IMongoClient mongoClient, PasswordHasher passwordHasher)
        {
            var database = mongoClient.GetDatabase("ECommerceDB");
            _vendorCollection = database.GetCollection<Vendor>("Vendors");
            _userCollection = database.GetCollection<User>("Users");
            _passwordHasher = passwordHasher;
        }

        // Create a new vendor
        public async Task<Vendor> CreateVendorAsync(CreateVendorDto vendorDto)
        {
            var hashedPassword = _passwordHasher.HashPassword(vendorDto.PasswordHash);

            var user = new User
            {
                Username = vendorDto.Username,
                Email = vendorDto.Email,
                PhoneNumber = vendorDto.PhoneNumber,
                PasswordHash = hashedPassword,
                Role = "Vendor",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userCollection.InsertOneAsync(user);

            var vendor = new Vendor
            {
                UserId = user.Id.ToString(),
                BusinessName = vendorDto.BusinessName,
                AverageRating = 0.0,
                Comments = new List<VendorComment>(),
            };

            await _vendorCollection.InsertOneAsync(vendor);
            return vendor;
        }

        // Get all vendors with user details
        public async Task<IEnumerable<VendorWithUserDetailsDto>> GetAllVendorsAsync()
        {
            var vendors = await _vendorCollection.Find(Builders<Vendor>.Filter.Empty).ToListAsync();
            var vendorWithUserDetailsList = new List<VendorWithUserDetailsDto>();

            foreach (var vendor in vendors)
            {
                var user = await _userCollection.Find(u => u.Id == ObjectId.Parse(vendor.UserId)).FirstOrDefaultAsync();
                if (user != null)
                {
                    vendorWithUserDetailsList.Add(new VendorWithUserDetailsDto
                    {
                        VendorId = vendor.UserId,
                        BusinessName = vendor.BusinessName,
                        AverageRating = vendor.AverageRating,
                        Comments = vendor.Comments,
                        Username = user.Username,
                        Email = user.Email,
                        CreatedAt=user.CreatedAt,
                        PhoneNumber = user.PhoneNumber,
                        IsActive = user.IsActive
                    });
                }
            }

            return vendorWithUserDetailsList;
        }

        // Get vendor by ID
        public async Task<VendorWithUserDetailsDto> GetVendorByIdAsync(string vendorId)
        {
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            if (vendor == null) return null;

            var user = await _userCollection.Find(u => u.Id == ObjectId.Parse(vendor.UserId)).FirstOrDefaultAsync();

            if (user != null)
            {
                return new VendorWithUserDetailsDto
                {
                    VendorId = vendor.UserId,
                    BusinessName = vendor.BusinessName,
                    AverageRating = vendor.AverageRating,
                    Comments = vendor.Comments,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive
                };
            }

            return null;
        }

        // Update vendor details
        public async Task<bool> UpdateVendorAsync(string vendorId, UpdateVendorDto vendorDto)
        {
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();

            if (vendor == null) return false; // Vendor not found

            var hashedPassword = _passwordHasher.HashPassword(vendorDto.PasswordHash);


            var update = Builders<Vendor>.Update.Set(v => v.BusinessName, vendorDto.BusinessName);
            var userUpdate = Builders<User>.Update
               .Set(u => u.Username, vendorDto.Username)
               .Set(u => u.Email, vendorDto.Email)
               .Set(u => u.PhoneNumber, vendorDto.PhoneNumber)
               .Set(u=> u.PasswordHash,hashedPassword);


            var vendorResult = await _vendorCollection.UpdateOneAsync(v => v.UserId == vendorId, update);
            var userResult = await _userCollection.UpdateOneAsync(u => u.Id == ObjectId.Parse(vendorId), userUpdate);

            return vendorResult.ModifiedCount > 0 || userResult.ModifiedCount > 0; // Return true if either was modified

        }

        // Add comment and rating
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
                DatePosted = DateTime.UtcNow
            };

            vendor.Comments.Add(comment);
            vendor.AverageRating = vendor.Comments.Average(c => c.Rating);

            var update = Builders<Vendor>.Update
                .Set(v => v.Comments, vendor.Comments)
                .Set(v => v.AverageRating, vendor.AverageRating);

            var result = await _vendorCollection.UpdateOneAsync(v => v.UserId == vendorId, update);
            return result.ModifiedCount > 0;
        }

        // Get all comments for a vendor
        public async Task<IEnumerable<VendorComment>> GetVendorCommentsAsync(string vendorId)
        {
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            return vendor?.Comments ?? new List<VendorComment>();
        }

        // Get vendor rating
        public async Task<double?> GetVendorRatingAsync(string vendorId)
        {
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            return vendor?.AverageRating;
        }

        // Edit comment and rating
        public async Task<bool> EditCommentAndRatingAsync(string vendorId, string userId, AddVendorCommentDto updatedCommentDto)
        {
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            if (vendor == null) return false;

            var comment = vendor.Comments.FirstOrDefault(c => c.UserId == userId);
            if (comment == null) return false;

            comment.Comment = updatedCommentDto.Comment;
            comment.DisplayName = updatedCommentDto.DisplayName;
            comment.Rating = updatedCommentDto.Rating;
            comment.DatePosted = DateTime.UtcNow;

            vendor.AverageRating = vendor.Comments.Average(c => c.Rating);

            var update = Builders<Vendor>.Update
                .Set(v => v.Comments, vendor.Comments)
                .Set(v => v.AverageRating, vendor.AverageRating);

            var result = await _vendorCollection.UpdateOneAsync(v => v.UserId == vendorId, update);
            return result.ModifiedCount > 0;
        }



        // Method to get a single comment by vendorId and userId
        public async Task<VendorComment> GetCommentByVendorAndUserIdAsync(string vendorId, string userId)
        {
            // Fetch the vendor
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();

            // If vendor doesn't exist, return null
            if (vendor == null) return null;

            // Find the comment by userId
            var comment = vendor.Comments.FirstOrDefault(c => c.UserId == userId);

            return comment; // Will return null if not found
        }


        // Get all comments by user ID, along with the associated vendor details
        public async Task<IEnumerable<CommentWithVendorDetailsDto>> GetAllCommentsByUserIdAsync(string userId)
        {
            // Find all vendors
            var vendors = await _vendorCollection.Find(Builders<Vendor>.Filter.Empty).ToListAsync();

            // Initialize a list to hold comments and their vendor details
            var allCommentsWithVendorDetails = new List<CommentWithVendorDetailsDto>();

            // Loop through each vendor to get their comments
            foreach (var vendor in vendors)
            {
                // Filter comments for the specific user ID
                var userComments = vendor.Comments.Where(c => c.UserId == userId);

                // Add each comment along with the vendor details to the list
                foreach (var comment in userComments)
                {
                    allCommentsWithVendorDetails.Add(new CommentWithVendorDetailsDto
                    {
                        VendorId = vendor.UserId, // Assuming vendor.UserId is the vendor ID
                        BusinessName = vendor.BusinessName, // Add vendor's business name
                        Comment = comment
                    });
                }
            }

            return allCommentsWithVendorDetails; // Return the list of comments with vendor details
        }



        public async Task<bool> DeleteVendorAsync(string vendorId)
        {
            // Find the vendor by ID
            var vendor = await _vendorCollection.Find(v => v.UserId == vendorId).FirstOrDefaultAsync();
            if (vendor == null) return false; // Vendor not found

            // Remove the vendor document from the Vendors collection
            var deleteVendorResult = await _vendorCollection.DeleteOneAsync(v => v.UserId == vendorId);

            // Also remove the associated user document from the Users collection
            var deleteUserResult = await _userCollection.DeleteOneAsync(u => u.Id == ObjectId.Parse(vendor.UserId));

            return deleteVendorResult.DeletedCount > 0 && deleteUserResult.DeletedCount > 0;
        }





    }
}



