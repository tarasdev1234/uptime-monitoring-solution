using System;
using System.Collections.Generic;
using System.Net;

namespace Uptime.Monitoring.HealthChecks.Results
{
    public sealed class PingCheckResult : HealthCheckResult
    {
        public IPAddress? ResolvedIp { get; set; }
        public IReadOnlyList<IPAddress> TraceRoute { get; set; } = Array.Empty<IPAddress>();
    }
}
