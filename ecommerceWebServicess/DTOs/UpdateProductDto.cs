namespace ecommerceWebServicess.DTOs
{
    public class UpdateProductDto
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryId { get; set; }

        public double Price { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
