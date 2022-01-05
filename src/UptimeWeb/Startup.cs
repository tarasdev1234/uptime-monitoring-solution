using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using ModularTest.Services;
using OrchardCore.DisplayManagement.Theming;
using OrchardCore.Environment.Shell;
using OrchardCore.Override;
using OrchardCore.ResourceManagement.TagHelpers;
using OrchardCore.Settings;
using OrchardCore.Themes.Services;
using Reliablesite.Authority.Model;
using Reliablesite.SqlXmlRepository;
using Uptime.Data;
using Uptime.Mvc.Services;

namespace ModularTest
{
    public class Startup {
        public Startup (IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices (IServiceCollection services) {
            var connectionString = Configuration.GetConnectionString("Uptime");

            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(connectionString, sqlOptions => {
                            sqlOptions.MigrationsAssembly("Uptime.Data");
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), new List<int>());
                        });
            });

            services
                .AddDataProtection()
                .PersistKeysToSqlStorage(connectionString);

            services.AddMemoryCache();

            services.Configure<AuthoritySettings>(Configuration.GetSection(nameof(AuthoritySettings)));
            //services.AddSingleton<IShellSettingsConfigurationProvider, ShellSettingsConfigurationProvider>();
            services.AddSingleton<IShellSettingsManager, ShellSettingsManager>();
            services.AddTransient<IConfigureOptions<ShellOptions>, ShellOptionsSetup>();

            var builder = services.AddOrchardCore()
                .AddCommands()

                .AddMvc()
                
                .AddShellConfiguration()
                
                .AddBackgroundService()

                .AddTheming()
                .AddCaching()
                
                .ConfigureServices(s => {
                    s.AddScoped<BrandContext>();

                    s.AddScoped<ISiteThemeService, Services.SiteThemeService>();
                    s.AddSingleton<ISiteService, SiteService>();
                    s.AddScoped<IThemeSelector, Services.SiteThemeSelector>();

                    s.AddAuthorityAuthentication();
                    s.AddAuthorization();

                    s.Configure<ForwardedHeadersOptions>(options =>
                    {
                        options.ForwardedHeaders =
                            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                        options.KnownNetworks.Clear();
                        options.KnownProxies.Clear();
                    });

                    //s.AddScoped<ISiteThemeService, SiteThemeService>();
                })
                .ConfigureServices(s => {
                    s.AddResourceManagement();

                    s.AddTagHelpers<LinkTagHelper>();
                    s.AddTagHelpers<MetaTagHelper>();
                    s.AddTagHelpers<ResourcesTagHelper>();
                    s.AddTagHelpers<ScriptTagHelper>();
                    s.AddTagHelpers<StyleTagHelper>();
                })
                .Configure(app =>
                {
                    app.UseForwardedHeaders();

                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseStaticFiles();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            Migrate(app);
            
            if (env.IsDevelopment()) {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseOrchardCore();
        }

        public void Migrate(IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                var keyContext = serviceScope.ServiceProvider.GetRequiredService<XmlRepositoryDbContext>();
                keyContext.Database.Migrate();

                var ctx = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                
                ctx.Database.Migrate();
                
                new ApplicationDbContextSeed()
                    .SeedAsync(ctx,"Default", "localhost")
                    .Wait();
            }
        }
    }
}
