using EnsureThat;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Kafka
{
    internal sealed class MessageQueueBackgroundService : BackgroundService
    {
        private readonly IEnumerable<IQueue> messageQueues;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly ILogger logger;

        public MessageQueueBackgroundService(
            IEnumerable<IQueue> messageQueues,
            IHostApplicationLifetime applicationLifetime,
            ILogger<MessageQueueBackgroundService> logger)
        {
            this.messageQueues = Ensure.Enumerable.HasAny(messageQueues, x => x != null, nameof(messageQueues));
            this.logger = logger;
            this.applicationLifetime = applicationLifetime;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting message queue listeners");

            try
            {
                foreach (var queue in messageQueues)
                {
                    queue.StartListening(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Can't start queue listeners");
                applicationLifetime.StopApplication();
            }

            logger.LogInformation("All message queue listeners started succesfully");

            return Task.Factory
                .ContinueWhenAll(
                    messageQueues.Select(x => x.WaitFinishAsync()).ToArray(),
                    (_) => Task.CompletedTask);
        }
    }
}
