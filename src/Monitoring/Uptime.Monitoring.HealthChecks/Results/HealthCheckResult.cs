using System;
using Uptime.Monitoring.HealthChecks.Abstractions;

namespace Uptime.Monitoring.HealthChecks.Results
{
    public abstract class HealthCheckResult : IHealthCheckResult
    {
        public CheckStatus CheckStatus { get; set; }

        public string? Description { get; set; }

        public string? Target { get; set; }

        public TimeSpan ResponseTime { get; set; }

        public Exception? Exception { get; set; }
    }
}
