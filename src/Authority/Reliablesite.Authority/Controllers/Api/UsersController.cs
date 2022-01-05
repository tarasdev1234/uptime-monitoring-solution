using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Authority.Data;
using Reliablesite.Authority.Model;
using Reliablesite.Authority.Models;
using Reliablesite.Authority.Models.Api;
using Reliablesite.Service.Model.Dto;

namespace Reliablesite.Authority.Controllers.Api
{
    [Authorize(Roles = "Admin", Policy = AuthorityConstants.CookieOrJwtPolicy)]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuthorityDbContext dbContext;

        public UsersController(UserManager<ApplicationUser> userManager, AuthorityDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        [HttpHead("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> IsExist(long id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                return Ok();
            }

            return NotFound("User with given id doesn't exist");
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedCollection<ApplicationUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "")
        {
            var query = userManager.Users;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Email.Contains(search));
            }

            var totalItems = await query.LongCountAsync();

            var itemsOnPage = await query
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            var model = new PagedCollection<ApplicationUser>(itemsOnPage, p.PageIndex.Value, p.PageSize.Value, totalItems);

            return Ok(model);
        }

        [Route("agents")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ApplicationUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAgents()
        {
            var roles = await dbContext.Roles
                .Where(r => r.NormalizedName == "Agent" || r.NormalizedName == "Admin")
                .Select(r => r.Id).ToListAsync();

            var users = await dbContext.UserRoles
                    .Where(ur => roles.Contains(ur.RoleId))
                    .Distinct()
                    .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => u)
                    .ToListAsync();

            var model = new PagedCollection<ApplicationUser>(users, 1, 1, 1);

            return Ok(model);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(long id)
        {
            //TODO: departments, company

            var usr = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (usr == null)
            {
                return NotFound();
            }

            var usrRoles = await userManager.GetRolesAsync(usr);
            var permissions = (await userManager.GetClaimsAsync(usr))
                .Where(claim => claim.Type == CustomClaimTypes.Permission)
                .Select(claim => claim.Value)
                .ToList();

            var response = new UserDetails()
            {
                Email = usr.Email,
                PhoneNumber = usr.PhoneNumber,
                Id = usr.Id,
                Roles = usrRoles.ToList(),
                Permissions = permissions
            };

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(NewUser newUser)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = newUser.Email,
                    UserName = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber
                };

                IdentityResult result = await userManager.CreateAsync(user, newUser.Password);

                if (result.Succeeded)
                {
                    return CreatedAtAction("GetUser", new { id = user.Id }, null);
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> UpdateUser([FromBody]UserDetails usrToUpdate)
        {
            var usr = await userManager.Users.FirstOrDefaultAsync(i => i.Id == usrToUpdate.Id);

            if (usr == null)
            {
                return NotFound(new { Message = $"User with id {usrToUpdate.Id} not found." });
            }

            usr.Email = usrToUpdate.Email;
            usr.PhoneNumber = usrToUpdate.PhoneNumber;
            usr.Signature = usrToUpdate.Signature;

            if (!string.IsNullOrEmpty(usrToUpdate.Password))
            {
                await userManager.RemovePasswordAsync(usr);
                await userManager.AddPasswordAsync(usr, usrToUpdate.Password);
            }

            var roles = await userManager.GetRolesAsync(usr);

            await userManager.RemoveFromRolesAsync(usr, roles);
            await userManager.AddToRolesAsync(usr, usrToUpdate.Roles);
            await userManager.UpdateAsync(usr);

            // set permissions
            var existingPermissions = (await userManager.GetClaimsAsync(usr))
                .Where(claim => claim.Type == CustomClaimTypes.Permission)
                .ToList();

            if (existingPermissions.Count > 0)
            {
                await userManager.RemoveClaimsAsync(usr, existingPermissions);
            }

            await userManager.AddClaimsAsync(usr, usrToUpdate.Permissions.Select(p => new Claim(CustomClaimTypes.Permission, p)).ToList());

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = usr.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var usr = await userManager.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (usr == null)
            {
                return NotFound();
            }

            await userManager.DeleteAsync(usr);

            return NoContent();
        }
    }
}