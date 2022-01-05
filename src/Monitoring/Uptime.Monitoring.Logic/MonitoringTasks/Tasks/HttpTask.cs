using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.HealthChecks.Checks;
using Uptime.Monitoring.Logic.Events;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Uptime.Monitoring.HealthChecks.Abstractions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Logic.MonitoringTasks.Tasks
{
    internal class HttpTask : AbstractMonitoringTask {
        protected HttpCheck HttpCheck;

        public HttpTask (
            HttpMonitorTask monitorTask,
            IEventsService eventSvc,
            EventManager eventMgr,
            ILogger logger
        ) : base(monitorTask, eventSvc, eventMgr, logger) {
            HttpCheck = new HttpCheck(new Uri(monitorTask.Target), HttpCheck.DefaultUrlCheck);
        }

        protected override Task<IHealthCheckResult> CheckAsync()
            => HttpCheck.CheckAsync().ContinueWith(x => x.Result as IHealthCheckResult);
    }
}
