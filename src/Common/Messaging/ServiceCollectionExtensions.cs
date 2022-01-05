using Confluent.Kafka;
using HealthChecks.Kafka;
using Messaging;
using Messaging.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IMessagingConfigurator ConfigureKafkaMessaging(this IServiceCollection serviceCollection)
        {
            var check = new HealthCheckRegistration(
                "kafka",
                sp =>
                {
                    var settings = sp.GetService<IOptions<KafkaSettings>>().Value;
                    return new KafkaHealthCheck(new ProducerConfig
                    {
                        BootstrapServers = settings.Hosts
                    }, default);
                },
                default,
                default,
                TimeSpan.FromSeconds(5));

            serviceCollection
                .AddHealthChecks()
                .Add(check);

            serviceCollection
                .AddHostedService<MessageQueueBackgroundService>()
                .AddTransient<IPostConfigureOptions<KafkaSettings>, KafkaOptionsPostConfigure>();

            return new KafkaConfigurator(serviceCollection);
        }
    }
}
