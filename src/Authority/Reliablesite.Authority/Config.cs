// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace Reliablesite.Authority
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "roles",
                    Description = "User roles",
                    DisplayName = "Roles",
                    UserClaims = new [] { JwtClaimTypes.Role, ClaimTypes.Role },
                    Required = true,
                    Emphasize = true
                }
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api", new string[] { JwtClaimTypes.Role })
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    RequireConsent = false,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost/signin-oidc", "https://localhost/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost/signout-callback-oidc", "https://localhost/signin-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "roles", "email", "api" },
                }
            };
    }
}