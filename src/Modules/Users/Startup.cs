using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using System;
using Uptime.Data;
using Uptime.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Users.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Users.Middlewares;
using System.Threading.Tasks;

namespace Users {
    public class Startup : StartupBase {
        private const string LoginPath = "id/login";
        private const string ChangePasswordPath = "id/resetpassword";

        private readonly string _tenantName;
        private readonly string _tenantPrefix;
        //private readonly string _connStr;
        private readonly IConfiguration _config;

        public Startup (ShellSettings shellSettings, IConfiguration config) {
            _tenantName = shellSettings.Name;
            _tenantPrefix = "/" + shellSettings.RequestUrlPrefix;
            //_connStr = shellSettings.ConnectionString;
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public override void ConfigureServices (IServiceCollection services) {
            var connectionString = _config["ConnectionString"];

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddSignInManager<UptimeSignInManager>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = "auth_" + _tenantName;
                options.Cookie.Path = _tenantPrefix;
                options.LoginPath = "/" + LoginPath;
                options.AccessDeniedPath = options.LoginPath;
                options.Events.OnRedirectToAccessDenied = context => {
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == StatusCodes.Status200OK) {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);

                    return Task.CompletedTask;
                };
            });

            services.AddAuthentication(options => options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme)
            .AddGoogle("Google", options => {
                options.ClientId = "336128599689-qndn9uq83vhs2p7g9qlfo4q6sstj6m80.apps.googleusercontent.com";
                options.ClientSecret = "UznKY6VYambNMFeUt22XoBVd";
            });
            services.AddAuthorization();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddScoped<IAuthorizationHandler, UptimePermissionHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure (IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {
            app.UseMiddleware<UserEnabledMiddleware>();
            app.UseAuthorization();
            
            // add default user
            //using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
            //    var ctx = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            //    var userMgr = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            //    var roleMgr = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                
            //    new ApplicationDbContextSeed()
            //        .AddDefaultUser(ctx,userMgr, roleMgr)
            //        .Wait();
            //}

            // Setup Databases
            //using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
            //    serviceScope.ServiceProvider.GetService<ConfigurationDbContext>().Database.Migrate();
            //    serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
            //}

            // account
            routes.MapAreaControllerRoute(name: "Login", areaName: "Users", pattern: "id/login", defaults: new { controller = "Account", action = "Login" });
            routes.MapAreaControllerRoute(name: "Logout", areaName: "Users", pattern: "id/logout", defaults: new { controller = "Account", action = "Logout" });
            routes.MapAreaControllerRoute(name: "LoginWith2fa", areaName: "Users", pattern: "id/2fa", defaults: new { controller = "Account", action = "LoginWith2fa" });
            routes.MapAreaControllerRoute(name: "Lockout", areaName: "Users", pattern: "id/lockout", defaults: new { controller = "Account", action = "Lockout" });
            routes.MapAreaControllerRoute(name: "ExternalLogin", areaName: "Users", pattern: "id/external", defaults: new { controller = "Account", action = "ExternalLogin" });
            routes.MapAreaControllerRoute(name: "ExternalLoginCallback", areaName: "Users", pattern: "id/externalcallback", defaults: new { controller = "Account", action = "ExternalLoginCallback" });
            routes.MapAreaControllerRoute(name: "Register", areaName: "Users", pattern: "id/register", defaults: new { controller = "Account", action = "Register" });
            routes.MapAreaControllerRoute(name: "ConfirmEmail", areaName: "Users", pattern: "id/confirm", defaults: new { controller = "Account", action = "ConfirmEmail" });
            routes.MapAreaControllerRoute(name: "ForgotPassword", areaName: "Users", pattern: "id/forgot", defaults: new { controller = "Account", action = "ForgotPassword" });
            routes.MapAreaControllerRoute(name: "ForgotPasswordConfirmation", areaName: "Users", pattern: "id/forgotconfirm", defaults: new { controller = "Account", action = "ForgotPasswordConfirmation" });
            routes.MapAreaControllerRoute(name: "ResetPasswordConfirmation", areaName: "Users", pattern: "id/resetpasswordconfirmation", defaults: new { controller = "Account", action = "ResetPasswordConfirmation" });
            routes.MapAreaControllerRoute(name: "ResetPassword", areaName: "Users", pattern: "id/resetpassword", defaults: new { controller = "Account", action = "ResetPassword" });

            // manage
            // use attribute routes
            routes.MapAreaControllerRoute(name: "IdManage", areaName: "Users", pattern: "id/manage", defaults: new { controller = "Manage", action = "Index" });
        }
    }
}
