using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Reliablesite.Authority.Authentication;
using Reliablesite.Authority.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddAuthorityAuthentication(this IServiceCollection serviceCollection)
        {
            // Prevent claims from mapping to windows proprietary claim types
            // e.g. "role" to "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            // you can read more here: https://leastprivilege.com/2016/08/21/why-does-my-authorize-attribute-not-work/
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            serviceCollection.AddAccessTokenManagement(options =>
            {
            });

            return serviceCollection
                .ConfigureOptions<ConfigureOpenIdConnectOptions>()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = AuthorityConstants.OpenIdConnectScheme;
                })
                .AddCookie(options =>
                {
                    // Prevent redirecting to login page unauthorizaed API methods calls
                    options.Events.OnRedirectToLogin += SupressApiRedirectToLogin;
                })
                .AddOpenIdConnect(AuthorityConstants.OpenIdConnectScheme, null);
        }

        private static Task SupressApiRedirectToLogin<T>(RedirectContext<T> context) where T: AuthenticationSchemeOptions
        {
            if (context.Request.Path.StartsWithSegments("/api") &&
                            (context.Response.StatusCode == 200 || context.Response.StatusCode == 302))
            {
                context.Response.StatusCode = 401;
            }

            return Task.CompletedTask;
        }
    }
}
