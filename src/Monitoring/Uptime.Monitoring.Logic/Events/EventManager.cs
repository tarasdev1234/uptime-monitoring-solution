using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Uptime.Extensions;
using Uptime.Monitoring.Model.Models;
using Uptime.Monitoring.Model.Abstractions;

namespace Uptime.Monitoring.Logic.Events
{
    public class EventManager {
        private readonly ILogger<EventManager> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public EventManager(
            ILogger<EventManager> logger,
            IServiceScopeFactory serviceScopeFactory
        ) {
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            this.logger = logger;
        }

        public async Task FireHealthCheckFinished(MonitorTask task, MonitoringEvent @event, bool eventRepeated) {
            if (@event.Type == EventType.Up) {
                await ForEachHandler(h => h.Up(task, @event, eventRepeated));
            } else if (@event.Type == EventType.Down) {
                await ForEachHandler(h => h.Down(task, @event, eventRepeated));
            }
        }

        public Task FireStatusUpdate(Monitor monitor, MonitorStatus status)
            => status switch
            {
                MonitorStatus.Stopped => ForEachHandler(h => h.Stop(monitor)),
                MonitorStatus.Started => ForEachHandler(h => h.Start(monitor)),
                MonitorStatus.Paused => ForEachHandler(h => h.Pause(monitor)),
                _ => throw new Exception($"Unexpected monitor status {status}")
            };

        public void LogError (string key, string message) {
            logger.LogError($"Error while processing background monitor event: {key}: {message}");
        }

        private async Task ForEachHandler(Func<ITaskEventsHandler, Task> action)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var eventHandlers = scope.ServiceProvider.GetServices<ITaskEventsHandler>();

            foreach (var handler in eventHandlers)
            {
                try
                {
                    await action(handler);
                }
                catch (Exception ex) when (!ex.IsFatal())
                {
                    logger.LogError(ex, "{HandlerType} thrown an exception", handler.GetType().Name);
                }
            }
        }
    }
}
