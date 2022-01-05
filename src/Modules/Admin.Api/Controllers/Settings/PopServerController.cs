using Admin.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Service.Model.Dto;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Support;
using Uptime.Mvc.Controllers;

namespace Admin.Api.Controllers.Support
{
    [Route("api/settings/email/pop")]
    [Authorize(Roles = "Admin")]
    public class PopServerController : BaseController {
        public PopServerController (ApplicationDbContext context) : base(context) {}
        
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<PopServer>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPopServers ([FromQuery]PagedQuery pagination, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.PopServers.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(s => s.ServerName.Contains(search));
            }

            var totalItems = await dbContext.PopServers.LongCountAsync();

            var srvs = await dbContext.PopServers
                .Include(p => p.Department)
                .Select(p => new PopServer() {
                    Id = p.Id,
                    DepartmentName = p.Department.Name,
                    ServerName = p.ServerName,
                    Login = p.Login,
                    ServerPort = p.ServerPort,
                    UpdateInterval = p.UpdateInterval,
                    EmailAddress = p.EmailAddress,
                    Enabled = p.Enabled,
                    Encryption = p.Encryption
                })
                .OrderBy(c => c.Id)
                .Paged(pagination)
                .ToListAsync();

            var model = new PaginatedItemsVm<PopServer>(pagination.PageIndex.Value, pagination.PageSize.Value, totalItems, srvs);

            return Ok(model);
        }
        
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatePopServer ([FromBody]PopServer data) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            data.PasswordEncrypted = data.Password;
            data.Password = null;

            dbContext.PopServers.Add(data);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServer), new { id = data.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetServer (long id) {
            var srv = await dbContext.PopServers.FindAsync(id);

            if (srv == null) {
                return NotFound();
            }

            return Ok(srv);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteServer (long id) {
            var srv = await dbContext.PopServers.SingleOrDefaultAsync(b => b.Id == id);

            if (srv == null) {
                return NotFound();
            }

            dbContext.PopServers.Remove(srv);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]PopServer data) {
            var exists = await dbContext.PopServers.AnyAsync(s => s.Id == data.Id);

            if (!exists) {
                return NotFound();
            }

            data.PasswordEncrypted = data.Password;
            data.Password = null;

            dbContext.Update(data);

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
