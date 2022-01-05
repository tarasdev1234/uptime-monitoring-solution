using System;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Model.Abstractions
{
    public interface ITaskEventsHandler {
        Task Start (Monitor monitor);

        Task Stop (Monitor monitor);

        Task Pause (Monitor monitor);

        Task Up (MonitorTask task, MonitoringEvent @event, bool eventRepeated);

        Task Down (MonitorTask task, MonitoringEvent @event, bool eventRepeated);
    }
}
