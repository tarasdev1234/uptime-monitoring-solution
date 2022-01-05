using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Reliablesite.Authority.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Monitoring.Data;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Plugin.Services
{
    internal class StatisticsService : IStatisticsService
    {
        private readonly IUsersInternalClient usersInternalClient;
        private readonly UptimeMonitoringContext monitoringContext;
        private readonly IEventsService eventsService;
        private readonly IMemoryCache cache;

        private readonly TimeSpan CacheExpiration = TimeSpan.FromHours(12);
        private const string UsersCountCache = "UsersCount";
        private const string MonitorsCountCache = "MonitorsCount";
        private const string AvgMonthUptimeCache = "AvgMonthUp_";
        private const string AvgWeekUptimeCache = "AvgWeekUp_";
        private const string AvgDayUptimeCache = "AvgDayUp_";

        public StatisticsService(
            IUsersInternalClient usersInternalClient,
            UptimeMonitoringContext monitoringContext,
            IEventsService eventsService,
            IMemoryCache cache)
        {
            this.usersInternalClient = usersInternalClient;
            this.monitoringContext = monitoringContext;
            this.eventsService = eventsService;
            this.cache = cache;
        }

        public Task<int> GetNotificationsCountAsync()
        {
            return Task.FromResult(0);
        }

        public async Task<int> GetSitesMonitoredCountAsync()
        {
            return await cache.GetOrCreateAsync(MonitorsCountCache,
                entry =>
                {
                    entry.SetAbsoluteExpiration(CacheExpiration);
                    return monitoringContext.Monitors.CountAsync();
                });
        }

        public async Task<int> GetUsersCountAsync()
        {
            return await cache.GetOrCreateAsync(UsersCountCache,
                entry =>
                {
                    entry.SetAbsoluteExpiration(CacheExpiration);
                    return usersInternalClient.GetUsersCountAsync();
                });
        }

        public async Task<AvgUptime> GetAverageUptimeAsync(long userId)
        {
            var result = new AvgUptime();

            var now = DateTime.UtcNow;

            IEnumerable<IGrouping<long, RecentMonitoringEvent>> recentEvents = null;

            result.LastMonth = await cache.GetOrCreateAsync(AvgMonthUptimeCache + userId,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                    recentEvents ??= (await eventsService.GetRecentEvents(userId, now - TimeSpan.FromDays(30))).GroupBy(x => x.MonitorId);
                    return CalculateAverageUptime(now, TimeSpan.FromDays(30), recentEvents);
                });

            result.LastWeek = await cache.GetOrCreateAsync(AvgWeekUptimeCache + userId,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                    recentEvents ??= (await eventsService.GetRecentEvents(userId, now - TimeSpan.FromDays(7))).GroupBy(x => x.MonitorId);
                    return CalculateAverageUptime(now, TimeSpan.FromDays(7), recentEvents);
                });

            result.LastDay = await cache.GetOrCreateAsync(AvgDayUptimeCache + userId,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                    recentEvents ??= (await eventsService.GetRecentEvents(userId, now - TimeSpan.FromDays(1))).GroupBy(x => x.MonitorId);
                    return CalculateAverageUptime(now, TimeSpan.FromDays(1), recentEvents);
                });

            return result;
        }

        private double CalculateAverageUptime(DateTime now, TimeSpan duration, IEnumerable<IGrouping<long, RecentMonitoringEvent>> groupedEvents)
        {
            var monitorsCount = groupedEvents.Count();

            if (monitorsCount == 0)
            {
                return 1.0;
            }

            var result = 0.0;
            var to = now - duration;

            foreach (var group in groupedEvents)
            {
                var overallUp = TimeSpan.Zero;
                var overallTurnedOn = TimeSpan.Zero;

                var prev = new RecentMonitoringEvent
                {
                    Created = now,
                    Type = group.First().Type
                };

                foreach (var ev in group.Where(x => x.Created >= to))
                {
                    if (ev.Type == EventType.Up)
                    {
                        overallUp += prev.Created - ev.Created;
                        overallTurnedOn += prev.Created - ev.Created;
                    }

                    if (ev.Type == EventType.Down)
                    {
                        overallTurnedOn += prev.Created - ev.Created;
                    }
                }

                if (overallTurnedOn == TimeSpan.Zero)
                {
                    result += 1.0;
                }
                else
                {
                    result += overallUp / overallTurnedOn;
                }
            }

            return result / groupedEvents.Count();
        }
    }
}
