using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reliablesite.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Monitoring.Data;
using Uptime.Monitoring.Logic.Events;
using Uptime.Monitoring.Logic.MonitoringTasks;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Models;
using Uptime.Schedule;

namespace Uptime.Monitoring.Logic.Services
{
    internal sealed class MonitoringTaskService : IMonitoringTaskService
    {
        private readonly ILogger<MonitoringTaskService> logger;

        private readonly MonitoringTaskFactory taskFactory;

        private readonly EventManager eventManager;

        private readonly Scheduler<long> tasksScheduler;

        private readonly UptimeMonitoringContext dbContext;

        private readonly IServiceScopeFactory scopeFactory;

        private readonly ServiceSettings serviceSettings;

        public MonitoringTaskService(
            MonitoringTaskFactory taskFactory,
            EventManager eventManager,
            UptimeMonitoringContext dbContext,
            IServiceScopeFactory scopeFactory,
            Scheduler<long> tasksScheduler,
            IOptions<ServiceSettings> serviceOptions,
            ILogger<MonitoringTaskService> logger)
        {
            this.logger = logger;
            this.taskFactory = taskFactory;
            this.eventManager = eventManager;
            this.tasksScheduler = tasksScheduler;
            this.dbContext = dbContext;
            this.scopeFactory = scopeFactory;
            serviceSettings = serviceOptions.Value;
        }

        public void StartOneTimeTask(MonitorTask monitor)
        {
            EnsureArg.IsNotNull(monitor);

            logger.LogInformation($"Starting one-time task '{monitor.MonitorId}'");

            var task = taskFactory.BuildTaskAsync(monitor);

            tasksScheduler.StartOnce(task);
        }

        public async Task StartAsync(long taskId)
        {
            logger.LogInformation($"Starting task '{taskId}'");

            if (IsRunning(taskId))
            {
                logger.LogInformation($"Task '{taskId}' has been started already");
                return;
            }

            var monitor = await dbContext.Monitors.FindAsync(taskId);

            if (monitor == null)
            {
                throw new Exception($"Unable to create task: monitor with id {taskId} not found.");
            }

            logger.LogDebug($"Task '{taskId}' not started yet. Trying to build one.");

            var task = taskFactory.BuildTaskAsync(monitor)
                .OnComplete(async (task) =>
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<UptimeMonitoringContext>();
                        var eventManager = scope.ServiceProvider.GetRequiredService<EventManager>();
                        await UpdateStatus(dbContext, eventManager, serviceSettings.InstanceId, task.Id, MonitorStatus.Stopped);
                    }
                });

            logger.LogDebug($"Task '{taskId}' was built with type: '{task.GetType().Name}' and name: '{task.Name}'");

            await UpdateStatus(taskId, MonitorStatus.Started);
            tasksScheduler.Schedule(task, TimeSpan.FromSeconds(monitor.Interval), monitor.Repeat);

            logger.LogInformation($"Task '{taskId}' successfully started with type: '{task.GetType().Name}' and name: '{task.Name}'");
        }

        public Task StopAsync(long taskId)
        {
            tasksScheduler.TryStop(taskId);
            return Task.CompletedTask;
        }

        public Task PauseAsync(long taskId)
        {
            throw new NotImplementedException();
            // await UpdateStatus(taskId, MonitorStatus.Paused);
        }

        public HashSet<long> GetRunningIds() => tasksScheduler.GetTasksIds().ToHashSet();

        public bool IsRunning(long taskId) => tasksScheduler.IsScheduled(taskId);

        private Task UpdateStatus(long monitorId, MonitorStatus status) => UpdateStatus(dbContext, eventManager, serviceSettings.InstanceId, monitorId, status);

        private static async Task UpdateStatus(UptimeMonitoringContext dbContext,
            EventManager eventManager,
            Guid serviceId,
            long monitorId,
            MonitorStatus status)
        {
            var monitor = dbContext.Monitors.Find(monitorId);
            monitor.Status = status;
            monitor.LastExecutor = serviceId;
            await dbContext.SaveChangesAsync();

            await eventManager.FireStatusUpdate(monitor, status);
        }
    }
}
