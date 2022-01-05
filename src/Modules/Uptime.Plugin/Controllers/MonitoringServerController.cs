using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Service.Model.Dto;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Monitoring.Data;
using Uptime.Monitoring.Model.Models;
using Uptime.Mvc.Controllers;
using Uptime.Plugin.HealthChecks;
using Uptime.Plugin.ViewModels;

namespace Uptime.Plugin.Controllers
{
    public class MonitoringServerController : BaseController {
        private readonly UptimeMonitoringContext monitoringContext;
        private readonly UptimeServerHealthStatusCollection healthDb;

        public MonitoringServerController (ApplicationDbContext ctx,
            UptimeServerHealthStatusCollection healthDb,
            UptimeMonitoringContext monitoringContext) : base(ctx) {
            this.monitoringContext = monitoringContext;
            this.healthDb = healthDb;
        }

        [HttpGet]
        [Route("/api/uptime/servers")]
        [ProducesResponseType(typeof(PaginatedItemsVm<MonitoringServer>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetServers ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = monitoringContext.MonitoringServers.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(s => s.Name.Contains(search));
            }

            var totalItems = await query.LongCountAsync();
            var servers = await query
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            foreach (var srv in servers) {
                srv.Health = (int)healthDb.Get(srv.Id);
            }
            
            var model = new PaginatedItemsVm<MonitoringServer>(p.PageIndex.Value + 1, p.PageSize.Value, totalItems, servers);

            return Ok(model);
        }

        [HttpPost]
        [Route("/api/uptime/servers")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create ([FromBody]MonitoringServer value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (await monitoringContext.MonitoringServers.AnyAsync(s => s.Host == value.Host)) {
                return BadRequest(new { error = $"Server with address {value.Host} is already exist" });
            }

            //var ipregex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            //var hostregex = @"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$";

            //if (!Regex.IsMatch(value.Host, ipregex) && !Regex.IsMatch(value.Host, hostregex)) {
            //    return BadRequest(new { error = "Host must be a valid ip address or hostname" });
            //}

            monitoringContext.MonitoringServers.Add(value);

            await monitoringContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServer), new { id = value.Id }, null);
        }

        [HttpGet]
        [Route("/api/uptime/servers/{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetServer (long id) {
            var server = await monitoringContext.MonitoringServers.FindAsync(id);

            if (server == null) {
                return NotFound();
            }

            return Ok(server);
        }

        [HttpPut]
        [Route("/api/uptime/servers")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]MonitoringServer value) {
            var exists = await monitoringContext.MonitoringServers
                .AnyAsync(s => s.Id == value.Id);

            if (!exists) {
                return NotFound(new { Message = $"Server with id {value.Id} not found." });
            }

            //var ipregex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            //var hostregex = @"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$";

            //if (!Regex.IsMatch(value.Host, ipregex) || !Regex.IsMatch(value.Host, hostregex)) {
            //    return BadRequest(new { error = "Host must be a valid ip address or hostname" });
            //}

            monitoringContext.MonitoringServers.Update(value);

            await monitoringContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServer), new { id = value.Id }, null);
        }

        [HttpDelete]
        [Route("/api/uptime/servers/{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var srv = await monitoringContext.MonitoringServers.FindAsync(id);

            if (srv == null) {
                return NotFound();
            }

            monitoringContext.MonitoringServers.Remove(srv);

            await monitoringContext.SaveChangesAsync();

            return NoContent();
        }

        private bool IsValidHostname() {
            return true;
        }
    }
}
