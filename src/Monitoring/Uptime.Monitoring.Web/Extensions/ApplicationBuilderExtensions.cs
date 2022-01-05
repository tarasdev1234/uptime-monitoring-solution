using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Uptime.Monitoring.Auth;

namespace Uptime.Monitoring.Web.Extensions {
    public static class ApplicationBuilderExtensions {
        public static IApplicationBuilder ConfigureWeb(this IApplicationBuilder applicationBuilder) {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return applicationBuilder
                .UseMiddleware<ApiKeyMiddleware>();
        }
    }
}