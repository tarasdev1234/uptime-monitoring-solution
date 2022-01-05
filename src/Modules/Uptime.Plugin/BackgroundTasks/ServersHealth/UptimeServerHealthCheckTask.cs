using OrchardCore.BackgroundTasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Uptime.Plugin.BackgroundTasks.ServersHealth {
    [BackgroundTask(Schedule = "*/1 * * * *", Description = "Check servers health.")]
    public class UptimeServerHealthCheckTask : IBackgroundTask {
        public Task DoWorkAsync (IServiceProvider serviceProvider, CancellationToken cancellationToken) {
            var task = serviceProvider.GetService<UptimeServerHealthChecker>();
            return task.CheckAsync();
        }
    }
}
