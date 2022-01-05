using Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Messages;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Logic.Messaging
{
    internal sealed class SetMonitorStatusConsumer : ScopedConsumer<SetMonitorStatus>
    {
        public SetMonitorStatusConsumer(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
        }

        protected override Task InternalConsume(SetMonitorStatus message, IServiceProvider scopedServiceProvider, CancellationToken token)
        {
            var taskService = scopedServiceProvider
                .GetRequiredService<IMonitoringTaskService>();

            switch(message.Status)
            {
                case MonitorStatus.Started:
                    return taskService.StartAsync(message.MonitorId);

                case MonitorStatus.Stopped:
                    return taskService.StopAsync(message.MonitorId);

                default:
                    throw new ArgumentException($"Unexpected monitor status: {message.Status}. Can't set this status.");
            }
        }
    }
}
