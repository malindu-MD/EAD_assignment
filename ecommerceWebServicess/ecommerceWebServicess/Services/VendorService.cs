/**************************************************************************
 * File: VendorService.cs
 * Description: Handles vendor management, comments, and ratings.
 **************************************************************************/

using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Helpers;
using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
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
            var update = Builders<Vendor>.Update.Set(v => v.BusinessName, vendorDto.BusinessName);
            var result = await _vendorCollection.UpdateOneAsync(v => v.UserId == vendorId, update);
            return result.ModifiedCount > 0;
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
    }
}
