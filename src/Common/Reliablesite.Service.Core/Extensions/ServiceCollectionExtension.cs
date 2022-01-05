using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Reliablesite.Service.Core;
using Reliablesite.Service.Core.Extensions;
using Reliablesite.Service.Model;
using Steeltoe.Consul.Client;
using Steeltoe.Discovery.Client;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultServices(this IServiceCollection services, IConfiguration configuration)
        {
            var startAssembly = Assembly.GetEntryAssembly();

            var consulConfig = new ConsulOptions();
            configuration.Bind(ConsulOptions.CONSUL_CONFIGURATION_PREFIX, consulConfig);

            services
                .AddOptions()
                .AddOptions<ServiceSettings>()
                    .Bind(configuration.GetSection("Service"))
                    .ValidateDataAnnotations()
                    .ValidateEagerly();

            services
                .ConfigureOptions<ServiceOptionsPostConfigure>()
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                }); ;

            services
                .AddHealthChecks()
                .AddConsul(options =>
                {
                    options.HostName = consulConfig.Host;
                    options.Port = consulConfig.Port;
                    options.RequireHttps = consulConfig.Scheme == "https";

                    if (!string.IsNullOrEmpty(consulConfig.Username))
                    {
                        options.RequireBasicAuthentication = true;
                        options.Username = consulConfig.Username;
                        options.Password = consulConfig.Password;
                    }
                });

            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonTimeSpanConverter());
                });

            return services
                .AddOpenApiDocument(cfg =>
                {
                    cfg.Title = startAssembly.GetName().Name;
                    cfg.DocumentName = "v0";
                })
                .AddServiceDiscovery(configuration);
        }

        private static IServiceCollection AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddTransient<ApiRoutesProvider>()
                .ConfigureOptions<ConsulDiscoveryOptionsPostConfigure>()
                .AddDiscoveryClient(configuration);
        }
    }
}
