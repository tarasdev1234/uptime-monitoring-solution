using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Reliablesite.Authority.Model;
using System;

namespace Reliablesite.Authority.Authentication
{
    internal sealed class ConfigureOpenIdConnectOptions : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly AuthoritySettings authoritySettings;

        public ConfigureOpenIdConnectOptions(IOptions<AuthoritySettings> authorityOptions)
        {
            authoritySettings = authorityOptions.Value;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            if (name != AuthorityConstants.OpenIdConnectScheme)
            {
                return;
            }

            if (authoritySettings == null)
            {
                throw new ArgumentNullException("AuthoritySettings does not configured.");
            }

            options.Authority = authoritySettings.Url.ToString();
            options.RequireHttpsMetadata = true;

            options.ClientId = "mvc";
            options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.SaveTokens = true;

            options.Scope.Add("api");
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("roles");
            options.Scope.Add("email");
            options.Scope.Add("offline_access");

            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "role";

            options.ClaimActions.MapJsonKey("role", "role");

            options.GetClaimsFromUserInfoEndpoint = true;
        }

        public void Configure(OpenIdConnectOptions options) => Configure(Options.DefaultName, options);
    }
}
