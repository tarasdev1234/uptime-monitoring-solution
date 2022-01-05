using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reliablesite.Service.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Coordinator.Client;
using Uptime.Monitoring.Model.Abstractions;

namespace Uptime.Monitoring.Logic.HostedServices
{
    internal sealed class AssignedTaskRunner : IHostedService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger logger;
        private readonly ServiceSettings serviceSettings;

        public AssignedTaskRunner(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<ServiceSettings> serviceOptions,
            ILogger<AssignedTaskRunner> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            this.serviceSettings = serviceOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Looking for tasks already assigned to this service");

            using var scope = serviceScopeFactory.CreateScope();
            var activitiesClient = scope.ServiceProvider.GetRequiredService<IActivitiesClient>();
            var monitoringTaskService = scope.ServiceProvider.GetRequiredService<IMonitoringTaskService>();

            ICollection<Activity> activities = Array.Empty<Activity>();

            try
            {
                activities = await activitiesClient.GetActivitiesAsync(serviceSettings.InstanceId, cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogWarning(ex, "Can't get assigned tasks");
            }

            logger.LogInformation("Found {TasksCount} tasks assigned to this service", activities.Count);
            foreach (var activity in activities)
            {
                await monitoringTaskService.StartAsync(activity.MonitorId);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
