using ecommerceWebServicess.Interfaces;
using ecommerceWebServicess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWebServicess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;


        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        // GET: api/Notification/{userId}
        [HttpGet("{userId}")]
        [Authorize(Roles = "CSR, Administrator, Customer,Vendor")]
        public async Task<IActionResult> GetNotificationsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            // Get notifications for the given user ID
            IEnumerable<Notification> notifications = await _notificationService.GetNotificationByUserID(userId);

            if (notifications == null)
            {
                return NotFound($"No notifications found for user ID: {userId}");
            }

            return Ok(notifications);
        }

    }
}
