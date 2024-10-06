namespace ecommerceWebServicess.DTOs
{
    public class CreateVendorDto
    {

        public string Username { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PasswordHash { get; set; }  // This should be hashed before storing it in the database

        public string BusinessName { get; set; }  // Vendor's business name

    }
}
