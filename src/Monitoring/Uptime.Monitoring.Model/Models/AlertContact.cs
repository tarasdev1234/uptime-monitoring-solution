using Newtonsoft.Json;
using System.Collections.Generic;

namespace Uptime.Monitoring.Model.Models {
    public class AlertContact {
        public static readonly Dictionary<int, string> Types = new Dictionary<int, string>() {
            { (int)ContactType.EMAIL, "Email" },
        };

        public static readonly Dictionary<int, string> NotificationTypes = new Dictionary<int, string>() {
            { (int)Models.NotificationType.ONLYDOWN, "Only Down" },
            { (int)Models.NotificationType.ONLYUP, "Only Up" },
            { (int)Models.NotificationType.UPDOWN, "Up And Down" },
        };

        public long Id { get; set; }

        public long UserId { get; set; }

        public int Type { get; set; }

        public int NotificationType { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        [JsonIgnore]
        public List<MonitorAlertContact> Monitors { get; set; }
    }
}
