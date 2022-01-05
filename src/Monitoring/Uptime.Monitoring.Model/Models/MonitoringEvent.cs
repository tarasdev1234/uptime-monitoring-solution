using System;
using System.Collections.Generic;

namespace Uptime.Monitoring.Model.Models
{
    public class MonitoringEvent
    {
        public Guid Id { get; set; }

        public long UserId { get; set; }

        public long MonitorId { get; set; }

        public EventType Type { get; set; }

        public EventType? PreviousEventType { get; set; }

        public long Repeats { get; set; }

        public DateTime? LastRepeat { get; set; }

        public DateTime Created { get; set; }

        public Guid? SourceEventId { get; set; }

        public MonitoringEventDetails Details { get; set; } = new MonitoringEventDetails();

        public MonitoringEvent()
        {
        }

        public MonitoringEvent(MonitoringEvent @event)
        {
            Id = @event.Id;
            UserId = @event.UserId;
            MonitorId = @event.MonitorId;
            Type = @event.Type;
            PreviousEventType = @event.PreviousEventType;
            Repeats = @event.Repeats;
            LastRepeat = @event.LastRepeat;
            Created = @event.Created;
            SourceEventId = @event.SourceEventId;
            Details = new MonitoringEventDetails(@event.Details);
        }

        public static MonitoringEvent CreateFrom(MonitoringEvent @event, Dictionary<string, string> details)
        {
            return new MonitoringEvent
            {
                UserId = @event.UserId,
                Type = @event.Type,
                PreviousEventType = @event.PreviousEventType,
                MonitorId = @event.MonitorId,
                SourceEventId = @event.SourceEventId,
                Details = details != null ? new MonitoringEventDetails(details) : new MonitoringEventDetails(),
            };
        }
    }
}
