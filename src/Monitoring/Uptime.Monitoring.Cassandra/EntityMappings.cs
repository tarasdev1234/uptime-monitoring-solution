using Cassandra;
using Cassandra.Mapping;
using System.Collections.Generic;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra
{

    internal sealed class EntityMappings : Mappings
    {
        public EntityMappings()
        {
            For<MonitoringEvent>()
                .TableName("events")
                .PartitionKey(x => x.UserId, x => x.MonitorId)
                .ClusteringKey(x => x.Id, SortOrder.Descending)
                .Column(x => x.Id, cm => cm.WithDbType<TimeUuid>())
                .Column(x => x.Type, cm => cm.WithDbType<int>())
                .Column(x => x.PreviousEventType, cm => cm.WithDbType<int?>())
                .Column(x => x.Details, cm => cm.WithDbType<Dictionary<string, string>>()); ;

            For<RecentMonitoringEvent>()
                .TableName("recent_events")
                .PartitionKey(x => x.UserId)
                .ClusteringKey(x => x.Id, SortOrder.Descending)
                .Column(x => x.Id, cm => cm.WithDbType<TimeUuid>())
                .Column(x => x.Type, cm => cm.WithDbType<int>())
                .Column(x => x.PreviousEventType, cm => cm.WithDbType<int?>())
                .Column(x => x.Details, cm => cm.WithDbType<Dictionary<string, string>>());

            For<LastDowntime>()
                .TableName("last_downtimes")
                .PartitionKey(x => x.UserId);
        }

    }

}