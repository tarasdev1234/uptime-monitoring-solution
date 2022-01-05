using FluentAssertions;
using FluentAssertions.Equivalency;
using System;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Tests
{
    internal static class EquivalencyConfigurator
    {
        public static EquivalencyAssertionOptions<T> ForMonitoringEvent<T>(EquivalencyAssertionOptions<T> cfg) where T : MonitoringEvent
        {
            // 100 ms precision for comparing MonitoringEvent.Created field
            // Cassandra's datetime precision is lower then standtart DateTimeOffset precision
            return cfg
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100))
                .When(x => x.SelectedMemberPath == nameof(MonitoringEvent.Created) || x.SelectedMemberPath == nameof(MonitoringEvent.LastRepeat));
        }
    }
}
