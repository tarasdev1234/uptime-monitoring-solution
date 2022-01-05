using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Uptime.Monitoring.Cassandra;
using Uptime.Monitoring.Cassandra.Repositories;
using Uptime.Monitoring.Cassandra.Services;
using Uptime.Monitoring.Model.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class ServiceCollectionExtension {

        public static IServiceCollection AddEventsServices(this IServiceCollection services, string connectionString) {
            return services
                .AddCassandraConnection(connectionString)
                .AddScoped<EventsRepository>()
                .AddScoped<RecentMonitoringEventsRepository>()
                .AddScoped<LastDowntimesRepository>()
                .AddScoped<IEventsService, EventService>();
        }

        private static IServiceCollection AddCassandraConnection(this IServiceCollection services, string connectionString)
        {
            return services
                .AddSingleton(new MappingConfiguration()
                    .Define<EntityMappings>()
                    .ConvertTypesUsing(new CustomTypeConverter()))
                .AddSingleton(sp => {
                    var cluster = Cluster.Builder()
                        .WithConnectionString(connectionString)
                        .Build();

                    var session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();

                    return session;
                })
                .AddScoped(typeof(Table<>));
        }
    }

}