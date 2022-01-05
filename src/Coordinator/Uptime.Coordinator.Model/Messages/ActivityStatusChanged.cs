using System;

namespace Uptime.Coordinator.Model.Messages
{
    public sealed class ActivityStatusChanged
    {
        public long MonitorId { get; set; }

        public ActivityStatus Status { get; set; }

        public Guid ExecutorId { get; set; }
    }
}
