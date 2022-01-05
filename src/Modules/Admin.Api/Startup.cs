using Admin.Api.EventHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;
using Uptime.Data.Services;
using Uptime.Events.EventHandlers.Tickets;
using Uptime.Mvc.Services;
using Admin.Api.Constants;
using Admin.Api.Middlewares;
using Users.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Uptime.Data;
using Uptime.Data.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Authority.Client;
using Reliablesite.Authority.Model;

namespace Admin.Api {
    public class Startup : StartupBase {
        public override int Order { get; } = 0;

        public IConfiguration Configuration;

        public Startup (IConfiguration config) {
            Configuration = config;
        }

        public override void ConfigureServices (IServiceCollection services) {
            //services.AddSwaggerGen(c => {
            //    c.SwaggerDoc("v1", new Info { Title = "Admin API", Version = "v1" });
            //});

            services.AddScoped<IPermissionProvider, Permissions>();

            services.AddScoped<ITicketEventsHandler, TicketEventsHandler>();

            services.AddScoped<ProductService>();

            services.AddAuthorityClient();

            //services.AddAuthorization(options => {
            //    options.AddPolicy(Constants.Permissions.READ_TICKETS, pb => {
            //        pb.AuthenticationSchemes.Add(IdentityConstants.ApplicationScheme);
            //        pb.Requirements.Add(new PermissionRequirement(Permissions.ReadTickets));
            //    });
            //    options.AddPolicy(Constants.Permissions.DELETE_TICKETS, pb => {
            //        pb.AuthenticationSchemes.Add(IdentityConstants.ApplicationScheme);
            //        pb.Requirements.Add(new PermissionRequirement(Permissions.DeleteTickets));
            //    });
            //    options.AddPolicy(Constants.Permissions.READ_ARTICLES, pb => {
            //        pb.AuthenticationSchemes.Add(IdentityConstants.ApplicationScheme);
            //        pb.Requirements.Add(new PermissionRequirement(Permissions.ReadArticles));
            //    });
            //    options.AddPolicy(Constants.Permissions.DELETE_ARTICLES, pb => {
            //        pb.AuthenticationSchemes.Add(IdentityConstants.ApplicationScheme);
            //        pb.Requirements.Add(new PermissionRequirement(Permissions.DeleteArticles));
            //    });
            //});

            //services.Replace(new ServiceDescriptor(typeof(IPluginManager), typeof(DbPluginManager), ServiceLifetime.Scoped));
        }

        public override void Configure (IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api"), ab => ab.UseMiddleware<ApiExceptionsHandlerMiddleware>());

            //router.MapAreaRoute(name: "Brands", areaName: "Admin.Api", template: "/api/brand", defaults: new { controller = "Brand", action = "GetBrands" });


            //app.UseSwagger();

            //app.UseSwaggerUI(c => {
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API V1");
            //});
        }
    }
}
