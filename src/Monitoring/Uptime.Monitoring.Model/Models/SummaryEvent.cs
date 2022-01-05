namespace Uptime.Monitoring.Model.Models
{
    public class SummaryEvent : MonitoringEvent
    {
        public int Hours { get; set; }

        public int Mins { get; set; }

        public override string ToString() {
            return $"{Type} [{Created}] [Monitor({MonitorId})] [{Hours} hours, {Mins} mins]";
        }

        public SummaryEvent()
        {
        }
    }
}
