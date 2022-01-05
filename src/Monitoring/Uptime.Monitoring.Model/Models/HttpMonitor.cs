namespace Uptime.Monitoring.Model.Models
{
    public sealed class HttpMonitor : Monitor
    {
        public HttpMonitor() : base(MonitorType.HTTP)
        {
        }
    }
}
