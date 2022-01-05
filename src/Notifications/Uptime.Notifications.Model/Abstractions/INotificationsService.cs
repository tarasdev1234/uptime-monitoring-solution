using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Model.Abstractions
{
    public interface INotificationsService
    {
        Task NotifyAsync(Notification notification, CancellationToken token);
    }
}
