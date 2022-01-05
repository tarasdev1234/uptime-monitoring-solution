using Cassandra.Data.Linq;
using EnsureThat;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Repositories
{
    internal sealed class LastDowntimesRepository
    {
        private readonly Table<LastDowntime> table;

        public LastDowntimesRepository(Table<LastDowntime> table)
        {
            this.table = table;
        }

        public Task<LastDowntime> FindAsync(long userId)
        {
            return table
                .Where(x => x.UserId == userId)
                .FirstOrDefault()
                .ExecuteAsync();
        }

        public async Task UpdateAsync(long userId, Expression<Func<LastDowntime, LastDowntime>> updateExpression)
        {
            EnsureArg.IsNotNull(updateExpression, nameof(updateExpression));

            var query = table
                .Where(x => x.UserId == userId)
                .Select(updateExpression)
                .Update();
            query.EnableTracing();

            await query.ExecuteAsync();
        }
    }
}
