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
    [Route("api/billing/currencies")]
    [Authorize(Roles = "Admin")]
    public class CurrencyController : Controller {
        private readonly ApplicationDbContext dbContext;

        public CurrencyController (ApplicationDbContext context) {
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<Currency>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCurrencies ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.Currencies.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(c => c.Code.Contains(search));
            }

            var totalItems = await query.LongCountAsync();

            var itemsOnPage = await query
                .Include(c => c.Format)
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            var model = new PaginatedItemsVm<Currency>(p.PageIndex.Value, p.PageSize.Value, totalItems, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("formats")]
        [ProducesResponseType(typeof(PaginatedItemsVm<CurrencyFormat>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFormats () {
            var totalItems = await dbContext.CurrencyFormats.LongCountAsync();

            var itemsOnPage = await dbContext.CurrencyFormats.ToListAsync();

            var model = new PaginatedItemsVm<CurrencyFormat>(1, 10, totalItems, itemsOnPage);

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create ([FromBody]Currency value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (await dbContext.Currencies.AnyAsync(c => c.Code == value.Code && c.BrandId == value.BrandId)) {
                return BadRequest(new { error = $"Currency with code {value.Code} is already exist" });
            }

            var curr = new Currency() {
                BrandId = value.BrandId,
                Code = value.Code,
                Prefix = value.Prefix,
                Suffix = value.Suffix,
                ConvertRate = value.ConvertRate,
                FormatId = value.FormatId,
                Base = false
            };

            dbContext.Currencies.Add(curr);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCurrency), new { id = curr.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCurrency (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var curr = await dbContext.Currencies.FindAsync(id);

            if (curr == null) {
                return NotFound();
            }

            return Ok(curr);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]Currency value) {
            var curr = await dbContext.Currencies.AsNoTracking().Where(c => c.Id == value.Id).SingleOrDefaultAsync();

            if (curr == null) {
                return NotFound(new { Message = $"Currency with id {value.Id} not found." });
            }

            curr = value;

            dbContext.Currencies.Update(curr);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCurrency), new { id = value.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var curr = await dbContext.Currencies.FindAsync(id);

            if (curr == null) {
                return NotFound();
            }

            dbContext.Currencies.Remove(curr);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
