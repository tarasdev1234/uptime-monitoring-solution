using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using StartupBase = OrchardCore.Modules.StartupBase;

namespace Uptime.Theme
{
    public class Startup : StartupBase {
        private readonly IHostingEnvironment env;

        public Startup (IHostingEnvironment env) {
            this.env = env;
            env.IsDevelopment();
        }

        public override void ConfigureServices (IServiceCollection services) {
        }

        public override void Configure (IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            //routes.MapAreaRoute(name: "Home", areaName: "Uptime.Theme", template: "/", defaults: new { controller = "Home", action = "Index" });
        }
    }
}
