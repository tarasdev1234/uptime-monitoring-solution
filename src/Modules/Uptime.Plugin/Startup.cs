using AutoMapper;
using Messaging.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.BackgroundTasks;
using OrchardCore.Modules;
using Polly;
using Polly.Extensions.Http;
using Reliablesite.Authority.Model;
using System;
using System.Net.Http;
using Uptime.Monitoring.Data;
using Uptime.Plugin.BackgroundTasks.ServersHealth;
using Uptime.Plugin.HealthChecks;
using Uptime.Plugin.Services;

namespace Uptime.Plugin
{
    public class Startup : StartupBase {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment env;
        private readonly IConfiguration config;

        public Startup (Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration config) {
            this.config = config;
            this.env = env;
        }

        public override void ConfigureServices (IServiceCollection services) {

            services
                .AddOptions()
                .Configure<KafkaSettings>(config.GetSection(nameof(KafkaSettings)));

            services.AddEventsServices(config.GetConnectionString("Events"));
            services.ConfigureUptimeMonitoringContext(config.GetConnectionString("Monitoring"));

            services.AddScoped<UptimeServerHealthChecker>();

            services.AddSingleton<UptimeServerHealthStatusCollection>();
            services.AddSingleton<IBackgroundTask, UptimeServerHealthCheckTask>();

            services.AddMessagingServices();

            //services.AddSingleton<IMonitoringServerService, MonitoringServerService>();

            services.AddScoped<IMonitoringTaskService, MonitoringTaskService>();
            services.AddTransient<IStatisticsService, StatisticsService>();

            services.AddHttpClient<IMonitoringServerService, MonitoringServerService>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddAutoMapper(typeof(WebDtoMapperProfile));

            services.AddAuthorityClient();
        }

        public override void Configure (IApplicationBuilder app, IEndpointRouteBuilder router, IServiceProvider serviceProvider) {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {
                scope.ServiceProvider.GetRequiredService<UptimeMonitoringContext>().Database.Migrate();
            }

            router.MapAreaControllerRoute(name: "UptimeHome", areaName: "Uptime.Plugin", pattern: "/", defaults: new { controller = "Home", action = "Index" });
            router.MapAreaControllerRoute(name: "UptimeAbout", areaName: "Uptime.Plugin", pattern: "/about", defaults: new { controller = "Home", action = "About" });
            router.MapAreaControllerRoute(name: "UptimeFaq", areaName: "Uptime.Plugin", pattern: "/faq", defaults: new { controller = "Home", action = "Faq" });
            router.MapAreaControllerRoute(name: "UptimeSupport", areaName: "Uptime.Plugin", pattern: "/support", defaults: new { controller = "Home", action = "Support" });

            router.MapAreaControllerRoute(name: "Dashboard", areaName: "Uptime.Plugin", pattern: "/dashboard", defaults: new { controller = "Dashboard", action = "Index" });
            router.MapAreaControllerRoute(name: "DashboardSettings", areaName: "Uptime.Plugin", pattern: "/dashboard/settings", defaults: new { controller = "Dashboard", action = "Settings" });
            router.MapAreaControllerRoute(name: "SupportTicket", areaName: "Uptime.Plugin", pattern: "/dashboard/support", defaults: new { Controller = "SupportTicket", action = "support" });
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy () {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy () {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
