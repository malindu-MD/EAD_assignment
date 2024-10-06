using System.ComponentModel.DataAnnotations;

namespace ecommerceWebServicess.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }
}
