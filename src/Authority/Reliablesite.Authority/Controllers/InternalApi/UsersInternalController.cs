using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Authority.Models;

namespace Reliablesite.Authority.Controllers.InternalApi
{
    [AllowAnonymous]
    [Route("api/internal/users")]
    [ApiController]
    public class UsersInternalController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersInternalController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersCount()
        {
            var usersCount = await userManager.Users.CountAsync();

            return Ok(usersCount);
        }
    }
}