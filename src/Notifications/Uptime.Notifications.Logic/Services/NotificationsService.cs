using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Model.Abstractions;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Logic.Services
{
    internal sealed class NotificationsService : INotificationsService
    {
        private readonly ITemplateEngineProvider engineProvider;
        private readonly INotificationSenderProvider senderProvider;
        private readonly ITemplatesService templatesService;
        private readonly ILogger<NotificationsService> logger;

        public NotificationsService(ITemplateEngineProvider engineProvider,
            INotificationSenderProvider senderProvider,
            ITemplatesService templatesService,
            ILogger<NotificationsService> logger)
        {
            this.engineProvider = engineProvider;
            this.senderProvider = senderProvider;
            this.templatesService = templatesService;
            this.logger = logger;
        }

        public async Task NotifyAsync(Notification notification, CancellationToken token)
        {
            logger.LogInformation("Notification request. {NotificationScope}.{NotificationType}:{DeliveryType}", notification.Scope, notification.Type, notification.DeliveryType);

            var template = await templatesService.GetTemplateAsync(notification.Scope, notification.Type, token);

            if (template == null)
            {
                logger.LogError("Can't find template for notification {NotificationScope}.{NotificationType}:{DeliveryType}", notification.Scope, notification.Type, notification.DeliveryType);
                return;
            }

            logger.LogDebug("Render engine: {RenderEngine}", template.RenderEngine);

            var templateEngine = engineProvider.GetTemplateEngine(template.RenderEngine);
            var rendered = await templateEngine.RenderAsync(template, notification.Data, token);

            var senderService = senderProvider.GetNotificationSender(notification.DeliveryType);
            await senderService.SendAsync(notification, rendered, token);
        }

    }
}
