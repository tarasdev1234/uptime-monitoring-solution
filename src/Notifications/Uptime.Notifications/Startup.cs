using AutoMapper;
using Messaging.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Uptime.Notifications.Data;
using Uptime.Notifications.Model.Settings;

namespace Uptime.Notifications
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddServices()
                .AddWebServices()
                .AddEmailNotificationServices()
                .AddAutoMapper((sp, cfg) => cfg.AddProfiles(sp.GetServices<Profile>()), Array.Empty<Type>())
                .AddNotificationsDbContext(configuration);

            services.AddOptions()
                .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)))
                .Configure<SmtpSettings>(configuration.GetSection(nameof(SmtpSettings)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<UptimeNotificationsContext>();
                    context.Database.Migrate();
                }

                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
        }
    }
}
