/***************************************************************************
 * File: INotificationService.cs
 * Description: Interface for notification services. Defines methods for 
 *              sending notifications and retrieving notifications by user ID.
 ***************************************************************************/

using ecommerceWebServicess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerceWebServicess.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Sends a notification to a user related to a specific product.
        /// </summary>
        /// <param name="userId">The ID of the user to send the notification to.</param>
        /// <param name="message">The notification message.</param>
        /// <param name="productId">The ID of the related product.</param>
        Task SendNotificationAsync(string userId, string message, string productId);

        /// <summary>
        /// Sends a general notification to a user.
        /// </summary>
        /// <param name="userId">The ID of the user to send the notification to.</param>
        /// <param name="message">The notification message.</param>
        Task SendNotificationAsync(string userId, string message);

        /// <summary>
        /// Retrieves all notifications for a specific user by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of Notification objects.</returns>
        Task<IEnumerable<Notification>> GetNotificationByUserID(string userId);
    }
}
