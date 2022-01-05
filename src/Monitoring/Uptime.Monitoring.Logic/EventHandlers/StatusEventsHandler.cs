using Messaging;
using Microsoft.Extensions.Options;
using Reliablesite.Service.Model;
using System.Threading.Tasks;
using Uptime.Coordinator.Model.Messages;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Logic.EventHandlers
{
    internal sealed class StatusEventsHandler : ITaskEventsHandler {
        private readonly IEventsService events;
        private readonly IProducer<ActivityStatusChanged> statusChangedProducer;
        private readonly ServiceSettings serviceSettings;

        public StatusEventsHandler (IEventsService events, IProducer<ActivityStatusChanged> statusChangedProducer, IOptions<ServiceSettings> serviceOptions) {
            this.events = events;
            this.statusChangedProducer = statusChangedProducer;
            this.serviceSettings = serviceOptions.Value;
        }

        /// <summary>
        /// On DOWN or UP event we need to update the monitor status.
        /// No need to insert summary events - task handlers will do it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reportError"></param>
        /// <returns></returns>
        public Task Down(MonitorTask task, MonitoringEvent @event, bool eventRepeated) => Task.CompletedTask;

        /// <summary>
        /// On DOWN or UP event we need to update the monitor status.
        /// No need to insert summary events - task handlers will do it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reportError"></param>
        /// <returns></returns>
        public Task Up(MonitorTask task, MonitoringEvent @event, bool eventRepeated) => Task.CompletedTask;

        public async Task Pause (Monitor monitor) {
            await InsertSummary(monitor, EventType.Paused);
        }

        public async Task Start (Monitor monitor) {
            await InsertSummary(monitor, EventType.Started);
            await statusChangedProducer.SendAsync(new ActivityStatusChanged
            {
                ExecutorId = serviceSettings.InstanceId,
                MonitorId = monitor.Id,
                Status = ActivityStatus.Started
            });
        }

        public async Task Stop (Monitor monitor) {
            await InsertSummary(monitor, EventType.Stopped);
            await statusChangedProducer.SendAsync(new ActivityStatusChanged
            {
                ExecutorId = serviceSettings.InstanceId,
                MonitorId = monitor.Id,
                Status = ActivityStatus.Stopped
            });
        }

        private async Task InsertSummary(Monitor monitor, EventType evt) {
            await events.AddEventAsync(new MonitoringEvent {
                Type = evt,
                MonitorId = monitor.Id,
                UserId = monitor.UserId,
            });
        }
    }
}
