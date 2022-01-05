using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Monitoring.Data;
using Uptime.Monitoring.Model.Models;
using Uptime.Mvc.Controllers;
using Uptime.Plugin.ViewModels;
using Uptime.Plugin.ViewModels.Dashboard;
using Reliablesite.Authority.Authentication;
using Reliablesite.Service.Model.Dto;

namespace Uptime.Plugin.Controllers
{
    [Authorize]
    public class AlertContactController : BaseController {
        private readonly UptimeMonitoringContext monitoringContext;

        public AlertContactController (
            ApplicationDbContext ctx,
            UptimeMonitoringContext monitoringContext
        ) : base(ctx) {
            this.monitoringContext = monitoringContext;
        }

        [HttpPost]
        [Route("/api/alertcontacts")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateEdit ([FromBody]CreateAlertViewModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = User.GetId();

            AlertContact alert;

            if (model.Id > 0) {
                alert = await monitoringContext.AlertContacts
                    .Where(m => m.UserId == userId && m.Id == model.Id)
                    .FirstOrDefaultAsync();

                if (alert == null) {
                    return NotFound();
                }
            } else {
                alert = new AlertContact();
                alert.UserId = userId;
                alert.Active = true;

                monitoringContext.AlertContacts.Add(alert);
            }

            alert.Name = model.Name;
            alert.Type = model.Type;
            alert.Email = model.Email;
            alert.NotificationType = model.NotificationType;

            await monitoringContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlert), new { id = alert.Id }, null);
        }

        [HttpGet]
        [Route("/api/alertcontacts")]
        [ProducesResponseType(typeof(PaginatedItemsVm<AlertContact>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetContacts ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = User.GetId();

            var query = monitoringContext.AlertContacts
                .Where(ac => ac.UserId == userId);

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(ac => ac.Name.Contains(search));
            }

            var totalItems = await query.LongCountAsync();
            var contacts = await query
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            var model = new PaginatedItemsVm<AlertContact>(p.PageIndex.Value + 1, p.PageSize.Value, totalItems, contacts);

            return Ok(model);
        }

        [HttpGet]
        [Route("/api/alertcontacts/{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlert (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var userId = User.GetId();

            var alert = await monitoringContext.AlertContacts
                .Where(ac => ac.Id == id && ac.UserId == userId)
                .FirstOrDefaultAsync();

            if (alert == null) {
                return NotFound();
            }

            return Ok(new CreateAlertViewModel() {
                Id = alert.Id,
                Name = alert.Name,
                Email = alert.Email,
                NotificationType = alert.NotificationType,
                Type = alert.Type,
                Active = alert.Active,
            });
        }

        [HttpDelete]
        [Route("/api/alertcontacts/{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var userId = User.GetId();

            var alert = await monitoringContext.AlertContacts
                .Where(m => m.UserId == userId && m.Id == id)
                .FirstOrDefaultAsync();

            if (alert == null) {
                return NotFound();
            }

            monitoringContext.AlertContacts.Remove(alert);

            await monitoringContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [Route("/api/alertcontacts/{id:long}/status")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EditAlert (long id, [FromQuery] bool active = true) {
            try {
                var userId = User.GetId();

                var alert = await monitoringContext.AlertContacts
                    .Where(m => m.UserId == userId && m.Id == id)
                    .FirstOrDefaultAsync();

                if (alert == null) {
                    return NotFound();
                }
                
                alert.Active = active;

                await monitoringContext.SaveChangesAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }
    }
}
