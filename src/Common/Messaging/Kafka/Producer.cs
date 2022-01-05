using Confluent.Kafka;
using EnsureThat;
using System;
using System.Threading.Tasks;

namespace Messaging.Kafka
{
    internal sealed class Producer<T> : IProducer<T>, IDisposable
    {
        private static readonly TimeSpan FlushTimeout = TimeSpan.FromMilliseconds(100);

        private readonly string topic;
        private readonly IProducer<string, T> innerProducer;

        public Producer(string topic, ProducerConfig config, ISerializer<T> serializer)
        {
            this.topic = EnsureArg.IsNotNullOrEmpty(topic, nameof(topic));

            innerProducer = new ProducerBuilder<string, T>(config)
                .SetValueSerializer(serializer)
                .Build();
        }

        public async Task SendAsync(T data)
        {
            var message = new Message<string, T>
            {
                Value = data
            };

            await innerProducer.ProduceAsync(topic, message).ConfigureAwait(false);
        }

        public async Task SendAsync(string key, T data)
        {
            var message = new Message<string, T>
            {
                Key = key,
                Value = data
            };

            await innerProducer.ProduceAsync(topic, message).ConfigureAwait(false);
        }

        public void Dispose()
        {
            innerProducer.Flush(FlushTimeout);
            innerProducer?.Dispose();
        }
    }
}
