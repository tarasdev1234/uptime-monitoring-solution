using EnsureThat;
using Messaging;
using System;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Messages;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Logic.EventHandlers
{
    internal sealed class FailureConfirmationEventHandler : ITaskEventsHandler
    {
        private readonly IProducer<FailureConfirmation> producer;

        public FailureConfirmationEventHandler(IProducer<FailureConfirmation> producer)
        {
            this.producer = EnsureArg.IsNotNull(producer);
        }

        public Task Down(MonitorTask task, MonitoringEvent @event, bool eventRepeated)
        {
            if (!task.IsConfirmation)
            {
                FailureConfirmation message = task switch
                {
                    KeywordMonitorTask t => new KeywordFailureConfirmation { ShouldContain = t.ShouldContain, Keyword = t.Keyword },
                    HttpMonitorTask _ => new HttpFailureConfirmation(),
                    PingMonitorTask _ => new PingFailureConfirmation(),
                    _ => throw new Exception($"Unknown monitor task type {task.GetType().FullName}")
                };

                message.MonitorId = task.MonitorId;
                message.Target = task.Target;
                message.OriginEventId = @event.Id;
                message.UserId = task.UserId;

                return producer.SendAsync(message);
            }

            return Task.CompletedTask;
        }

        public Task Up(MonitorTask task, MonitoringEvent @event, bool eventRepeated) => Task.CompletedTask;

        public Task Pause(Monitor monitor) => Task.CompletedTask;

        public Task Start(Monitor monitor) => Task.CompletedTask;

        public Task Stop(Monitor monitor) => Task.CompletedTask;
    }
}
