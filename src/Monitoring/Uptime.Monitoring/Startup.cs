using System;
using Messaging.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Uptime.Coordinator.Client;
using Uptime.Monitoring.Cassandra.Extensions;
using Uptime.Monitoring.Data;
using Uptime.Monitoring.Web.Extensions;

namespace Uptime.Monitoring
{
    public class Startup
    {
        private IConfiguration configuration;


        public Startup (IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        
        public void ConfigureServices (IServiceCollection services)
        {

            services.AddOptions();
            
            services.Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));
            services.Configure<CoordinatorClientSettings>(configuration.GetSection(nameof(CoordinatorClientSettings)));

            services.AddEventsServices(configuration.GetConnectionString("Events"));
            services.ConfigureUptimeMonitoringContext(configuration.GetConnectionString("Monitoring"));

            services.AddMonitoringServices();
            services.AddMessagingServices();
            services.AddCoordinatorClient();

            services.AddMemoryCache();
        }
        
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment()) {
                serviceProvider.CreateCassandraDb();

                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<UptimeMonitoringContext>();
                    context.Database.Migrate();
                }

                app.UseDeveloperExceptionPage();
            }

            app
                .ConfigureWeb()
                .UseRouting();
        }
    }
}
