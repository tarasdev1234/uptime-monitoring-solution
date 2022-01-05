using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Uptime.Monitoring.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ConfigureUptimeMonitoringContext(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<UptimeMonitoringContext>(options => {
                options.UseSqlServer(connectionString, sqlOptions => {
                    sqlOptions.MigrationsAssembly(typeof(UptimeMonitoringContext).Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });
        }
    }
}
