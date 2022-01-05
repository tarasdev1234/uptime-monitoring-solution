using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> roleMgr;
        private readonly UserManager<ApplicationUser> userMgr;
        private readonly AuthorityDbContext dbContext;

        public RolesController(RoleManager<ApplicationRole> roleMgr, UserManager<ApplicationUser> userMgr, AuthorityDbContext dbContext)
        {
            this.roleMgr = roleMgr;
            this.userMgr = userMgr;
            this.dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedCollection<ApplicationRole>), StatusCodes.Status200OK)]
        public IActionResult GetRoles([FromQuery]PagedQuery pagination, [FromQuery(Name = "s")] string search = "")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var query = roleMgr.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name.Contains(search));
            }

            var totalItems = query.LongCount();

            var itemsOnPage = query
                .OrderBy(c => c.Id)
                .Paged(pagination)
                .ToList();

            var model = new PagedCollection<ApplicationRole>(itemsOnPage, pagination.PageIndex.Value, pagination.PageSize.Value, totalItems);

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleAsync(long id)
        {
            var role = await roleMgr.FindByIdAsync(id.ToString());

            if (role == null)
            {
                return NotFound();
            }

            var permissions = (await roleMgr.GetClaimsAsync(role))
                .Where(claim => claim.Type == CustomClaimTypes.Permission)
                .Select(claim => claim.Value)
                .ToList();

            var vm = new RoleDetails()
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = permissions
            };

            return Ok(vm);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                if (await roleMgr.RoleExistsAsync(name))
                {
                    return Ok();
                }

                var role = new ApplicationRole(name);

                IdentityResult result = await roleMgr.CreateAsync(role);

                if (result.Succeeded)
                {
                    return CreatedAtAction(nameof(GetRoleAsync), new { id = role.Id }, null);
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
        public async Task<IActionResult> Update([FromBody]RoleDetails data)
        {
            var role = await dbContext.Roles
                            .Where(r => r.Id == data.Id)
                            .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound(new { Message = $"Role with id {data.Id} not found." });
            }

            var roleClaims = dbContext.RoleClaims.Where(x => x.RoleId == role.Id && x.ClaimType == CustomClaimTypes.Permission);

            role.Name = data.Name;

            dbContext.RoleClaims.RemoveRange(roleClaims);
            dbContext.RoleClaims.AddRange(data.Permissions.Select(p => new IdentityRoleClaim<long>()
            {
                ClaimType = CustomClaimTypes.Permission,
                ClaimValue = p,
                RoleId = data.Id
            }));

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoleAsync), new { id = role.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleMgr.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            await roleMgr.DeleteAsync(role);

            return NoContent();
        }

        [Route("{id}/addtorole/{userid}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToRole(string id, long userid)
        {
            var usr = await userMgr.Users.SingleOrDefaultAsync(u => u.Id == userid);
            var role = await roleMgr.FindByIdAsync(id);

            if (usr == null)
            {
                return NotFound(usr);
            }

            if (role == null)
            {
                return NotFound(role);
            }

            var result = await userMgr.AddToRoleAsync(usr, role.Name);

            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return BadRequest(ModelState);
        }
    }
}