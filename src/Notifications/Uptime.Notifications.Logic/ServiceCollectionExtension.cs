using AutoMapper;
using Uptime.Notifications.Model.Messages;
using Uptime.Notifications.Logic;
using Uptime.Notifications.Logic.Services;
using Uptime.Notifications.Model.Abstractions;
using Uptime.Notifications.Logic.Messaging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddMessagingServices()
                .AddTransient<ITemplateEngineProvider, TemplateEngineProvider>()
                .AddTransient<Profile, MessagingMapperProfile>()
                .AddTransient<INotificationsService, NotificationsService>()
                .AddTransient<INotificationSenderProvider, NotificationSenderProvider>()
                .AddScoped<ITemplatesService, TemplatesService>();
        }

        private static IServiceCollection AddMessagingServices(this IServiceCollection services)
        {
            services.ConfigureKafkaMessaging()
                .Queue<NotificationMsg>("Notifications").WithListener<NotificationMsgConsumer>();

            return services;
        }
    }
}
