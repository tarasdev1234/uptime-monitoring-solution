using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Plugin.BackgroundTasks.ServersHealth;

namespace Uptime.Plugin.HostedService {
    public class UptimeServerHealthCheckHostedService : IHostedService {
        private readonly ILogger<UptimeServerHealthCheckHostedService> logger;
        private readonly IServiceProvider serviceProvider;
        private CancellationTokenSource cancellationTokenSource;

        private Task executingTask;

        public UptimeServerHealthCheckHostedService (ILogger<UptimeServerHealthCheckHostedService> logger, IServiceProvider serviceProvider) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync (CancellationToken cancellationToken) {
            executingTask = ExecuteAsync(cancellationTokenSource.Token);

            if (executingTask.IsCompleted) {
                return executingTask;
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync (CancellationToken cancelToken) {
            cancellationTokenSource.Cancel();

            await Task.WhenAny(executingTask, Task.Delay(Timeout.Infinite, cancelToken));
        }

        private async Task ExecuteAsync (CancellationToken cancellationToken) {
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            while (!cancellationToken.IsCancellationRequested) {
                logger.LogDebug("Executing Server HealthCheck HostedService.");

                using (var scope = scopeFactory.CreateScope()) {
                    try {
                        var checker = scope.ServiceProvider.GetRequiredService<IUptimeServerHealthChecker>();
                        await checker.CheckAsync(cancellationToken);

                        logger.LogDebug("HealthCheck HostedService executed successfully.");
                    } catch (Exception ex) {
                        logger.LogError(ex, $"HealthCheck HostedService throw a error: {ex.Message}");
                    }
                }

                await Task.Delay(10 * 1000, cancellationToken);
            }
        }
    }
}
