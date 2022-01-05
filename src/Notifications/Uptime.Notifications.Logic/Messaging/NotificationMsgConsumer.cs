using AutoMapper;
using Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Model.Abstractions;
using Uptime.Notifications.Model.Messages;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Logic.Messaging
{
    internal sealed class NotificationMsgConsumer : ScopedConsumer<NotificationMsg>
    {
        public NotificationMsgConsumer(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
        }

        protected override Task InternalConsume(NotificationMsg message, IServiceProvider scopedServiceProvider, CancellationToken token)
        {
            var service = scopedServiceProvider.GetRequiredService<INotificationsService>();
            var mapper = scopedServiceProvider.GetRequiredService<IMapper>();

            return service.NotifyAsync(mapper.Map<Notification>(message), token);
        }
    }
}
