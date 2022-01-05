using Microsoft.Extensions.DependencyInjection;
using System;
using Uptime.Notifications.Email.Abstractions;
using Uptime.Notifications.Model.Abstractions;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Logic.Services
{
    internal sealed class NotificationSenderProvider : INotificationSenderProvider
    {
        private readonly IServiceProvider serviceProvider;

        public NotificationSenderProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public INotificationSender GetNotificationSender(DeliveryType deliveryType)
            => deliveryType switch
        {
            DeliveryType.Email => serviceProvider.GetRequiredService<ISmtpNotificationSender>(),
            _ => throw new ArgumentException($"Unsupported delivery type:{deliveryType}", nameof(deliveryType))
        };
    }
}
