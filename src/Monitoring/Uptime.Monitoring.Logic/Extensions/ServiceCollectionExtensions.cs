using Uptime.Coordinator.Model.Messages;
using Uptime.Monitoring.Logic.EventHandlers;
using Uptime.Monitoring.Logic.Events;
using Uptime.Monitoring.Logic.HostedServices;
using Uptime.Monitoring.Logic.Messaging;
using Uptime.Monitoring.Logic.MonitoringTasks;
using Uptime.Monitoring.Logic.Services;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Messages;
using Uptime.Notifications.Model.Messages;
using Uptime.Schedule;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMonitoringServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<Scheduler<long>>()
                .AddHostedService<AssignedTaskRunner>()
                .AddScoped<IMonitoringTaskService, MonitoringTaskService>()
                .AddScoped<ITaskEventsHandler, StatusEventsHandler>()
                .AddScoped<ITaskEventsHandler, FailureConfirmationEventHandler>()
                .AddScoped<ITaskEventsHandler, NotificationEventsHandler>()
                .AddScoped<MonitoringTaskFactory>()
                .AddSingleton<EventManager>();
        }

        public static IServiceCollection AddMessagingServices(this IServiceCollection services)
        {
            services.ConfigureKafkaMessaging()
                .Queue<FailureConfirmation>("FailureConfirmationRequests").WithListener<FailureConfirmationConsumer>()
                .Queue<SetMonitorStatus>("SetMonitorStatusRequests").WithListener<SetMonitorStatusConsumer>()
                .Queue<ActivityStatusChanged>("ActivityStatuses")
                .Queue<NotificationMsg>("Notifications");

            return services;
        }
    }
}