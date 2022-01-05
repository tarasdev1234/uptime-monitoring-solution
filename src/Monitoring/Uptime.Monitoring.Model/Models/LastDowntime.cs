using System;

namespace Uptime.Monitoring.Model.Models
{
    public sealed class LastDowntime
    {
        public long UserId { get; set; }

        public long MonitorId { get; set; }

        public Guid FirstEventId { get; set; }

        public Guid LastEventId { get; set; }

        public DateTime Start { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
