// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Reliablesite.Authority.Data;
using Reliablesite.Authority.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reliablesite.SqlXmlRepository;
using Microsoft.AspNetCore.DataProtection;

namespace Reliablesite.Authority
{
    public class SeedData
    {
        private static readonly string[] Roles = new[] { "Admin", "User", "Agent" };

        public static void EnsureSeedData(string connectionString, ILogger logger)
        {
            logger.LogInformation("Seeding database");

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AuthorityDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddDataProtection().PersistKeysToSqlStorage(connectionString);

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AuthorityDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var keysContext = scope.ServiceProvider.GetRequiredService<XmlRepositoryDbContext>();
                    keysContext.Database.Migrate();

                    var context = scope.ServiceProvider.GetRequiredService<AuthorityDbContext>();
                    context.Database.Migrate();

                    SeedRoles(scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>(), logger).Wait();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var admin = userMgr.FindByNameAsync("admin").Result;
                    if (admin == null)
                    {
                        admin = new ApplicationUser
                        {
                            UserName = "admin"
                        };
                        var result = userMgr.CreateAsync(admin, "Qwerty123").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddToRoleAsync(admin, "Admin").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(admin, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Admin Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Admin"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "admin@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        logger.LogInformation("Admin created");
                    }
                    else
                    {
                        logger.LogInformation("Admin already exists");
                    }

                    var demo = userMgr.FindByNameAsync("demo").Result;
                    if (demo == null)
                    {
                        demo = new ApplicationUser
                        {
                            UserName = "demo"
                        };
                        var result = userMgr.CreateAsync(demo, "Qwerty123").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(demo, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Demo User"),
                            new Claim(JwtClaimTypes.GivenName, "Demo"),
                            new Claim(JwtClaimTypes.FamilyName, "User"),
                            new Claim(JwtClaimTypes.Email, "demo@user.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        logger.LogInformation("Demo user created");
                    }
                    else
                    {
                        logger.LogInformation("Demo user already exists");
                    }
                }
            }

            logger.LogInformation("Seeding completed");
        }

        private static async Task SeedRoles(RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            logger.LogInformation("Creating roles");

            foreach (var roleName in Roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                    logger.LogInformation("Role '{role}' created", roleName);
                }
                else
                {
                    logger.LogInformation("Role '{role}' already exist", roleName);
                }
            }
        }
    }
}
