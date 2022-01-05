using System.Threading;
using System.Threading.Tasks;

namespace Uptime.Plugin.BackgroundTasks.ServersHealth {
    interface IUptimeServerHealthChecker {
        Task CheckAsync (CancellationToken cancellationToken);
    }
}
