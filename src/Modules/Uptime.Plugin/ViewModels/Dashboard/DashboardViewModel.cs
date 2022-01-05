using System;
using System.Collections.Generic;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Plugin.ViewModels.Dashboard
{
    public class DashboardViewModel {
        public int TotalMons { get; set; }

        public int ActiveMons { get; set; }

        public int PausedMons { get; set; }

        public int DownMons { get; set; }

        public int UpMons { get; set; }

        public AvgUptime AvgUptime { get; set; }

        public LastDowntime LatestDowntime { get; set; }

        public string LatestDowntimeMonitor { get; set; }

        public Dictionary<int, string> MonitorTypes { get; set; }
    }
}
