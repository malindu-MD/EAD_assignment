namespace ecommerceWebServicess.DTOs
{
    public class CreateProductDto
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryId { get; set; }

        public double Price { get; set; }

        public int Stock { get; set; }

        public int StockThreshold { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;


    }
}
