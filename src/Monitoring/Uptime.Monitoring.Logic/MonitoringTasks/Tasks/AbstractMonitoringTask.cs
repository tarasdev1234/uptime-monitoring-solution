using System.Threading.Tasks;
using Uptime.Monitoring.Model.Models;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.HealthChecks.Abstractions;
using Uptime.Monitoring.Logic.Events;
using Microsoft.Extensions.Logging;
using Uptime.Monitoring.Logic.Extensions;
using System.Linq;

namespace Uptime.Monitoring.Logic.MonitoringTasks.Tasks
{
    internal abstract class AbstractMonitoringTask : Schedule.Task<long>, IMonitoringTask
    {
        protected readonly EventManager eventMgr;
        protected readonly IEventsService eventSvc;

        protected MonitoringEvent prevEvent = null;

        public MonitorTask MonitorTask { get; private set; }

        public AbstractMonitoringTask(
            MonitorTask monitorTask,
            IEventsService eventSvc,
            EventManager eventMgr,
            ILogger logger
        ) : base(monitorTask.MonitorId, monitorTask.Name) {
            this.MonitorTask = monitorTask;
            this.eventSvc = eventSvc;
            this.eventMgr = eventMgr;
            Logger = logger;
        }

        public override async Task BeforeExecuteAsync()
        {
            Logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' Check previous event", GetType().Name, Id, Name);

            prevEvent = (await eventSvc.GetEventsByMonitorAsync(MonitorTask.UserId, MonitorTask.MonitorId, Pagination.First)).FirstOrDefault();

            if (prevEvent != null)
            {
                Logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' Previous event: {EventType}", GetType().Name, Id, Name, prevEvent.Type);
            }
            else
            {
                Logger.LogInformation("{TaskType}:{TaskId} '{TaskName}' No previous event", GetType().Name, Id, Name);
            }
        }

        public override async Task ExecuteAsync()
        {
            var result = await CheckAsync();
            var evnt = await result.ToMonitoringEventAsync(MonitorTask);
            evnt.PreviousEventType = prevEvent?.Type;
            bool eventRepeated = false;

            if (prevEvent?.Type == evnt.Type)
            {
                eventRepeated = true;
                evnt = await eventSvc.RepeatEventAsync(prevEvent, evnt.Details);
            }
            else
            {
                await eventSvc.AddEventAsync(evnt);
                prevEvent = evnt;
            }

            await eventMgr.FireHealthCheckFinished(MonitorTask, evnt, eventRepeated);
        }

        protected abstract Task<IHealthCheckResult> CheckAsync();
    }
}
