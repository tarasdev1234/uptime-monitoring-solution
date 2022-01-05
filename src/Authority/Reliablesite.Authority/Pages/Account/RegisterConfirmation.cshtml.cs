using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Reliablesite.Authority.Models;
using Microsoft.AspNetCore.Identity;
using Messaging;
using Uptime.Notifications.Model.Messages;

namespace Reliablesite.Authority.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProducer<NotificationMsg> _notificationsSender;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager,
            IProducer<NotificationMsg> notificationsSender)
        {
            _userManager = userManager;
            _notificationsSender = notificationsSender;
        }

        public string Email { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            return Page();
        }
    }
}
