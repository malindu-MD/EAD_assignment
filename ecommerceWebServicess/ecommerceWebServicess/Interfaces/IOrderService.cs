using ecommerceWebServicess.DTOs;

namespace ecommerceWebServicess.Interfaces
{
    public interface IOrderService
    {

        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderAsync(UpdateOrderDto updateOrderDto);
        Task<bool> CancelOrderAsync(string orderId, string cancellationNote);
        Task<bool> MarkOrderAsDeliveredAsync(string orderId);
        Task<bool> MarkProductAsReadyByVendorAsync(string orderId, string productId, string vendorId);
        Task<IEnumerable<OrderDto>> GetVendorOrdersAsync(string vendorId);

    }
}
