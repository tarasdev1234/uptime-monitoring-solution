using System;
using System.Collections.Generic;
using System.Text;

namespace Uptime.Plugin.HealthChecks {
    public enum HealthStatus {
        Unhealthy = 0,
        Unknown = 1,
        Healthy = 2,
    }
}
