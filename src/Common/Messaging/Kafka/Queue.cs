using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using EnsureThat;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka
{
    internal sealed class QueueSettings
    {
        public string Topic { get; set; }
        public string? ConsumerGroup { get; set; }
        public string Hosts { get; set; }

        public QueueSettings(string hosts, string topic)
        {
            Topic = EnsureArg.IsNotNullOrEmpty(topic, nameof(topic));
            Hosts = EnsureArg.IsNotNullOrEmpty(hosts, nameof(hosts));
        }
    }

    internal sealed class Queue<T> : IQueue<T>
    {
        private const Partitioner DefaultPartitioner = Partitioner.Murmur2Random;

        private readonly QueueSettings settings;
        private readonly ISerializer<T> serializer;
        private readonly IDeserializer<T> deserializer;
        private readonly ILogger logger;

        private List<IConsumer<T>> consumers = new List<IConsumer<T>>();
        private Task pollingLoop = Task.CompletedTask;

        public Queue(QueueSettings settings, ISerializer<T> serializer, IDeserializer<T> deserializer, ILogger logger)
        {
            this.settings = EnsureArg.IsNotNull(settings, nameof(settings));
            this.serializer = EnsureArg.IsNotNull(serializer, nameof(serializer));
            this.deserializer = EnsureArg.IsNotNull(deserializer, nameof(deserializer));
            this.logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public IProducer<T> GetProducer()
        {
            return new Producer<T>(settings.Topic, new ProducerConfig { BootstrapServers = settings.Hosts, Partitioner = DefaultPartitioner }, serializer);
        }

        public void Subscribe(IConsumer<T> consumer)
        {
            consumers.Add(consumer);
        }

        public void StartListening(CancellationToken token)
        {
            if (!pollingLoop.IsCompleted)
            {
                throw new Exception("Trying to start queue listening multiple times");
            }

            if (consumers.Count == 0)
            {
                pollingLoop = Task.CompletedTask;
            }
            else
            {
                pollingLoop = Task.Run(() => StartPolling(token), token);
            }
        }

        public Task WaitFinishAsync()
        {
            return pollingLoop;
        }

        private async void StartPolling(CancellationToken token)
        {
            logger.LogInformation("Starting kafka consumer on topic '{KafkaTopic}'", settings.Topic);
            var config = new ConsumerConfig
            {
                BootstrapServers = settings.Hosts,
                GroupId = settings.ConsumerGroup ?? Guid.NewGuid().ToString()
            };

            using var consumer = new ConsumerBuilder<string, T>(config)
                .SetValueDeserializer(deserializer)
                .Build();

            consumer.Subscribe(settings.Topic);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var message = consumer.Consume(token);
                    logger.LogDebug("Accepted message from kafka topic {KafkaTopic}", settings.Topic);
                    foreach (var handler in consumers)
                    {
                        await handler.ConsumeAsync(message.Message.Value, token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Do nothing. We will quit in next while iteration
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error on consuming message from kafka on topic {KafkaTopic}", settings.Topic);
                }
            }
            logger.LogInformation("Kafka consumer on topic '{KafkaTopic}' has been stoped", settings.Topic);
            consumer.Close();
        }
    }
}
