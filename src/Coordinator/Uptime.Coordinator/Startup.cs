using Messaging.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Uptime.Coordinator.Data;
using Uptime.Coordinator.Web.JsonConverters;

namespace Uptime.Coordinator
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions()
                .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));

            services
                .AddCoordinatorDbContext(configuration)
                .AddServices();

            services
                .AddMvc()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new ConsulHealthStatusConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UptimeCoordinatorContext>();
                context.Database.Migrate();
            }

            app.UseRouting();
        }
    }
}
