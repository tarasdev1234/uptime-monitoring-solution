using Uptime.Monitoring.Model.Messages;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection services)
        {
            services.ConfigureKafkaMessaging()
                .Queue<SetMonitorStatus>("SetMonitorStatusRequests");

            return services;
        }
    }
}
