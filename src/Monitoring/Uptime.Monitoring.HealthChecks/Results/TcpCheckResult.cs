using System.Net;

namespace Uptime.Monitoring.HealthChecks.Results
{
    public sealed class TcpCheckResult : HealthCheckResult
    {
        public IPAddress? ResolvedIp { get; set; }
    }
}
