using System.ComponentModel.DataAnnotations;

namespace ecommerceWebServicess.DTOs
{
    public class InventoryUpdateDto
    {


        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockThreshold { get; set; } // Stock threshold for low stock alerts



    }
}
