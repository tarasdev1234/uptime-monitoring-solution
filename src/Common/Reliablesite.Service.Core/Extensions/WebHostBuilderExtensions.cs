using NLog.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;
using Microsoft.Extensions.Configuration;
using Steeltoe.Consul.Client;
using System;
using Reliablesite.Service.Core.StartupFilters;

namespace Reliablesite.Service.Core
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseDefaults<TStartup>(this IWebHostBuilder host) where TStartup : class
        {
            host
                .ConfigureAppConfiguration(AddConsulConfigurationSource)
                .ConfigureServices((context, services) 
                    =>
                {
                    services.AddTransient<IStartupFilter, DefaultConfigurationFilter>();
                    services.AddDefaultServices(context.Configuration);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .UseStartup<TStartup>();

            return host;
        }

        private static void AddConsulConfigurationSource(WebHostBuilderContext context, IConfigurationBuilder builder)
        {
            var preconfig = builder.Build();

            var consultConfig = preconfig
                .GetSection(ConsulOptions.CONSUL_CONFIGURATION_PREFIX)
                .Get<ConsulOptions>();

            if (consultConfig != null)
            {
                var configKey = $"configs/{context.HostingEnvironment.ApplicationName}/{context.HostingEnvironment.EnvironmentName}";

                builder.AddConsul(
                    configKey.ToLowerInvariant(),
                    options => PopulateConsulConfigurationSourceOptions(options, consultConfig));
            }
        }

        private static void PopulateConsulConfigurationSourceOptions(IConsulConfigurationSource consulConfigurationSource, ConsulOptions consultConfig)
        {
            consulConfigurationSource.ConsulConfigurationOptions = client =>
            {
                client.Address = new Uri($"{consultConfig.Scheme}://{consultConfig.Host}:{consultConfig.Port}");
                client.Datacenter = consultConfig.Datacenter;
                client.Token = consultConfig.Token;
                client.WaitTime = ToTimeSpan(consultConfig.WaitTime);
            };

            consulConfigurationSource.Optional = true;
        }

        private static TimeSpan? ToTimeSpan(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
            {
                return null;
            }

            time = time.ToLower();

            if (time.EndsWith("ms"))
            {
                return TimeSpan.FromMilliseconds(int.Parse(time.Substring(0, time.Length - 2)));
            }

            if (time.EndsWith("s"))
            {
                return TimeSpan.FromSeconds(int.Parse(time.Substring(0, time.Length - 1)));
            }

            if (time.EndsWith("m"))
            {
                return TimeSpan.FromMinutes(int.Parse(time.Substring(0, time.Length - 1)));
            }

            if (time.EndsWith("h"))
            {
                return TimeSpan.FromHours(int.Parse(time.Substring(0, time.Length - 1)));
            }

            throw new InvalidOperationException("Incorrect format:" + time);
        }
    }
}
