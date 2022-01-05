using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Model.Abstractions
{
    public interface IEventsService
    {
        Task AddEventAsync(MonitoringEvent @event);

        Task<MonitoringEvent> RepeatEventAsync(MonitoringEvent eventToRepeat, Dictionary<string, string> newEventDetails);

        Task<IReadOnlyCollection<RecentMonitoringEvent>> GetRecentEvents(long userId, Pagination pagination = null);

        Task<IReadOnlyCollection<RecentMonitoringEvent>> GetRecentEvents(long userId, DateTimeOffset createdAfter, Pagination pagination = null);

        Task<IReadOnlyCollection<MonitoringEvent>> GetEventsByMonitorAsync(long userId, long monitorId, Pagination pagination = null);

        Task<IReadOnlyCollection<MonitoringEvent>> GetEventsByMonitorsAsync(long userId, IEnumerable<long> monitorIds, DateTimeOffset createdAfter, Pagination pagination = null);

        Task DeleteEventsAsync(long userId, long monitorId);

        Task<LastDowntime> GetLastDowntimeEventAsync(long userId);
    }
}