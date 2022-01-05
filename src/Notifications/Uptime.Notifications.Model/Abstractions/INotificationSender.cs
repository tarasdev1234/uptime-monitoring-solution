using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Model.Abstractions
{
    public interface INotificationSender
    {
        Task SendAsync(Notification notification, string renderedNotification, CancellationToken token);
    }
}
