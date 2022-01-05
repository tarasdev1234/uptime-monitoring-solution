using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Uptime.Data.Models.Identity;

namespace Users.Security {
    public class UptimePermissionHandler : AuthorizationHandler<PermissionRequirement> {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public UptimePermissionHandler (RoleManager<ApplicationRole> roleMgr, UserManager<ApplicationUser> usrMgr) {
            roleManager = roleMgr;
            userManager = usrMgr;
        }

        protected override async Task HandleRequirementAsync (AuthorizationHandlerContext context, PermissionRequirement requirement) {
            var user = await userManager.FindByIdAsync(context.User.FindFirst("sub")?.Value);

            if (user == null || !context.User.Identity.IsAuthenticated) {
                return;
            }
            
            var claims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var hasClaim = claims.Any(c => c.Type == Permission.ClaimType && c.Value == requirement.Permission.Name);

            if (hasClaim) {
                context.Succeed(requirement);
                return;
            }

            foreach (var roleName in roles) {
                var role = await roleManager.FindByNameAsync(roleName);

                if (role != null) {
                    foreach (var claim in await roleManager.GetClaimsAsync(role)) {
                        if (!String.Equals(claim.Type, Permission.ClaimType, StringComparison.OrdinalIgnoreCase)) {
                            continue;
                        }
                        
                        if (requirement.Permission.Name == claim.Value) {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
            }
        }
    }
}
