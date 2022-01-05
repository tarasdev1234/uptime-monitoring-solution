using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data.Linq;
using EnsureThat;
using Uptime.Monitoring.Cassandra.Extensions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Repositories
{
    internal sealed class RecentMonitoringEventsRepository
    {
        public int Ttl { get; set; } = 31536000; // One year, seconds

        private readonly Table<RecentMonitoringEvent> table;

        public RecentMonitoringEventsRepository(Table<RecentMonitoringEvent> table) {
            this.table = table;
        }

        public async Task<IReadOnlyCollection<RecentMonitoringEvent>> FindByUserAsync(long userId, DateTimeOffset? createdAfter = null, Pagination pagination = null)
        {
            var query = table.Where(x => x.UserId == userId);

            if (createdAfter.HasValue)
            {
                query = query.Where(x => x.Id > CqlFunction.MaxTimeUuid((DateTimeOffset)createdAfter));
            }

            return (await query.ExecutePagedAsync(pagination)).ToList();
        }

        public async Task AddAsync(RecentMonitoringEvent entity)
        {
            EnsureArg.IsNotNull(entity, nameof(entity));

            var now = DateTime.UtcNow;

            entity.Created = now;

            if (entity.Id == default)
            {
                entity.Id = TimeUuid.NewId(now);
            }

            await table.Insert(entity).SetTTL(Ttl).ExecuteAsync();
        }

        public async Task UpdateAsync(long userId, Guid eventId, Expression<Func<RecentMonitoringEvent, RecentMonitoringEvent>> updateExpression)
        {
            await table
                .Where(x => x.UserId == userId && x.Id == eventId)
                .Select(updateExpression)
                .Update()
                .SetTTL(Ttl)
                .ExecuteAsync();
        }
    }

}