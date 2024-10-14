/***************************************************************************
 * File: NotificationController.cs
 * Description: Provides API endpoints for managing notifications by user ID.
 ***************************************************************************/

using System.Security.Claims;
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

        // Constructor to inject the notification service
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notification/{userId}
        // Fetches notifications for a specific user
        [HttpGet]
        [Authorize(Roles = "CSR, Administrator, Customer, Vendor")]
        public async Task<IActionResult> GetNotificationsByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("user not authenticated.");
            }

            // Get notifications for the given user ID
            IEnumerable<Notification> notifications = await _notificationService.GetNotificationByUserID(userId);

            if (notifications == null)
            {
                return NotFound($"No notifications found for user ID: {userId}");
            }

            return Ok(notifications);
        }

        // put api/notification/{notificationId}/read
        [HttpPut("{notificationId}/read")]
        [Authorize(Roles = "CSR, Administrator, Customer, Vendor")]
        public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
        {
            // Call the service to mark the notification as read
            var result = await _notificationService.MarkNotificationAsReadAsync(notificationId);

            if (result)
            {
                return Ok(new { message = "Notification marked as read successfully." });
            }
            else
            {
                return NotFound(new { message = "Notification not found." });
            }
        }
    }
}
