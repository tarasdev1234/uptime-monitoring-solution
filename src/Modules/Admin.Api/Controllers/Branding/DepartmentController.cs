using Admin.Api.ViewModels;
using Admin.Api.ViewModels.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Service.Model.Dto;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Branding;
using Uptime.Data.Models.Support;

namespace Admin.Api.Controllers.Branding
{
    [Authorize(Roles = "Admin")]
    [Route("api/departments")]
    public class DepartmentController : Controller {
        private readonly ApplicationDbContext dbContext;

        public DepartmentController (ApplicationDbContext context) {
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<Brand>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDepartments ([FromQuery]PagedQuery pagination, [FromQuery]long? brandId = null, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.Departments.AsQueryable();

            if (brandId != null) {
                query = query.Where(d => d.BrandId == brandId);
            }

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(d => d.Name.Contains(search));
            }

            var totalItems = await query.LongCountAsync();
            
            var dps = await query
                .Include(d => d.Brand)
                .Select(d => new Department() {
                    Id = d.Id,
                    BrandId = d.BrandId,
                    BrandName = d.Brand.Name,
                    SmtpServerId = d.SmtpServerId,
                    Name = d.Name,
                    AutoResponderId = d.AutoResponderId,
                    TicketsCount = d.Tickets.Count,
                    UsersCount = d.Users.Count,
                    ResponderEnabled = d.ResponderEnabled,
                })
                .OrderBy(d => d.Id)
                .Paged(pagination)
                .ToListAsync();

            var model = new PaginatedItemsVm<Department>(pagination.PageIndex.Value, pagination.PageSize.Value, totalItems, dps);

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateDepartment ([FromForm]long brandId, [FromForm]string name, [FromForm]long? smtpServerId) {
            var brnd = await dbContext.Brands.FindAsync(brandId);

            if (brnd == null) {
                return BadRequest(new { Error = "Brand not found" });
            }

            if (await dbContext.Departments.AnyAsync(c => c.Name == name)) {
                return BadRequest(new { error = $"Department with name {name} is already exist" });
            }

            var dep = new Department() {
                Name = name,
                BrandId = brandId,
                SmtpServerId = smtpServerId
            };

            dbContext.Departments.Add(dep);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = dep.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDepartment (long id) {
            var dep = await dbContext.Departments.FindAsync(id);

            if (dep == null) {
                return NotFound();
            }

            return Ok(dep);
        }

        [HttpGet("{id:long}/responder")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetResponder (long id) {
            var dep = await dbContext.Departments
                            .Include(d => d.AutoResponder)
                            .Where(d => d.Id == id)
                            .FirstOrDefaultAsync();

            if (dep == null) {
                return NotFound();
            }

            var responder = dep.AutoResponder ?? new AutoResponder() {
                Template = "Default Template"
            };

            responder.IsEnabled = dep.ResponderEnabled;

            return Ok(responder);
        }

        [HttpPut("{id:long}/responder")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateResponder (long id, [FromBody]AutoResponder data) {
            var dep = await dbContext.Departments
                            .Include(d => d.AutoResponder)
                            .Where(d => d.Id == id)
                            .FirstOrDefaultAsync();

            if (dep == null) {
                return NotFound();
            }

            var responder = dep.AutoResponder ?? new AutoResponder();

            responder.Template = data.Template;
            dep.ResponderEnabled = data.IsEnabled;
            dep.AutoResponder = responder;

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteDepartment (long id) {
            var dep = await dbContext.Departments.FindAsync(id);

            if (dep == null) {
                return NotFound();
            }

            dbContext.Departments.Remove(dep);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateDepartment ([FromBody]DepartmentViewModel depData) {
            var dep = await dbContext.Departments.FindAsync(depData.Id);

            if (dep == null) {
                return NotFound();
            }

            dep.BrandId = depData.BrandId;
            dep.Name = depData.Name;
            dep.SmtpServerId = depData.SmtpServerId;

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
