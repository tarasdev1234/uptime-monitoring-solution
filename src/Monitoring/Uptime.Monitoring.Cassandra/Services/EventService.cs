using EnsureThat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uptime.Monitoring.Cassandra.Repositories;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Services
{
    internal sealed class EventService : IEventsService
    {
        private readonly EventsRepository eventsRepository;
        private readonly LastDowntimesRepository lastDowntimesRepository;
        private readonly RecentMonitoringEventsRepository recentEventsRepository;

        public EventService (
            EventsRepository eventsRepository,
            LastDowntimesRepository lastDowntimesRepository,
            RecentMonitoringEventsRepository recentEventsRepository) {
            this.eventsRepository = eventsRepository;
            this.lastDowntimesRepository = lastDowntimesRepository;
            this.recentEventsRepository = recentEventsRepository;
        }

        public async Task AddEventAsync(MonitoringEvent @event)
        {
            EnsureArg.IsNotNull(@event, nameof(@event));

            await eventsRepository.AddAsync(@event);

            if (!@event.SourceEventId.HasValue)
            {
                await recentEventsRepository.AddAsync(new RecentMonitoringEvent(@event));

                if (@event.Type == EventType.Down)
                {
                    await lastDowntimesRepository.UpdateAsync(
                    @event.UserId,
                    x => new LastDowntime
                    {
                        MonitorId = @event.MonitorId,
                        FirstEventId = @event.Id,
                        LastEventId = @event.Id,
                        Start = @event.Created,
                        LastUpdate = @event.Created
                    });
                }
            }
        }

        /// <summary>
        /// Insert event that repeats multiple times at row.
        /// </summary>
        /// <param name="newEvent">New event to insert</param>
        /// <param name="firstEvent">First event in chain of same events</param>
        /// <returns></returns>
        public async Task<MonitoringEvent> RepeatEventAsync(MonitoringEvent eventToRepeat, Dictionary<string, string> newEventDetails)
        {
            EnsureArg.IsNotNull(eventToRepeat, nameof(eventToRepeat));
            MonitoringEvent result;

            var repeats = eventToRepeat.Repeats + 1;
            var now = DateTime.UtcNow;

            eventToRepeat.Repeats = repeats;
            eventToRepeat.LastRepeat = now;

            if (eventToRepeat.Type == EventType.Up)
            {
                await eventsRepository.UpdateAsync(
                    eventToRepeat.UserId,
                    eventToRepeat.MonitorId,
                    eventToRepeat.Id,
                    x => new MonitoringEvent
                    {
                        Repeats = repeats,
                        LastRepeat = now
                    });

                result = eventToRepeat;
            }
            else
            {
                var newEvent = MonitoringEvent.CreateFrom(eventToRepeat, newEventDetails);
                newEvent.PreviousEventType = eventToRepeat.Type;

                await eventsRepository.AddAsync(newEvent);

                if (eventToRepeat.Type == EventType.Down && !eventToRepeat.SourceEventId.HasValue)
                {
                    await lastDowntimesRepository.UpdateAsync(
                        newEvent.UserId,
                        x => new LastDowntime
                        {
                            MonitorId = newEvent.MonitorId,
                            FirstEventId = eventToRepeat.Id,
                            LastEventId = newEvent.Id,
                            Start = eventToRepeat.Created,
                            LastUpdate = newEvent.Created
                        });
                }

                result = newEvent;
            }

            await recentEventsRepository.UpdateAsync(
                eventToRepeat.UserId,
                eventToRepeat.Id,
                x => new RecentMonitoringEvent
                {
                    Repeats = repeats,
                    LastRepeat = now
                });

            return result;
        }

        public Task<IReadOnlyCollection<RecentMonitoringEvent>> GetRecentEvents(long userId, Pagination pagination = null)
        {
            return recentEventsRepository.FindByUserAsync(userId, pagination: pagination);
        }

        public Task<IReadOnlyCollection<RecentMonitoringEvent>> GetRecentEvents(long userId, DateTimeOffset createdAfter, Pagination pagination = null)
        {
            return recentEventsRepository.FindByUserAsync(userId, createdAfter, pagination);
        }

        public Task<IReadOnlyCollection<MonitoringEvent>> GetEventsByMonitorAsync(long userId, long monitorId, Pagination pagination = null)
        {
            return eventsRepository.FindAsync(userId, monitorId, pagination);
        }

        public Task<IReadOnlyCollection<MonitoringEvent>> GetEventsByMonitorsAsync(long userId, IEnumerable<long> monitorIds, DateTimeOffset createdAfter, Pagination pagination = null)
        {
            return eventsRepository.FindAsync(userId, monitorIds, createdAfter);
        }

        public Task<LastDowntime> GetLastDowntimeEventAsync(long userId)
        {
            return lastDowntimesRepository.FindAsync(userId);
        }

        public Task DeleteEventsAsync(long userId, long monitorId)
        {
            return eventsRepository.DeleteAsync(userId, monitorId);
        }
    }
}
