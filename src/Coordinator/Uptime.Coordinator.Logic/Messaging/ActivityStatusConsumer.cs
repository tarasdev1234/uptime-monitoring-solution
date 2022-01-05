using Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reliablesite.Service.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Coordinator.Model.Abstractions;
using Uptime.Coordinator.Model.Messages;
using Uptime.Coordinator.Model.Models;

namespace Uptime.Coordinator.Logic.Messaging
{
    internal sealed class ActivityStatusConsumer : ScopedConsumer<ActivityStatusChanged>
    {
        private readonly ILogger logger;
        private readonly ServiceSettings serviceSettings;

        public ActivityStatusConsumer(IServiceScopeFactory scopeFactory,
            IOptions<ServiceSettings> serviceOptions,
            ILogger<ActivityStatusConsumer> logger): base(scopeFactory)
        {
            this.logger = logger;
            this.serviceSettings = serviceOptions.Value;
        }

        protected override async Task InternalConsume(ActivityStatusChanged message, IServiceProvider scopedServiceProvider, CancellationToken token)
        {
            logger.LogDebug("Receive message ActivityStatusChanged. Monitor: {MonitorId}, executor: {ExecutorId}, status:{Status}",
                message.MonitorId, message.ExecutorId, message.Status);

            var activitiesService = scopedServiceProvider.GetRequiredService<IActivitiesService>();

            var activity = await activitiesService.GetActivityAsync(message.MonitorId, token);
            
            if (activity == null)
            {
                await activitiesService.AddActivityAsync(new Activity
                {
                    CoordinatorId = serviceSettings.InstanceId,
                    ExecutorId = message.ExecutorId,
                    MonitorId = message.MonitorId
                }, token);
            }
            else if (message.Status == ActivityStatus.Stopped)
            {
                await activitiesService.DeleteActivityAsync(message.MonitorId, token);
            }
            else if (message.Status == ActivityStatus.Started && message.ExecutorId != activity.ExecutorId)
            {
                logger.LogWarning("Activity {MonitorId} has been alredy started with other executor {ExecutorId}", message.MonitorId, message.ExecutorId);
                activity.ExecutorId = message.ExecutorId;
                await activitiesService.UpdateActivityAsync(activity, token);
            }
        }
    }
}
