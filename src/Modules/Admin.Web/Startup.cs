using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;
using System;
using StartupBase = OrchardCore.Modules.StartupBase;

namespace Admin.Web {
    public class Startup : StartupBase {
        private const string CacheKey = "CurrentThemeName";
        private readonly IWebHostEnvironment env;

        public Startup (IWebHostEnvironment env, IMemoryCache memoryCache) {
            //memoryCache.Set(CacheKey, "Uptime.Theme");
            //memoryCache.Set("CurrentAdminThemeName", "Uptime.Theme");
            this.env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public override void ConfigureServices (IServiceCollection services) {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure (IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            if (env.IsDevelopment()) {
//                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
//                    ProjectPath = @"C:\UptimeWeb\Admin.Web",
//                    HotModuleReplacement = true
//                });
            }

            routes.MapAreaControllerRoute(name: "AdminSpa", areaName: "Admin.Web", pattern: "admin", defaults: new { controller = "Admin", action = "Index" });
            
            routes.MapFallbackToAreaController(
                pattern: "admin",
                action: "index",
                controller: "Admin",
                area: "Admin.Web"
            );
        }
    }
}
