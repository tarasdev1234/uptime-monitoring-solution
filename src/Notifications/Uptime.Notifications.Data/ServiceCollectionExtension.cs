using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Uptime.Notifications.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationsDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Templates");

            return serviceCollection.AddDbContext<UptimeNotificationsContext>(options => {
                options.UseSqlServer(connectionString, sqlOptions => {
                    sqlOptions.MigrationsAssembly(typeof(UptimeNotificationsContext).Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });
        }
    }
}
