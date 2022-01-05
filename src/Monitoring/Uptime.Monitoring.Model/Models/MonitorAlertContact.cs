using Newtonsoft.Json;

namespace Uptime.Monitoring.Model.Models
{
    public class MonitorAlertContact {
        [JsonIgnore]
        public Monitor Monitor { get; set; }

        public long MonitorId { get; set; }

        [JsonIgnore]
        public AlertContact AlertContact { get; set; }

        public long AlertContactId { get; set; }
    }
}
