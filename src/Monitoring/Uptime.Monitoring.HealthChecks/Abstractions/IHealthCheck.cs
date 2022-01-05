using System.Threading.Tasks;

namespace Uptime.Monitoring.HealthChecks.Abstractions {
    public interface IHealthCheck<T> where T: notnull, IHealthCheckResult
    {
        string Target { get; }

        Task<T> CheckAsync ();
    }
}
