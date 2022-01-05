using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace UptimeWeb.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await HttpContext.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}