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

namespace Admin.Api.Controllers.Settings
{
    [Route("api/settings/email/smtp")]
    [Authorize(Roles = "Admin")]
    public class SmtpServerController : BaseController {
        public SmtpServerController (ApplicationDbContext context) : base(context) {}

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<SmtpServer>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetServers ([FromQuery]PagedQuery pagination, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.SmtpServers.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(s => s.Name.Contains(search));
            }

            var totalItems = await query.LongCountAsync();

            var srvs = await query
                .OrderBy(c => c.Id)
                .Paged(pagination)
                .ToListAsync();

            var model = new PaginatedItemsVm<SmtpServer>(pagination.PageIndex.Value, pagination.PageSize.Value, totalItems, srvs);

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateServer ([FromBody]SmtpServer data) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var srv = data;

            if (srv.Default) {
                await dbContext.SmtpServers.ForEachAsync(s => s.Default = false);
            }

            srv.PasswordEncrypted = data.Password;
            srv.Password = null;

            //var srv = new SmtpServer() {
            //    ServerName = data.ServerName,
            //    ServerPort = data.ServerPort,
            //    Login = data.Login,
            //    Password = data.Password,
            //    Name = data.Name,
            //    EmailAddress = data.EmailAddress,
            //    Encryption = data.Encryption,
            //};

            dbContext.SmtpServers.Add(srv);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServer), new { id = srv.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetServer (long id) {
            var srv = await dbContext.SmtpServers.FindAsync(id);

            if (srv == null) {
                return NotFound();
            }

            return Ok(srv);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var srv = await dbContext.SmtpServers.FindAsync(id);

            if (srv == null) {
                return NotFound();
            }

            if (srv.Default) {
                ModelState.AddModelError("error", "You can't delete the default server.");
                return BadRequest(ModelState);
            }

            dbContext.SmtpServers.Remove(srv);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]SmtpServer data) {
            var exists = await dbContext.SmtpServers.AnyAsync(s => s.Id == data.Id);

            if (!exists) {
                return NotFound();
            }

            dbContext.Update(data);

            data.PasswordEncrypted = data.Password;
            data.Password = null;

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
