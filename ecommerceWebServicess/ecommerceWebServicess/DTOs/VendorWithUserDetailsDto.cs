using ecommerceWebServicess.Models;

namespace ecommerceWebServicess.DTOs
{
    public class VendorWithUserDetailsDto
    {

        // Vendor-specific fields
        public string VendorId { get; set; }
        public string BusinessName { get; set; }
        public double AverageRating { get; set; }
        public List<VendorComment> Comments { get; set; }
        public DateTime CreatedAt { get; set; }

        // User-specific fields
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

    }
}
