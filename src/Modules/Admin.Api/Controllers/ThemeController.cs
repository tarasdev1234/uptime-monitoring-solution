using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models;
using Admin.Api.ViewModels;
using OrchardCore.Environment.Extensions;
using OrchardCore.DisplayManagement.Extensions;
using Reliablesite.Service.Model.Dto;

namespace Admin.Api.Controllers
{
    [Route("api/themes")]
    [Authorize(Roles = "Admin")/*(AuthenticationSchemes = "Bearer")*/]
    public class ThemeController : Controller {
        public static readonly string[] DefaultThemes = {
            "SafeMode",
            "TheAdmin",
            "TheAgencyTheme",
            "TheBlogTheme",
            "TheTheme",
            "TheComingSoonTheme"
        }; 
        private readonly IExtensionManager _extensionManager;
        private readonly ApplicationDbContext dbCtx;

        public ThemeController (
            IExtensionManager extensionManager,
            ApplicationDbContext context) {
            _extensionManager = extensionManager;
            dbCtx = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<Theme>), (int)HttpStatusCode.OK)]
        public IActionResult GetAll ([FromQuery]PagedQuery pagination, [FromQuery]string s) {
            var themes = _extensionManager.GetExtensions().OfType<IThemeExtensionInfo>()
                .Where(ed => !DefaultThemes.Contains(ed.Id))
                .Select(extensionDescriptor => {
                    return new ThemeEntry {
                        Id = extensionDescriptor.Id,
                        Name = extensionDescriptor.Manifest.Name
                    };
                }).ToList();
            
            var view = new PaginatedItemsVm<ThemeEntry>(pagination.PageIndex.Value, pagination.PageSize.Value, themes.LongCount(), themes);

            return Ok(view);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create ([FromBody]Theme value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var theme = new Theme() {
                Name = value.Name,
            };

            dbCtx.Themes.Add(theme);

            await dbCtx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTheme), new { id = theme.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTheme (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var curr = await dbCtx.Themes.FindAsync(id);

            if (curr == null) {
                return NotFound();
            }

            return Ok(curr);
        }
        
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]Theme value) {
            var theme = await dbCtx.Themes.AsNoTracking().Where(t => t.Id == value.Id).SingleOrDefaultAsync();

            if (theme == null) {
                return NotFound(new { Message = $"Theme with id {value.Id} not found." });
            }

            theme = value;

            dbCtx.Themes.Update(theme);

            await dbCtx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTheme), new { id = value.Id }, null);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var theme = await dbCtx.Themes.FindAsync(id);

            if (theme == null) {
                return NotFound();
            }

            dbCtx.Themes.Remove(theme);

            await dbCtx.SaveChangesAsync();

            return NoContent();
        }
    }
}
