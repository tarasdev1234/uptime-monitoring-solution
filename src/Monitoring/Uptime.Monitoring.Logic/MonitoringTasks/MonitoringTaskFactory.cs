using Microsoft.Extensions.Logging;
using System;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Models;
using Uptime.Monitoring.Logic.Events;
using Uptime.Monitoring.Logic.MonitoringTasks.Tasks;

namespace Uptime.Monitoring.Logic.MonitoringTasks
{
    internal sealed class MonitoringTaskFactory {
        private readonly IEventsService monitoringEvents;
        private readonly EventManager eventMgr;
        private readonly ILoggerFactory loggerFactory;

        private const string LoggerName = "Task-";

        public MonitoringTaskFactory (
            IEventsService monitoringEvents,
            EventManager eventMgr,
            ILoggerFactory loggerFactory
        ) {
            this.monitoringEvents = monitoringEvents;
            this.loggerFactory = loggerFactory;
            this.eventMgr = eventMgr;
        }

        public IMonitoringTask BuildTaskAsync(MonitorTask task) => task switch
        {
            TcpMonitorTask t => new TcpTask(t, monitoringEvents, eventMgr, CreateLogger(t)),
            HttpMonitorTask t => new HttpTask(t, monitoringEvents, eventMgr, CreateLogger(t)),
            PingMonitorTask t => new PingTask(t, monitoringEvents, eventMgr, CreateLogger(t)),
            KeywordMonitorTask t => new KeywordTask(t, monitoringEvents, eventMgr, CreateLogger(t)),
            _ => throw new Exception($"Invalid task type {task.GetType().Name}"),
        };

        public IMonitoringTask BuildTaskAsync(Monitor monitor) => BuildTaskAsync(ToMonitorTask(monitor));

        private static MonitorTask ToMonitorTask(Monitor monitor)
        {
            MonitorTask result = monitor switch
            {
                TcpMonitor m => new TcpMonitorTask { Port = m.Port },
                HttpMonitor _ => new HttpMonitorTask(),
                PingMonitor _ => new PingMonitorTask(),
                KeywordMonitor m => new KeywordMonitorTask { ShouldContain = m.ContainsWord, Keyword = m.Keyword },
                _ => throw new Exception($"Invalid monitor type {monitor.Type}")
            };

            result.MonitorId = monitor.Id;
            result.Name = monitor.Name;
            result.Target = monitor.Url;
            result.UserId = monitor.UserId;

            return result;
        }

        private ILogger CreateLogger(MonitorTask task) => loggerFactory.CreateLogger(LoggerName + task.MonitorId);
    }
}
