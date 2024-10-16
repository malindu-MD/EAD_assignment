namespace ecommerceWebServicess.DTOs
{
    public class OrderCancelRequestDto
    {

        public string OrderId { get; set; }   // The ID of the order being cancelled
        public string Note { get; set; }       // Reason for cancelling the order

    }
}
