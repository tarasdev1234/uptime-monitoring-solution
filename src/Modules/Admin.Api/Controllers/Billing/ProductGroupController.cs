using Admin.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Service.Model.Dto;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Billing;

namespace Admin.Api.Controllers.Billing
{
    [Route("api/billing/productgroups")]
    [Authorize(Roles = "Admin")]
    public class ProductGroupController : Controller {
        private readonly ApplicationDbContext dbContext;

        public ProductGroupController (ApplicationDbContext context) {
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<ProductGroup>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetGroups ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.ProductGroups.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(pg => pg.Name.Contains(search));
            }

            var totalItems = await query.LongCountAsync();

            var grps = await query
                .Include(pg => pg.Products)
                .Include(pg => pg.Brand)
                .Select(pg => new ProductGroup() {
                    Id = pg.Id,
                    Name = pg.Name,
                    BrandId = pg.BrandId,
                    BrandName = pg.Brand.Name,
                    ProductsCount = pg.Products.Count
                })
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            var model = new PaginatedItemsVm<ProductGroup>(p.PageIndex.Value, p.PageSize.Value, totalItems, grps);

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create ([FromBody]ProductGroup value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (await dbContext.ProductGroups.AnyAsync(c => c.Name == value.Name && c.BrandId == value.BrandId)) {
                return BadRequest(new { error = $"Group with name {value.Name} is already exist" });
            }

            dbContext.ProductGroups.Add(value);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = value.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetGroup (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var group = await dbContext.ProductGroups.FindAsync(id);

            if (group == null) {
                return NotFound();
            }

            return Ok(group);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]ProductGroup value) {
            var grp = await dbContext.ProductGroups.AsNoTracking().Where(pg => pg.Id == value.Id).SingleOrDefaultAsync();

            if (grp == null) {
                return NotFound(new { Message = $"Group with id {value.Id} not found." });
            }

            grp = value;

            dbContext.ProductGroups.Update(grp);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = value.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var grp = await dbContext.ProductGroups.FindAsync(id);

            if (grp == null) {
                return NotFound();
            }

            dbContext.ProductGroups.Remove(grp);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
