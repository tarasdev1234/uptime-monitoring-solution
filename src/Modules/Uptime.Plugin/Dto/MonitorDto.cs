using System;
using System.Collections.Generic;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Plugin.Dto
{
    public sealed class MonitorDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public MonitorStatus Status { get; set; }

        public MonitorType Type { get; private set; }

        public IEnumerable<MonitoringSample> MonitoringHistory { get; set; } = Array.Empty<MonitoringSample>();
    }

    public sealed class MonitoringSample
    {
        public Guid EventId { get; set; }
        public DateTime From { get; set; }
        public MonitoringState State { get; set; }
    }

    public enum MonitoringState
    {
        Off = 0,
        Up = 1,
        Down = 2
    }
}
