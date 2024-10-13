/***************************************************************************
 * File: IOrderService.cs
 * Description: Interface for order services. Defines methods for order 
 *              management including creating, updating, and retrieving orders.
 ***************************************************************************/

using ecommerceWebServicess.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerceWebServicess.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="createOrderDto">Details for creating the order.</param>
        /// <returns>The created order DTO.</returns>
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);

        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>The order DTO.</returns>
        Task<OrderDto> GetOrderByIdAsync(string orderId);

        /// <summary>
        /// Retrieves all orders placed by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of order DTOs.</returns>
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of all order DTOs.</returns>
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        /// <summary>
        /// Updates an order.
        /// </summary>
        /// <param name="updateOrderDto">Details for updating the order.</param>
        /// <returns>True if the update was successful.</returns>
        Task<bool> UpdateOrderAsync(UpdateOrderDto updateOrderDto);

        /// <summary>
        /// Cancels an order and adds a cancellation note.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="cancellationNote">A note explaining the cancellation.</param>
        /// <returns>True if the cancellation was successful.</returns>
        Task<bool> CancelOrderAsync(string orderId, string cancellationNote);

        /// <summary>
        /// Marks an order as delivered.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>True if the order was marked as delivered successfully.</returns>
        Task<bool> MarkOrderAsDeliveredAsync(string orderId);

        /// <summary>
        /// Marks a product in an order as ready by the vendor.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>True if the product was marked as ready successfully.</returns>
        Task<bool> MarkProductAsReadyByVendorAsync(string orderId, string productId, string vendorId);

        /// <summary>
        /// Retrieves all orders for a specific vendor.
        /// </summary>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>A list of vendor order DTOs.</returns>
        Task<IEnumerable<OrderDto>> GetVendorOrdersAsync(string vendorId);

        /// <summary>
        /// Retrieves order items for a specific vendor in a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>A list of order item DTOs for the vendor.</returns>
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAndVendorIdAsync(string orderId, string vendorId);

        /// <summary>
        /// Retrieves all order items for a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>A list of order item DTOs.</returns>
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(string orderId);




        Task<bool> MarkProductAsShippedByVendorAsync(string orderId, string productId, string vendorId);
    }
}
