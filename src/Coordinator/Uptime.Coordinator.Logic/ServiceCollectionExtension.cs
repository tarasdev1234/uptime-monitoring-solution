using Uptime.Coordinator.Logic.HostedServices;
using Uptime.Coordinator.Logic.Messaging;
using Uptime.Coordinator.Logic.Services;
using Uptime.Coordinator.Model.Abstractions;
using Uptime.Coordinator.Model.Messages;
using Uptime.Monitoring.Model.Messages;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddMessagingServices()
                .AddScoped<IActivitiesService, ActivitiesService>()
                .AddHostedService<StartupMonintoringsChecker>();
        }

        private static IServiceCollection AddMessagingServices(this IServiceCollection services)
        {
            services.ConfigureKafkaMessaging()
                .Queue<ActivityStatusChanged>("ActivityStatuses").WithListener<ActivityStatusConsumer>()
                .Queue<SetMonitorStatus>("SetMonitorStatusRequests");

            return services;
        }
    }
}
