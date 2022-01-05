// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Reliablesite.Authority.Data;
using Reliablesite.Authority.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServer4.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using IdentityServer4;
using Reliablesite.Authority.Model;
using Uptime.Notifications.Model.Messages;
using Messaging.Kafka;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace Reliablesite.Authority
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services
                .AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                });

#if DEBUG
            if (Environment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
#endif
            services
                .AddDataProtection()
                .PersistKeysToSqlStorage(Configuration.GetConnectionString("DefaultConnection"));

            services.AddControllersWithViews();

            services.AddDbContext<AuthorityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                if (Environment.IsDevelopment())
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                }
            })
                .AddEntityFrameworkStores<AuthorityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin += context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api") && 
                        (context.Response.StatusCode == 200 || context.Response.StatusCode == 302))
                    {
                        context.Response.StatusCode = 401;
                    }

                    return Task.CompletedTask;
                };

                options.Cookie.Path = "/";
            });

            services
                .AddAuthentication()
          
                .AddLocalApi();

            services.AddOptions()
                .Configure<KafkaSettings>(Configuration.GetSection(nameof(KafkaSettings)));

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.Authentication = new AuthenticationOptions()
                    {
                        CookieLifetime = TimeSpan.FromHours(10), // ID server cookie timeout set to 10 hours
                        CookieSlidingExpiration = true
                    };
                })
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))
                .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorityConstants.CookieOrJwtPolicy, policy =>
                {
                    policy.AuthenticationSchemes = new[] { IdentityConstants.ApplicationScheme, IdentityServerConstants.LocalApi.AuthenticationScheme };
                    policy.RequireAuthenticatedUser();
                });
            });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddDefaultServices(Configuration);

            services
                .ConfigureKafkaMessaging()
                .Queue<NotificationMsg>("Notifications");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var forwardedPrefix))
                {
                    context.Request.PathBase = new Microsoft.AspNetCore.Http.PathString(forwardedPrefix[0]);
                }

                return next();
            });

            if (Environment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseForwardedHeaders();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseDefaultServices();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}