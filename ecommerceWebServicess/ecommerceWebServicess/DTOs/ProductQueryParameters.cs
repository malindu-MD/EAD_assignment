namespace ecommerceWebServicess.DTOs
{
    public class ProductQueryParameters
    {
        public string Name { get; set; } // Search by product name

        public string CategoryId { get; set; } // Filter by category ID

        public decimal? MinPrice { get; set; } // Minimum price filter

        public decimal? MaxPrice { get; set; } // Maximum price filter

        public string VendorId { get; set; } // Filter by vendor ID

        public bool? IsActive { get; set; } // Filter by active/inactive status

        public int PageNumber { get; set; } = 1; // The page number to display (for pagination)

        public int PageSize { get; set; } = 10; // The number of items to return per page (for pagination)
    }
}
