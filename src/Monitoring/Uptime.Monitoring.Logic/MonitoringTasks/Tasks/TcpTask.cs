using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Uptime.Monitoring.HealthChecks.Abstractions;
using Uptime.Monitoring.HealthChecks.Checks;
using Uptime.Monitoring.Logic.Events;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Logic.MonitoringTasks.Tasks
{
    internal sealed class TcpTask : AbstractMonitoringTask
    {
        private TcpCheck tcpCheck;

        public TcpTask(
            TcpMonitorTask monitorTask,
            IEventsService eventSvc,
            EventManager eventMgr,
            ILogger logger) : base(monitorTask, eventSvc, eventMgr, logger)
        {
            tcpCheck = new TcpCheck(monitorTask.Target, monitorTask.Port);
        }

        protected override Task<IHealthCheckResult> CheckAsync()
            => tcpCheck.CheckAsync().ContinueWith(x => x.Result as IHealthCheckResult);
    }
}
