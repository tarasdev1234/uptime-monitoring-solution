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

namespace Admin.Api.Controllers.Branding
{
    [Route("api/company")]
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller {
        private readonly ApplicationDbContext dbContext;

        public CompanyController (ApplicationDbContext context) {
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<Company>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCompanies ([FromQuery]PagedQuery p, [FromQuery] long? brandId, [FromQuery(Name = "s")] string search = "") {
            var query = dbContext.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(c => c.Name.Contains(search));
            }

            if (brandId.HasValue) {
                query = query.Where(c => c.BrandId == brandId);
            }

            var totalItems = await query.LongCountAsync();

            var items = await query
                .Include(c => c.Brand)
                .Select(c => new Company() {
                    Id = c.Id,
                    BrandId = c.BrandId,
                    BrandName = c.Brand.Name,
                    Name = c.Name,
                    UsersCount = c.Users.Count
                })
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            var model = new PaginatedItemsVm<Company>(p.PageIndex.Value + 1, p.PageSize.Value, totalItems, items);

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateCompany ([FromBody]CompanyViewModel value) {
            var brnd = await dbContext.Brands.FindAsync(value.BrandId);

            if (brnd == null) {
                return BadRequest(new { Error = "Brand not found" });
            }

            var company = new Company() {
                Name = value.Name,
                BrandId = value.BrandId
            };

            dbContext.Companies.Add(company);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, null);
        }
        
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateDepartment ([FromBody]CompanyViewModel data) {
            var company = await dbContext.Companies.FindAsync(data.Id);

            if (company == null) {
                return NotFound();
            }

            company.BrandId = data.BrandId;
            company.Name = data.Name;

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCompany (long id) {
            var item = await dbContext.Companies.FindAsync(id);

            if (item == null) {
                return NotFound();
            }

            return Ok(item);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteCompany (long id) {
            var item = await dbContext.Companies.FindAsync(id);

            if (item == null) {
                return NotFound();
            }

            dbContext.Companies.Remove(item);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
