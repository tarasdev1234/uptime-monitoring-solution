using Admin.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reliablesite.Service.Model.Dto;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.KnowledgeBase;
using Uptime.Mvc.Controllers;

namespace Admin.Api.Controllers.KnowledgeBase
{
    [Route("api/kb/categories")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController {
        public CategoryController (ApplicationDbContext context) : base(context) {}

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<KbCategory>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCats ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.KbCategories
                        .Include(c => c.ParentCategory)
                        .Include(c => c.Articles)
                        .AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(pg => pg.Name.Contains(search));
            }

            var count = await query.LongCountAsync();
            var cats = await query
                        .Select(c => new KbCategory() {
                            Id = c.Id,
                            Name = c.Name,
                            ParentCategoryId = c.ParentCategoryId,
                            ParentName = c.ParentCategory.Name,
                            ArticlesCount = c.Articles.Count
                        })
                        .OrderBy(a => a.Id)
                        .Paged(p)
                        .ToListAsync();

            var view = new PaginatedItemsVm<KbCategory>(p.PageIndex.Value, p.PageSize.Value, count, cats);

            return Ok(view);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCat (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var cat = await dbContext.KbCategories.FindAsync(id);

            if (cat == null) {
                return NotFound();
            }

            return Ok(cat);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateCat ([FromBody]KbCategory value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (await dbContext.KbCategories.AnyAsync(c => c.Name == value.Name)) {
                return BadRequest(new { error = $"Category with name {value.Name} is already exist" });
            }

            var cat = new KbCategory() {
                Name = value.Name,
                ParentCategoryId = value.ParentCategoryId
            };

            dbContext.KbCategories.Add(cat);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCat), new { id = cat.Id }, null);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]KbCategory value) {
            var cat = await dbContext.KbCategories.FindAsync(value.Id);

            if (cat == null) {
                return NotFound(new { Message = $"Category with id {value.Id} not found." });
            }

            cat.Name = value.Name;
            cat.ParentCategoryId = value.ParentCategoryId;

            dbContext.Update(cat);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCat), new { id = value.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var cat = await dbContext.KbCategories.FindAsync(id);
            
            if (cat == null) {
                return NotFound();
            }

            await DeleteCategoryAsync(cat);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private async Task DeleteCategoryAsync (KbCategory cat) {
            var children = await dbContext.KbCategories
                                .Where(c => c.ParentCategoryId == cat.Id)
                                .ToListAsync();

            foreach (var category in children) {
                await DeleteCategoryAsync(category);
            }

            dbContext.KbCategories.Remove(cat);
        }
    }
}
