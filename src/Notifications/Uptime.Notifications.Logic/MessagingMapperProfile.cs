using AutoMapper;
using Uptime.Notifications.Model.Messages;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Logic
{
    public sealed class MessagingMapperProfile : Profile
    {
        public MessagingMapperProfile()
        {
            CreateMap<NotificationMsg, Notification>();
        }
    }
}
