using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Model.Abstractions
{
    public interface INotificationSenderProvider
    {
        public INotificationSender GetNotificationSender(DeliveryType deliveryType);
    }
}
