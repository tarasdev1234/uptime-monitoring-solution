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
    [Route("api/billing/products")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller {
        private readonly ApplicationDbContext dbContext;

        public ProductController (ApplicationDbContext context) {
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<Product>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProducts ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(prdct => prdct.Name.Contains(search));
            }

            var totalItems = await query.LongCountAsync();

            var prdcts = await query
                .Include(prdct => prdct.ProductGroup)
                .Select(prdct => new Product() {
                    Id = prdct.Id,
                    Name = prdct.Name,
                    Price = prdct.Price,
                    Description = prdct.Description,
                    ProductGroupId = prdct.ProductGroupId,
                    ProductGroupName = prdct.ProductGroup.Name
                })
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            var model = new PaginatedItemsVm<Product>(p.PageIndex.Value, p.PageSize.Value, totalItems, prdcts);

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create ([FromBody]Product value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (await dbContext.Products.AnyAsync(c => c.Name == value.Name && c.ProductGroupId == value.ProductGroupId)) {
                return BadRequest(new { error = $"Product with name {value.Name} is already exist" });
            }

            dbContext.Products.Add(value);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = value.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProduct (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var prd = await dbContext.Products.FindAsync(id);

            if (prd == null) {
                return NotFound();
            }

            return Ok(prd);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]Product value) {
            var prd = await dbContext.Products.AsNoTracking().Where(p => p.Id == value.Id).SingleOrDefaultAsync();

            if (prd == null) {
                return NotFound(new { Message = $"Product with id {value.Id} not found." });
            }

            prd = value;

            dbContext.Products.Update(prd);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = value.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var prd = await dbContext.Products.FindAsync(id);

            if (prd == null) {
                return NotFound();
            }

            dbContext.Products.Remove(prd);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
