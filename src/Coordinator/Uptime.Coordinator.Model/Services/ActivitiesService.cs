using Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Coordinator.Data;
using Uptime.Coordinator.Model.Abstractions;
using Uptime.Coordinator.Model.Models;
using Uptime.Monitoring.Model.Messages;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Coordinator.Logic.Services
{
    internal sealed class ActivitiesService : IActivitiesService
    {
        private readonly UptimeCoordinatorContext context;
        private readonly IProducer<SetMonitorStatus> producer;
        private readonly ILogger<ActivitiesService> logger;

        public ActivitiesService(UptimeCoordinatorContext context,
            IProducer<SetMonitorStatus> producer,
            ILogger<ActivitiesService> logger)
        {
            this.context = context;
            this.producer = producer;
            this.logger = logger;
        }

        public async Task AddActivityAsync(Activity activity, CancellationToken token)
        {
            context.Activities.Add(activity);
            await context.SaveChangesAsync(token);
        }

        public async Task DeleteActivityAsync(long monitorId, CancellationToken token)
        {
            var activity = await context.Activities.FindAsync(new object[] { monitorId }, token);
            context.Activities.Remove(activity);
            await context.SaveChangesAsync(token);
        }

        public async Task<IReadOnlyList<Guid>> GetActiveExecutorsAsync(CancellationToken token)
        {
            return await context.Activities.Select(x => x.ExecutorId).Distinct().ToListAsync(token);
        }

        public async Task<IReadOnlyList<Activity>> GetActivitiesByExecutorAsync(Guid executorId, CancellationToken token)
        {
            return await context.Activities.Where(x => x.ExecutorId == executorId).ToListAsync(token);
        }

        public async Task<IReadOnlyList<Activity>> GetActivitiesByExecutorsAsync(IEnumerable<Guid> executorIds, CancellationToken token)
        {
            return await context.Activities.Where(x => executorIds.Contains(x.ExecutorId)).ToListAsync(token);
        }

        public async Task<Activity> GetActivityAsync(long monitorId, CancellationToken token)
        {
            return await context.Activities.FindAsync(new object[] { monitorId }, token);
        }

        public async Task UpdateActivityAsync(Activity activity, CancellationToken token)
        {
            context.Activities.Update(activity);
            await context.SaveChangesAsync(token);
        }

        public async Task DeleteActivitiesAsync(IEnumerable<long> monitorIds, CancellationToken token)
        {
            context.Activities.RemoveRange(context.Activities.Where(x => monitorIds.Contains(x.MonitorId)));
            await context.SaveChangesAsync(token);
        }

        public async Task CorrectLiveExecutorsAsync(IEnumerable<Guid> liveExecutorIds, CancellationToken token)
        {
            var hangedActivities = await context.Activities.Where(x => !liveExecutorIds.Contains(x.ExecutorId)).ToListAsync();
            var hangedExecutors = hangedActivities.Select(x => x.ExecutorId).Distinct().ToList();

            if (hangedActivities.Count != 0)
            {
                logger.LogInformation("Found {0} executors that no longer active: [{1}]", hangedExecutors.Count, string.Join(", ", hangedExecutors));
                logger.LogInformation("Restarting next activities: [{0}]", string.Join(", ", hangedActivities.Select(x => x.MonitorId)));

                await DeleteActivitiesAsync(hangedActivities.Select(x => x.MonitorId), token);

                foreach (var activtiy in hangedActivities)
                {
                    await producer.SendAsync(activtiy.MonitorId.ToString(),
                        new SetMonitorStatus
                    {
                        MonitorId = activtiy.MonitorId,
                        Status = MonitorStatus.Started
                    });
                }
            }
        }
    }
}
