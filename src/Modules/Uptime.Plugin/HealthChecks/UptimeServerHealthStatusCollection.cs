using System;
using System.Collections.Generic;
using System.Text;

namespace Uptime.Plugin.HealthChecks {
    public class UptimeServerHealthStatusCollection {
        private Dictionary<long, HealthStatus> Reports = new Dictionary<long, HealthStatus>();

        public void Update (long id, HealthStatus status) {
            Reports[id] = status;
        }

        public HealthStatus Get (long id) {
            if (Reports.TryGetValue(id, out var status)) {
                return status;
            }

            return HealthStatus.Unknown;
        }

        public Dictionary<long, HealthStatus> GetAll () => Reports;
    }
}
