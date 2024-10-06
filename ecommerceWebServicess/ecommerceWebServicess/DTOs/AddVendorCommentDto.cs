namespace ecommerceWebServicess.DTOs
{
    public class AddVendorCommentDto
    {

        public string UserId { get; set; }  // ID of the user leaving the comment

        public string DisplayName { get; set; }

        public string Comment { get; set; }  // The comment text

        public int Rating { get; set; }  // The rating provided (e.g., 1-5 stars)


    }
}
