//using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Admin.Web.Controllers {
    //[Authorize]
    public class AccountController : Controller {
        //[Route("refresh")]
        //public async Task<IActionResult> RefreshToken () {
        //    var client = new DiscoveryClient(HttpContext.Request.Host.Host);
        //    client.Policy.RequireHttps = false;

        //    var disco = await client.GetAsync();

        //    if (disco.IsError) {
        //        return Json(disco.Error);
        //    }

        //    var tokenClient = new TokenClient(disco.TokenEndpoint, "mvcclient", "secret"); // TODO: move client id and secret to app settings
        //    var rt = await HttpContext.GetTokenAsync("refresh_token");
        //    var tokenResult = await tokenClient.RequestRefreshTokenAsync(rt);

        //    if (!tokenResult.IsError) {
        //        var old_id_token = await HttpContext.GetTokenAsync("id_token");
        //        var new_access_token = tokenResult.AccessToken;
        //        var new_refresh_token = tokenResult.RefreshToken;

        //        var tokens = new List<AuthenticationToken> {
        //            new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = old_id_token },
        //            new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = new_access_token },
        //            new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = new_refresh_token }
        //        };

        //        var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
        //        tokens.Add(new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) });

        //        var info = await HttpContext.AuthenticateAsync("Cookies");
        //        info.Properties.StoreTokens(tokens);
        //        await HttpContext.SignInAsync("Cookies", info.Principal, info.Properties);

        //        return Json(new {
        //            accessToken = new_access_token
        //        });
        //    }

        //    return Json(tokenResult.Error);
        //}
    }
}
