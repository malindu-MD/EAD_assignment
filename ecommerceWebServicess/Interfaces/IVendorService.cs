using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Models;

namespace ecommerceWebServicess.Interfaces
{
    public interface IVendorService
    {
       
        Task<Vendor> CreateVendorAsync(CreateVendorDto vendorDto);
        Task<bool> AddCommentAndRatingAsync(string vendorId, AddVendorCommentDto commentDto);
        Task<IEnumerable<VendorWithUserDetailsDto>> GetAllVendorsAsync();
        Task<VendorWithUserDetailsDto> GetVendorByIdAsync(string vendorId);
        Task<bool> UpdateVendorAsync(string vendorId, UpdateVendorDto vendorDto);

        Task<IEnumerable<VendorComment>> GetVendorCommentsAsync(string vendorId);
        Task<double?> GetVendorRatingAsync(string vendorId);

        Task<bool> EditCommentAndRatingAsync(string vendorId, string userId, AddVendorCommentDto updatedCommentDto);


    }
}
