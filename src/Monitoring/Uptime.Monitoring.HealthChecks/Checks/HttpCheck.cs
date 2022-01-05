using EnsureThat;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Uptime.Monitoring.HealthChecks.Results;

namespace Uptime.Monitoring.HealthChecks.Checks
{
    public class HttpCheck : BaseHealthCheck<HttpCheckResult> {

        private Uri url;
        private Func<HttpResponseMessage, TimeSpan, Task<HttpCheckResult>> checkFunc;

        public HttpCheck (Uri url, Func<HttpResponseMessage, TimeSpan, Task<HttpCheckResult>> checkFunc) {
            this.url = url;
            this.checkFunc = checkFunc;
        }

        public override string Target => url.ToString();

        protected override async Task<HttpCheckResult> CheckAsyncInternal () {
            var result = await GetHttpWithTimingInfo(url).ConfigureAwait(false);
            var response = result.Item1;

            return await checkFunc(result.Item1, result.Item2).ConfigureAwait(false);
        }

        public static Task<HttpCheckResult> DefaultUrlCheck (HttpResponseMessage response, TimeSpan time){
            EnsureArg.IsNotNull(response, nameof(response));

            return Task.FromResult(new HttpCheckResult(response) {
                ResponseTime = time,
            });
        }

        private async Task<Tuple<HttpResponseMessage, TimeSpan>> GetHttpWithTimingInfo (Uri url) {
            var stopWatch = Stopwatch.StartNew();

            using (var client = CreateHttpClient()) {
                var result = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                return new Tuple<HttpResponseMessage, TimeSpan>(result, stopWatch.Elapsed);
            }
        }

        private static HttpClient CreateHttpClient () {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            return httpClient;
        }
    }
}
