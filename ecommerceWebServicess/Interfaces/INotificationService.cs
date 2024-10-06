using ecommerceWebServicess.Models;

namespace ecommerceWebServicess.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string message,string productId);

        Task SendNotificationAsync(string userId, string message);

        Task<IEnumerable<Notification>> GetNotificationByUserID(string userId);

    }
}
