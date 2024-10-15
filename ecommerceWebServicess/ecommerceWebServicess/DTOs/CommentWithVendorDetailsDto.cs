using ecommerceWebServicess.Models;

namespace ecommerceWebServicess.DTOs
{
    public class CommentWithVendorDetailsDto
    {

        public string VendorId { get; set; }
        public string BusinessName { get; set; }
        public VendorComment Comment { get; set; }


    }
}
