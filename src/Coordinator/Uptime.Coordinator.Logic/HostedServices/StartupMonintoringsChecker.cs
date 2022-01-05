using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Coordinator.Model;
using Uptime.Coordinator.Model.Abstractions;

namespace Uptime.Coordinator.Logic.HostedServices
{
    internal sealed class StartupMonintoringsChecker : IHostedService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IConsulClient consulClient;
        private readonly ILogger<StartupMonintoringsChecker> logger;

        public StartupMonintoringsChecker(
            IServiceScopeFactory scopeFactory,
            IConsulClient consulClient,
            ILogger<StartupMonintoringsChecker> logger)
        {
            this.scopeFactory = scopeFactory;
            this.consulClient = consulClient;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var activitiesService = scope.ServiceProvider.GetRequiredService<IActivitiesService>();

            logger.LogInformation("Obtaining information about active monitoring services");
            var liveMonitorServices = (await consulClient.Health.Service("monitoring", null, true, cancellationToken)).Response;
            logger.LogInformation("{0} monitoring services are active", liveMonitorServices.Length);

            await activitiesService.CorrectLiveExecutorsAsync(liveMonitorServices.Select(x => ConsulHelpers.GetInstanceId(x.Service.ID)).ToList(), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
