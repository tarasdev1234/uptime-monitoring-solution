using System;

namespace Uptime.Monitoring.Model.Models
{
    public abstract class MonitorTask
    {
        public long MonitorId { get; set; }

        public long UserId { get; set; }

        public string Target { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Id of event to confirm.
        /// Not null value indicates that
        /// task goal is to confirm other task result.
        /// </summary>
        public Guid? EventIdToConfirm { get; set; }

        /// <summary>
        /// Indicates whether task goal is to confirm
        /// results of other task or not.
        /// </summary>
        public bool IsConfirmation => EventIdToConfirm.HasValue;
    }

    public class HttpMonitorTask : MonitorTask
    {
    }

    public class PingMonitorTask : MonitorTask
    {
    }

    public class KeywordMonitorTask : MonitorTask
    {
        public string Keyword { get; set; }
        public bool ShouldContain { get; set; }
    }

    public class TcpMonitorTask : MonitorTask
    {
        public int Port { get; set; }
    }
}
