using EnsureThat;
using System.Net.Http;

namespace Uptime.Monitoring.HealthChecks.Results
{
    public sealed class HttpCheckResult : HealthCheckResult
    {
        public HttpResponseMessage? Response { get; set; }

        public HttpCheckResult()
        {
        }

        public HttpCheckResult(HttpResponseMessage response)
        {
            EnsureArg.IsNotNull(response, nameof(response));

            CheckStatus = response.IsSuccessStatusCode ? CheckStatus.Healthy : CheckStatus.Unhealthy;
            Response = response;
            Target = response.RequestMessage.RequestUri.ToString();
            Description = response.ReasonPhrase;
        }
    }
}
