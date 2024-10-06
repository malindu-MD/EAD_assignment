namespace ecommerceWebServicess.DTOs
{
    public class ProductDto
    {
        public string Id { get; set; } // MongoDB ObjectId as string

        public string ProductId { get; set; } // Unique Product ID

        public string VendorId { get; set; } // Reference to Vendors collection

        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryId { get; set; } // Reference to Categories collection

        public double Price { get; set; }

        public int Stock { get; set; }

        public int StockThreshold { get; set; } // Stock threshold for low stock alerts

        public bool IsActive { get; set; }

        public string ImageUrl { get; set; } // URL to the product image


        public System.DateTime DateCreated { get; set; }

        public System.DateTime DateModified { get; set; }


    }
}
