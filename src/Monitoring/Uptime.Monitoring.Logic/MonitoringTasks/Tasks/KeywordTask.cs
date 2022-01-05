using System;
using System.Net.Http;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.HealthChecks;
using Uptime.Monitoring.HealthChecks.Checks;
using Uptime.Monitoring.Logic.Events;
using Microsoft.Extensions.Logging;
using Uptime.Monitoring.Model.Models;
using EnsureThat;
using Uptime.Monitoring.HealthChecks.Abstractions;
using Uptime.Monitoring.HealthChecks.Results;

namespace Uptime.Monitoring.Logic.MonitoringTasks.Tasks
{
    internal class KeywordTask : AbstractMonitoringTask
    {
        private static readonly string FOUND_PHRASE = "Keyword Found";
        private static readonly string NOT_FOUND_PHRASE = "Keyword Not Found";

        protected HttpCheck HttpCheck;

        public new KeywordMonitorTask MonitorTask => (KeywordMonitorTask)base.MonitorTask;

        public KeywordTask (
            KeywordMonitorTask monitorTask,
            IEventsService eventSvc,
            EventManager eventMgr,
            ILogger logger
        ) : base(monitorTask, eventSvc, eventMgr, logger) {

            EnsureArg.IsNotNullOrEmpty(monitorTask.Keyword, nameof(monitorTask.Keyword));
            HttpCheck = new HttpCheck(new Uri(monitorTask.Target), KeywordUrlCheck);
        }

        protected override Task<IHealthCheckResult> CheckAsync()
            => HttpCheck.CheckAsync().ContinueWith(x => x.Result as IHealthCheckResult);

        private async Task<HttpCheckResult> KeywordUrlCheck (HttpResponseMessage response, TimeSpan time) {
            var body = await response.Content?.ReadAsStringAsync();

            string reason;
            CheckStatus status;

            if (string.IsNullOrEmpty(body)) {
                reason = "Empty Body Content";
                status = CheckStatus.Unhealthy;
            } else if (!response.IsSuccessStatusCode) {
                reason = response.ReasonPhrase;
                status = CheckStatus.Unhealthy;
            } else {
                var success = IsSuccess(body);

                if (success) {
                    status = CheckStatus.Healthy;
                    reason = MonitorTask.ShouldContain ? FOUND_PHRASE : NOT_FOUND_PHRASE;
                } else {
                    status = CheckStatus.Unhealthy;
                    reason = MonitorTask.ShouldContain ? NOT_FOUND_PHRASE : FOUND_PHRASE;
                }
            }

            response.ReasonPhrase = reason;

            return new HttpCheckResult() {
                CheckStatus = status,
                Response = response,
                ResponseTime = time,
                Description = "OK"
            };
        }

        private bool IsSuccess(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }

            var contains = body.Contains(MonitorTask.Keyword);

            // TODO: xor?
            return MonitorTask.ShouldContain ? contains : !contains;
        }
    }
}
