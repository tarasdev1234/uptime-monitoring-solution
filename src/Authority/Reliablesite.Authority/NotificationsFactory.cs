using System.Dynamic;
using Uptime.Notifications.Model.Messages;

namespace Reliablesite.Authority
{
    public static class NotificationsFactory
    {
        public static NotificationMsg ConfirmEmail(string email, string callbackUrl)
        {
            dynamic data = new ExpandoObject();
            data.CallbackUrl = callbackUrl;

            return new NotificationMsg(NotificationTypes.Scope, NotificationTypes.ConfirmEmail)
            {
                Destinations = new[] { email },
                DeliveryType = DeliveryType.Email,
                Data = data
            };
        }

        public static NotificationMsg ResetPassword(string email, string callbackUrl)
        {
            dynamic data = new ExpandoObject();
            data.CallbackUrl = callbackUrl;

            return new NotificationMsg(NotificationTypes.Scope, NotificationTypes.ResetPassword)
            {
                Destinations = new[] { email },
                DeliveryType = DeliveryType.Email,
                Data = data
            };
        }
    }
}
