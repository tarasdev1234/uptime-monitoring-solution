using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.KnowledgeBase;
using Uptime.Mvc.Controllers;
using Admin.Api.ViewModels;
using Reliablesite.Service.Model.Dto;

namespace Admin.Api.Controllers.KnowledgeBase
{
    [Route("api/kb/tags")]
    [Authorize(Roles = "Admin")]
    public class TagController : BaseController {
        public TagController (ApplicationDbContext context) : base(context) { }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<KbTag>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTags ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.KbTags.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(pg => pg.Name.Contains(search));
            }

            var count = await query.LongCountAsync();
            var tags = await query
                        .OrderBy(a => a.Id)
                        .Paged(p)
                        .ToListAsync();

            var view = new PaginatedItemsVm<KbTag>(p.PageIndex.Value, p.PageSize.Value, count, tags);

            return Ok(view);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTag (long id) {
            var tag = await dbContext.KbTags.FindAsync(id);

            if (tag == null) {
                return NotFound();
            }

            return Ok(tag);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateTag ([FromBody]KbTag value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            dbContext.KbTags.Add(value);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = value.Id }, null);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]KbTag value) {
            var tag = await dbContext.KbTags.AsNoTracking().Where(p => p.Id == value.Id).SingleOrDefaultAsync();

            if (tag == null) {
                return NotFound(new { Message = $"Tag with id {value.Id} not found." });
            }

            tag = value;

            dbContext.KbTags.Update(tag);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = value.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var tag = await dbContext.KbTags.FindAsync(id);

            if (tag == null) {
                return NotFound();
            }

            dbContext.KbTags.Remove(tag);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
