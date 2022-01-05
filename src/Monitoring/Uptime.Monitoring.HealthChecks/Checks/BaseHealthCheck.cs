using EnsureThat;
using System;
using System.Threading.Tasks;
using Uptime.Monitoring.HealthChecks.Abstractions;
using Uptime.Monitoring.HealthChecks.Results;

namespace Uptime.Monitoring.HealthChecks.Checks
{
    public abstract class BaseHealthCheck<T> : IHealthCheck<T> where T: notnull, HealthCheckResult, new()
    {
        public abstract string Target { get; }

        public async Task<T> CheckAsync()
        {
            try
            {
                return await CheckAsyncInternal().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Fail(ex);
            }
        }

        protected abstract Task<T> CheckAsyncInternal();

        protected T Fail(Exception exception)
        {
            EnsureArg.IsNotNull(exception, nameof(exception));

            var ex = exception.InnerException ?? exception;

            return new T
            {
                CheckStatus = CheckStatus.Unhealthy,
                Exception = ex,
                Target = Target,
                Description = ex.Message
            };
        }
    }
}
