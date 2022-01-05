using Helpers;
using Newtonsoft.Json;
using System;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Plugin.ViewModels.Monitor
{
    class MonitoringEventDto
    {
        public Guid Id { get; set; }

        public string MonitorName { get; set; }

        public EventType Type { get; set; }

        public string Description { get; set; }

        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTimeOffset Created { get; set; }

        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public TimeSpan? Duration { get; set; }
    }
}
