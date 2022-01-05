using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Model.Messages
{
    public sealed class SetMonitorStatus
    {
        public long MonitorId { get; set; }

        public MonitorStatus Status { get; set; }
    }
}
