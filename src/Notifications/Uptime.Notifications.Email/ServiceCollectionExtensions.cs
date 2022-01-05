using Uptime.Notifications.Email.Abstractions;
using Uptime.Notifications.Email.Services;
using Uptime.Notifications.Model.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailNotificationServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IRazorTemplateEngine, RazorTemplateEngine>()
                .AddTransient<ITemplateEngine, RazorTemplateEngine>()
                .AddTransient<INotificationSender, SmtpNotificationSender>()
                .AddTransient<ISmtpNotificationSender, SmtpNotificationSender>();
        }
    }
}
