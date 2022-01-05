using Admin.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using Uptime.Core;
using Admin.Web;
using Microsoft.AspNetCore.Antiforgery;

namespace Admin.Web.ViewComponents {
    public class ClientAppSettingsViewComponent : ViewComponent {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IAntiforgery _xsrf;

        public ClientAppSettingsViewComponent (IOptionsSnapshot<AppSettings> settings, IAntiforgery xsrf) {
            _settings = settings;
            _xsrf = xsrf;
        }

        public async Task<IViewComponentResult> InvokeAsync (int maxPriority, bool isDone) {
            var afToken = _xsrf.GetAndStoreTokens(HttpContext).RequestToken;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var apiUrl = _settings.Value.AdminApiUrl ?? "/";
            var authUrl = _settings.Value.IdentityServerUrl ?? "/";
            var appUrl = _settings.Value.AppUrl ?? "/";
            var clientUrl = _settings.Value.ClientUrl ?? "/";
            var email = HttpContext.User?.FindFirst(o => o.Type == "email")?.Value;
            var name = HttpContext.User?.FindFirst(o => o.Type == "name")?.Value;
            var hash = Hash.Md5(email).ToLower();

            var model = new ClientAppSettingsViewModel() {
                AntiForgeryToken = afToken,
                ApiUrl = apiUrl,
                AppUrl = appUrl,
                AuthUrl = authUrl,
                ClientPortalUrl = clientUrl,
                IdToken = idToken,
                AccessToken = accessToken,
                UserEmail = email,
                UserName = name,
                EmailHash = hash,
            };

            return View(model);
        }
    }
}
