using System.Collections.Generic;
using System.Dynamic;

namespace Uptime.Notifications.Model.Models
{
    public class Notification
    {
        public ICollection<string> Destinations { get; set; }

        public string Scope { get; set; }

        public string Type { get; set; }

        public DeliveryType DeliveryType { get; set; }

        public ExpandoObject Data { get; set; }
    }
}
