using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Reliablesite.Service.Model;

namespace Reliablesite.Service.Core
{
    internal sealed class ServiceOptionsPostConfigure : IPostConfigureOptions<ServiceSettings>
    {
        private readonly IHostEnvironment environment;

        public ServiceOptionsPostConfigure(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        public void PostConfigure(string name, ServiceSettings options)
        {
            if (string.IsNullOrEmpty(options.Name))
            {
                options.Name = environment.ApplicationName.ToLowerInvariant();
            }
        }
    }
}
