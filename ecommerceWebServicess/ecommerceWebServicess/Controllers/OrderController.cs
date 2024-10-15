/***************************************************************************
 * File: OrderController.cs
 * Description: Controller for handling order-related operations including 
 *              creation, fetching, updating, cancellation, and vendor-specific
 *              actions on orders.
 ***************************************************************************/

using System.Security.Claims;
using ecommerceWebServicess.DTOs;
using ecommerceWebServicess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWebServicess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        // Constructor to inject the order service
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: Create a new order (Customer only)
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        // GET: Fetch order by ID
        [HttpGet("{id}")]
        [Authorize(Roles = "CSR, Administrator, Customer")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        // GET: Fetch all orders (CSR/Admin only)
        [HttpGet]
        [Authorize(Roles = "CSR,Administrator")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // GET: Fetch orders by user ID (Customer only)
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        // GET: Fetch vendor-specific orders (Vendor only)
        [HttpGet("vendor")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> GetVendorOrders()
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }
            var orders = await _orderService.GetVendorOrdersAsync(vendorId);
            return Ok(orders);
        }

        // PUT: Update an order (CSR/Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "CSR,Administrator")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            if (id != updateOrderDto.Id) return BadRequest("Order ID mismatch");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.UpdateOrderAsync(updateOrderDto);
            if (!result) return NotFound();
            return NoContent();
        }

        // PUT: Cancel an order (CSR/Admin only)
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "CSR,Administrator")]
        public async Task<IActionResult> CancelOrder(string id, [FromBody] CancellationRequest cancellationRequest )
        {
            if (cancellationRequest == null || string.IsNullOrEmpty(cancellationRequest.CancellationNote))
            {
                return BadRequest("Cancellation note is required.");
            }
            var result = await _orderService.CancelOrderAsync(id, cancellationRequest.CancellationNote);
            if (!result) return NotFound();
            return NoContent();
        }

        // PUT: Mark order as delivered (CSR/Admin/Vendor)
        [HttpPut("{id}/mark-delivered")]
        [Authorize(Roles = "CSR,Administrator,Vendor")]
        public async Task<IActionResult> MarkOrderAsDelivered(string id)
        {
            var result = await _orderService.MarkOrderAsDeliveredAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // PUT: Mark a product as ready by the vendor
        [HttpPut("{orderId}/product/{productId}/ordritem-delivered")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> MarkProductAsReady(string orderId, string productId)
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (vendorId == null)
            {

                return Unauthorized("Vendor not authenticated.");
            }


            var result = await _orderService.MarkProductAsReadyByVendorAsync(orderId, productId, vendorId);
            if (!result) return NotFound();
            return NoContent();
        }

        // GET: Fetch vendor-specific order items
        [HttpGet("{orderId}/vendor/items")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> GetOrderItemsByOrderIdAndVendorId(string orderId)
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }
            var orderItems = await _orderService.GetOrderItemsByOrderIdAndVendorIdAsync(orderId, vendorId);
            if (!orderItems.Any()) return NotFound("No items found for this vendor in the order.");
            return Ok(orderItems);
        }

        // GET: Fetch all order items for an order
        [HttpGet("{orderId}/items")]
        [Authorize(Roles = "CSR,Administrator")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItemsByOrderId(string orderId)
        {
            var orderItems = await _orderService.GetOrderItemsByOrderIdAsync(orderId);
            if (orderItems == null || !orderItems.Any())
            {
                return NotFound("No order items found for this order ID.");
            }
            return Ok(orderItems);
        }

        [HttpPut("{orderId}/mark-item-shipped/{productId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> MarkProductAsship (string orderId, string productId)
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (vendorId == null)
            {   
                
                return Unauthorized("Vendor not authenticated.");
            }

            var result = await _orderService.MarkProductAsShippedByVendorAsync(orderId, productId, vendorId);
            if (!result) return NotFound();
            return NoContent();


          
        }




    }
}
