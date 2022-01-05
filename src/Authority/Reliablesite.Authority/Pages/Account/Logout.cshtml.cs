using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Reliablesite.Authority.Models;

namespace Reliablesite.Authority.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IEventService _events;

        public LogoutModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LogoutModel> logger,
            IEventService events)
        {
            _signInManager = signInManager;
            _logger = logger;
            _events = events;
        }

        public async Task<IActionResult> OnGet(bool interactive = true, string returnUrl = null)
        {
            if (!interactive && HttpContext.Request.Host.HasValue)
            {
                var referrer = HttpContext.Request.Headers["Referer"].LastOrDefault();

                if (new Uri(referrer).Host == HttpContext.Request.Host.Host)
                {
                    return await Logout(returnUrl);
                }
            }

            return Page();
        }

        public Task<IActionResult> OnPost(string returnUrl = null)
        {
            return Logout(returnUrl);
        }

        private async Task<IActionResult> Logout(string returnUrl = null)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
                _logger.LogInformation("User logged out.");
            }

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
