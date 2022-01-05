using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Uptime.Data.Models.Identity;

namespace Users.Middlewares {
    public class UserEnabledMiddleware {
        private readonly RequestDelegate next;

        public UserEnabledMiddleware (RequestDelegate next) {
            this.next = next;
        }

        public async Task Invoke (HttpContext httpContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            if (!string.IsNullOrEmpty(httpContext.User.Identity.Name)) {
                var user = await userManager.FindByNameAsync(httpContext.User.Identity.Name);

                if (!user.IsEnabled) {
                    await signInManager.SignOutAsync();
                    httpContext.Response.Redirect("/");
                }
            }

            await next(httpContext);
        }
    }
}
