using System.Collections.Generic;
using System.Dynamic;

namespace Uptime.Notifications.Model.Messages
{
    public class NotificationMsg
    {
        public IEnumerable<string> Destinations { get; set; }

        public string Scope { get; private set; }

        public string Type { get; private set; }

        public DeliveryType DeliveryType { get; set; }

        public ExpandoObject Data { get; set; }

        private NotificationMsg()
        {
        }

        public NotificationMsg(string scope, string type)
        {
            Scope = scope;
            Type = type;
        }
    }
}
