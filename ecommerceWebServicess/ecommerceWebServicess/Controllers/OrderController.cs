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

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        // Create a new order
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

        // Get order by ID
        [HttpGet("{id}")]
        [Authorize(Roles = "CSR, Administrator, Customer")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }


        // Get all orders (For CSR/Admin)
        [HttpGet]
        [Authorize(Roles = "CSR,Administrator")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }


        // Get orders by User ID (For customers)
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        // Get orders for a vendor
        [HttpGet("vendor/{vendorId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> GetVendorOrders(string vendorId)
        {
            var orders = await _orderService.GetVendorOrdersAsync(vendorId);
            return Ok(orders);
        }


        // Update order (For CSR/Admin)
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


        // Cancel order (CSR/Admin)
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "CSR,Administrator")]
        public async Task<IActionResult> CancelOrder(string id, [FromBody] string cancellationNote)
        {
            var result = await _orderService.CancelOrderAsync(id, cancellationNote);
            if (!result) return NotFound();

            return NoContent();
        }


        // Mark order as delivered (For CSR/Admin/Vendor)
        [HttpPut("{id}/mark-delivered")]
        [Authorize(Roles = "CSR,Administrator,Vendor")]
        public async Task<IActionResult> MarkOrderAsDelivered(string id)
        {
            var result = await _orderService.MarkOrderAsDeliveredAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }



        // Mark product as ready by vendor
        [HttpPut("{orderId}/product/{productId}/mark-ready")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> MarkProductAsReady(string orderId, string productId)
        {
            var vendorId = User.FindFirst("nameid")?.Value;
            if (vendorId == null)
            {
                return Unauthorized("Vendor not authenticated.");
            }

            var result = await _orderService.MarkProductAsReadyByVendorAsync(orderId, productId, vendorId);
            if (!result) return NotFound();

            return NoContent();
        }







    }
}
