using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using Reliablesite.Service.Model;
using Steeltoe.Discovery.Consul.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Reliablesite.Service.Core
{
    internal sealed class ConsulDiscoveryOptionsPostConfigure : IPostConfigureOptions<ConsulDiscoveryOptions>
    {
        private readonly IServerAddressesFeature addressFeature;
        private readonly ServiceSettings serviceSettings;
        private readonly ApiRoutesProvider apiRoutesProvider;

        public ConsulDiscoveryOptionsPostConfigure(IServer server, IOptions<ServiceSettings> serviceOptions, ApiRoutesProvider apiRoutesProvider)
        {
            addressFeature = server.Features.Get<IServerAddressesFeature>();
            serviceSettings = serviceOptions.Value;
            this.apiRoutesProvider = apiRoutesProvider;
        }

        public void PostConfigure(string name, ConsulDiscoveryOptions options)
        {
            if (options.Tags == null)
            {
                options.Tags = new List<string>();
            }
            options.Tags.Add("instanceid=" + serviceSettings.InstanceId.ToString("N"));

            if (!options.Tags.Any(tag => tag.StartsWith("traefik.http.routers")))
            {
                var apiRoutes = apiRoutesProvider.GetApiRoutePrefixes();

                if (apiRoutes.Count != 0)
                {
                    var prefixes = string.Join(",", apiRoutes.Select(x => "`" + x + "`"));
                    options.Tags.Add($"traefik.http.routers.{serviceSettings.Name}.rule=PathPrefix({prefixes})");
                }
            }

            if (options.Port == 0)
            {
                options.Port = GetServerPort();
            }

            if (string.IsNullOrEmpty(options.ServiceName))
            {
                options.ServiceName = serviceSettings.Name;
            }

            if (string.IsNullOrEmpty(options.InstanceId))
            {
                options.InstanceId = options.ServiceName + "-" + serviceSettings.InstanceId.ToString("N");
            }
        }

        private int GetServerPort()
        {
            var address = addressFeature.Addresses.FirstOrDefault();

            if (address != null)
            {
                return new Uri(address).Port;
            }

            return 0;
        }
    }
}
