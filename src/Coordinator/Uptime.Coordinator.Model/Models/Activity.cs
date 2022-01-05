using System;

namespace Uptime.Coordinator.Model.Models
{
    public sealed class Activity
    {
        public long MonitorId { get; set; }

        public Guid ExecutorId { get; set; }

        public Guid CoordinatorId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public int RowVersion { get; set; }
    }
}
