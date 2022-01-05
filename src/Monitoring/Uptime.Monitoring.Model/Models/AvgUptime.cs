namespace Uptime.Monitoring.Model.Models
{
    public class AvgUptime
    {
        public double LastDay { get; set; } = 0;

        public double LastWeek { get; set; } = 0;

        public double LastMonth { get; set; } = 0;
    }
}
