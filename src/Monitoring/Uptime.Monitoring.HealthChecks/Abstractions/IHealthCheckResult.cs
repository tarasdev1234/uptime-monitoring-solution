using System;

namespace Uptime.Monitoring.HealthChecks.Abstractions {
    public interface IHealthCheckResult {
        CheckStatus CheckStatus { get; }

        string? Description { get; }

        string? Target { get; }

        TimeSpan ResponseTime { get; }

        Exception? Exception { get; }
    }
}
