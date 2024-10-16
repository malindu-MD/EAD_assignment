namespace ecommerceWebServicess.DTOs
{
    public class UpdateVendorDto
    {
        public string BusinessName { get; set; }  // Updated business name of the vendor

        public string PhoneNumber { get; set; }  // Updated phone number of the vendor

        public string PasswordHash { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
       
    }
}
