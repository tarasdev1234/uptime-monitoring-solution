using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reliablesite.Service.Core.Middleware;
using Steeltoe.Discovery.Client;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDefaultServices(this IApplicationBuilder applicationBuilder)
        {
            var hostLifetime = applicationBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            // We need to call UseDiscoveryClient after web server start
            // to be sure that IServerAddressesFeature contains
            // actual information about listening addresses
            // See also ConsulDiscoveryOptionsPostConfigure
            hostLifetime.ApplicationStarted.Register(() =>
            {
                applicationBuilder.UseDiscoveryClient();
            });

            return applicationBuilder
                .UseForwardedHeaders()
                .UseOpenApi()
                .UseSwaggerUi3()
                .UseEndpoints(cfg =>
                {
                    cfg.MapControllers();
                });
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder applicationBuilder)
        {
            var env = applicationBuilder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                applicationBuilder.UseMiddleware<RequestLoggingMiddleware>();
            }

            return applicationBuilder;
        }
    }
}
