namespace Uptime.Monitoring.Model.Models
{
    public sealed class TcpMonitor : Monitor
    {
        public int Port { get; set; }

        public TcpMonitor() : base(MonitorType.TCPPORT)
        {
        }
    }
}
