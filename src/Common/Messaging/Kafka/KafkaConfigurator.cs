using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Messaging.Kafka
{
    internal class KafkaConfigurator : IMessagingConfigurator
    {
        protected readonly IServiceCollection serviceCollection;

        public KafkaConfigurator(IServiceCollection serviceCollection)
        {
            this.serviceCollection = EnsureArg.IsNotNull(serviceCollection, nameof(serviceCollection));
        }

        public virtual IQueueConfigurator<T> Queue<T>(string queueName)
        {
            return new KafkaConfigurator<T>(queueName, serviceCollection);
        }
    }

    internal sealed class KafkaConfigurator<T> : KafkaConfigurator, IQueueConfigurator<T>
    {
        private readonly HashSet<string> registeredQueueNames;
        private readonly HashSet<Type> registeredMessageTypes;
        private readonly List<Func<IServiceProvider, IConsumer<T>>> registeredConsumers =
            new List<Func<IServiceProvider, IConsumer<T>>>();

        public KafkaConfigurator(string queueName, IServiceCollection serviceCollection) :
            this(queueName, new HashSet<string>(), new HashSet<Type>(), serviceCollection)
        {
        }

        private KafkaConfigurator(string queueName, HashSet<string> registeredQueueNames, HashSet<Type> registeredMessageTypes, IServiceCollection serviceCollection) :
            base(serviceCollection)
        {
            EnsureArg.IsNotNullOrEmpty(queueName, nameof(queueName));
            this.registeredQueueNames = registeredQueueNames;
            this.registeredMessageTypes = registeredMessageTypes;

            if (!registeredQueueNames.Add(queueName))
            {
                throw new Exception($"Trying to register multiple consumers on same queue '{queueName}'");
            }

            if (!registeredMessageTypes.Add(typeof(T)))
            {
                throw new Exception($"Trying to register multiple consumers with same message type '{typeof(T).FullName}'");
            }

            serviceCollection.AddSingleton<IQueue<T>>(sp =>
            {
                var queue = CreateQueue(sp, queueName);
                foreach (var consumerFactory in registeredConsumers)
                {
                    queue.Subscribe(consumerFactory(sp));
                }
                return queue;
            })
                .AddSingleton<IProducer<T>>(sp => sp.GetService<IQueue<T>>().GetProducer())
                .AddSingleton<IQueue>(sp => sp.GetService<IQueue<T>>());
        }

        public IQueueConfigurator<T> WithListener(Func<IServiceProvider, IConsumer<T>> consumerFactory)
        {
            EnsureArg.IsNotNull(consumerFactory, nameof(consumerFactory));

            registeredConsumers.Add(consumerFactory);
            return this;
        }

        public IQueueConfigurator<T> WithListener<TConsumer>() where TConsumer : IConsumer<T>
        {
            registeredConsumers.Add(sp => ActivatorUtilities.CreateInstance<TConsumer>(sp));
            return this;
        }

        public override IQueueConfigurator<TMessageType> Queue<TMessageType>(string queueName)
        {
            return new KafkaConfigurator<TMessageType>(queueName, registeredQueueNames, registeredMessageTypes, serviceCollection);
        }

        private static Queue<T> CreateQueue(IServiceProvider sp, string queueName)
        {
            var serializer = new JsonSerializer<T>();
            return new Queue<T>(GetQueueSettings(sp, queueName), serializer, serializer, sp.GetRequiredService<ILogger<Queue<T>>>());
        }

        private static QueueSettings GetQueueSettings(IServiceProvider sp, string topic)
        {
            var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
            EnsureArg.IsNotNull(kafkaSettings, nameof(kafkaSettings));

            var topicSettings = kafkaSettings.Topics.FirstOrDefault(x => x.Topic == topic || x.Alias == topic);

            return new QueueSettings(
                hosts: kafkaSettings.Hosts,
                topic: topicSettings?.Topic ?? topic
                )
            {
                ConsumerGroup = topicSettings?.ConsumerGroup ?? kafkaSettings.ConsumerGroup
            };
        }
    }
}
