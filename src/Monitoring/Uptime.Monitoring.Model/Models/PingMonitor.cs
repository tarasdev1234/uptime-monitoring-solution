namespace Uptime.Monitoring.Model.Models
{
    public sealed class PingMonitor : Monitor
    {
        public PingMonitor() : base(MonitorType.PING)
        {
        }
    }
}
