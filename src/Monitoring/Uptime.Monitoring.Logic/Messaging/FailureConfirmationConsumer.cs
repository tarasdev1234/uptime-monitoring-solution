using System;
using System.Threading;
using System.Threading.Tasks;
using Messaging;
using Microsoft.Extensions.DependencyInjection;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Messages;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Logic.Messaging
{
    internal sealed class FailureConfirmationConsumer : ScopedConsumer<FailureConfirmation>
    {
        public FailureConfirmationConsumer(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
        }

        protected override Task InternalConsume(FailureConfirmation message, IServiceProvider scopedServiceProvider, CancellationToken token)
        {
            MonitorTask task = message switch
            {
                PingFailureConfirmation _ => new PingMonitorTask { Target = message.Target },
                HttpFailureConfirmation _ => new HttpMonitorTask { Target = message.Target },
                KeywordFailureConfirmation c =>
                    new KeywordMonitorTask { Target = c.Target, Keyword = c.Keyword, ShouldContain = c.ShouldContain },
                _ => throw new Exception($"Unknown task type for failure confirmation: '{message.Type}'")
            };

            task.MonitorId = message.MonitorId;
            task.UserId = message.UserId;
            task.EventIdToConfirm = message.OriginEventId;
            task.Name = $"Confirmation_{message.OriginEventId}";

            token.ThrowIfCancellationRequested();
            scopedServiceProvider
                    .GetRequiredService<IMonitoringTaskService>()
                    .StartOneTimeTask(task);

            return Task.CompletedTask;
        }
    }
}
