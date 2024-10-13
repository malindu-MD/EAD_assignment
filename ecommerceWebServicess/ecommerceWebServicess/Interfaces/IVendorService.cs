/***************************************************************************
 * File: IVendorService.cs
 * Description: Interface defining vendor-related operations such as 
 *              creating vendors, adding comments/ratings, fetching vendor 
 *              details, and updating vendor information.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerceWebServicess.Interfaces
{
    public interface IVendorService
    {
        /// <summary>
        /// Creates a new vendor.
        /// </summary>
        Task<Vendor> CreateVendorAsync(CreateVendorDto vendorDto);

        /// <summary>
        /// Adds a comment and rating for a vendor.
        /// </summary>
        Task<bool> AddCommentAndRatingAsync(string vendorId, AddVendorCommentDto commentDto);

        /// <summary>
        /// Retrieves all vendors with user details.
        /// </summary>
        Task<IEnumerable<VendorWithUserDetailsDto>> GetAllVendorsAsync();

        /// <summary>
        /// Retrieves a vendor by their ID.
        /// </summary>
        Task<VendorWithUserDetailsDto> GetVendorByIdAsync(string vendorId);

        /// <summary>
        /// Updates an existing vendor by their ID.
        /// </summary>
        Task<bool> UpdateVendorAsync(string vendorId, UpdateVendorDto vendorDto);

        /// <summary>
        /// Retrieves all comments for a specific vendor.
        /// </summary>
        Task<IEnumerable<VendorComment>> GetVendorCommentsAsync(string vendorId);

        /// <summary>
        /// Retrieves the average rating of a vendor.
        /// </summary>
        Task<double?> GetVendorRatingAsync(string vendorId);

        /// <summary>
        /// Edits a comment and rating for a specific vendor by a user.
        /// </summary>
        Task<bool> EditCommentAndRatingAsync(string vendorId, string userId, AddVendorCommentDto updatedCommentDto);
    }
}
