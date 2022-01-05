using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Uptime.Coordinator.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCoordinatorDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];

            return services.AddDbContext<UptimeCoordinatorContext>(options => {
                options.UseSqlServer(connectionString, sqlOptions => {
                    sqlOptions.MigrationsAssembly(typeof(UptimeCoordinatorContext).Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });
        }
    }
}
