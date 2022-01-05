using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Web;

namespace Admin.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly IOptionsSnapshot<AppSettings> settings;

        public AdminController (IOptionsSnapshot<AppSettings> settings) {
            this.settings = settings;
        }

        public IActionResult Index () {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout () {
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var authUrl = settings.Value.IdentityServerUrl + "connect/endsession";

            var url = authUrl + "?id_token_hint=" + idToken + "&post_logout_redirect_uri=" + HttpUtility.UrlEncode(settings.Value.AppUrl);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(url);
        }

        public IActionResult Error () {
            return View();
        }
    }
}
