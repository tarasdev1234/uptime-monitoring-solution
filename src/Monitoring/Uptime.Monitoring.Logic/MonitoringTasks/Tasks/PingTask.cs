using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.HealthChecks.Checks;
using Microsoft.Extensions.Logging;
using Uptime.Monitoring.Logic.Events;
using System.Threading.Tasks;
using Uptime.Monitoring.HealthChecks.Abstractions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Logic.MonitoringTasks.Tasks
{
    internal sealed class PingTask : AbstractMonitoringTask
    {
        private PingCheck pingCheck;

        public PingTask (
            PingMonitorTask monitorTask,
            IEventsService eventSvc,
            EventManager eventMgr,
            ILogger logger
        ) : base(monitorTask, eventSvc, eventMgr, logger)
        {
            pingCheck = new PingCheck(monitorTask.Target);
        }

        protected override Task<IHealthCheckResult> CheckAsync()
            => pingCheck.CheckAsync().ContinueWith(x => x.Result as IHealthCheckResult);
    }
}
