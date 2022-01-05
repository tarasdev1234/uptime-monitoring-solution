namespace Uptime.Monitoring.Model.Models {
    public class SummaryEventInfo {
        public SummaryEvent SummaryEvent { get; set; }
        
        public bool RecoveredFromDown { get; set; } = false;

        public bool WentDown { get; set; } = false;
    }
}
