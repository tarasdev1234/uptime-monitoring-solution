namespace Uptime.Monitoring.Model.Models
{
    public sealed class KeywordMonitor : Monitor
    {
        public bool ContainsWord { get; set; }

        public string Keyword { get; set; }

        public KeywordMonitor() : base(MonitorType.KEYWORD)
        {
        }
    }
}
