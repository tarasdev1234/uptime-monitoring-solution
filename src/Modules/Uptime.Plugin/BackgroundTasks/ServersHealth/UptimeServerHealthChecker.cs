using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Monitoring.Data;
using Uptime.Monitoring.Model.Models;
using Uptime.Plugin.HealthChecks;

namespace Uptime.Plugin.BackgroundTasks.ServersHealth {
    public class UptimeServerHealthChecker {
        private readonly ILogger<UptimeServerHealthChecker> logger;
        private readonly UptimeMonitoringContext dbContext;
        private readonly UptimeServerHealthStatusCollection healthDb;

        private readonly HttpClient httpClient;

        public UptimeServerHealthChecker (ILogger<UptimeServerHealthChecker> logger,
            UptimeMonitoringContext dbContext,
            UptimeServerHealthStatusCollection healthDb,
            IHttpClientFactory httpClientFactory) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.healthDb = healthDb ?? throw new ArgumentNullException(nameof(healthDb));

            httpClient = httpClientFactory.CreateClient("health-checks");
        }

        public async Task CheckAsync () {
            using (logger.BeginScope("UptimeServerHealthChecker is collecting health checks results.")) {
                var servers = await dbContext.MonitoringServers.ToListAsync();

                foreach (var srv in servers) {
                    var health = await GetHealthReport(srv);

                    healthDb.Update(srv.Id, health);
                }

                logger.LogDebug("UptimeServerHealthChecker has completed.");
            }
        }

        private async Task<HealthStatus> GetHealthReport (MonitoringServer srv) {
            try {
                var response = await httpClient.GetAsync(srv.Host + "/hc");

                return response.IsSuccessStatusCode
                    ? HealthStatus.Healthy
                    : HealthStatus.Unhealthy;
            } catch (Exception exception) {
                logger.LogError(exception, $"GetHealthReport threw an exception when trying to get report from {srv.Host}.");

                return HealthStatus.Unhealthy;
            }
        }
    }
}
