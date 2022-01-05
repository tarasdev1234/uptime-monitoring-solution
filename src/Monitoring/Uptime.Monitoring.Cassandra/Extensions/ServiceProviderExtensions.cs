using Cassandra;
using Cassandra.Data.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider CreateCassandraDb(this IServiceProvider serviceProvider)
        {
            var session = serviceProvider.GetRequiredService<ISession>();
            session.CreateKeyspaceIfNotExists(session.Keyspace);

            serviceProvider.GetRequiredService<Table<RecentMonitoringEvent>>().CreateIfNotExists();
            serviceProvider.GetRequiredService<Table<MonitoringEvent>>().CreateIfNotExists();
            serviceProvider.GetRequiredService<Table<LastDowntime>>().CreateIfNotExists();

            return serviceProvider;
        }
    }
}
