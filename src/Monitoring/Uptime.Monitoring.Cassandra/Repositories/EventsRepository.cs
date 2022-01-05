using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra.Data.Linq;
using Uptime.Monitoring.Model.Models;
using System.Linq;
using Uptime.Monitoring.Cassandra.Extensions;
using Cassandra;
using EnsureThat;
using System;
using System.Linq.Expressions;

namespace Uptime.Monitoring.Cassandra.Repositories
{
    internal sealed class EventsRepository
    {
        private readonly Table<MonitoringEvent> table;

        public EventsRepository(Table<MonitoringEvent> table)
        {
            this.table = table;
        }

        public async Task<IReadOnlyCollection<MonitoringEvent>> FindAsync(long userId, long monitorId, Pagination pagination = null)
        {
            var query = table.Where(x => x.UserId == userId && x.MonitorId == monitorId);

            return (await query.ExecutePagedAsync(pagination)).ToList();
        }

        public async Task<IReadOnlyCollection<MonitoringEvent>> FindAsync(long userId, IEnumerable<long> monitorId, DateTimeOffset? createdAfter, Pagination pagination = null)
        {
            EnsureArg.IsNotNull(monitorId, nameof(monitorId));

            var query = table.Where(x => x.UserId == userId && monitorId.Contains(x.MonitorId));

            if (createdAfter.HasValue)
            {
                query = query.Where(x => x.Id > CqlFunction.MaxTimeUuid((DateTimeOffset)createdAfter));
            }

            return (await query.ExecutePagedAsync(pagination)).ToList();
        }

        public async Task AddAsync(MonitoringEvent entity)
        {
            EnsureArg.IsNotNull(entity, nameof(entity));

            var now = DateTime.UtcNow;

            entity.Created = now;

            if (entity.Id == default)
            {
                entity.Id = TimeUuid.NewId(now);
            }

            await table.Insert(entity).ExecuteAsync();
        }

        public async Task UpdateAsync(long userId, long monitorId, Guid eventId, Expression<Func<MonitoringEvent, MonitoringEvent>> updateExpression)
        {
            await table
                .Where(x => x.UserId == userId && x.MonitorId == monitorId && x.Id == eventId)
                .Select(updateExpression)
                .Update()
                .ExecuteAsync();
        }

        public async Task DeleteAsync(long userId, long monitorId)
        {
            await table
                .Where(x => x.UserId == userId && x.MonitorId == monitorId)
                .Delete()
                .ExecuteAsync();
        }
    }
}