using Admin.Api.ViewModels;
using Admin.Api.ViewModels.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Branding;
using Uptime.Data.Models.Plugins;
using OrchardCore.Environment.Shell;
using Microsoft.AspNetCore.Hosting;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Models;
using OrchardCore.Settings;
using Reliablesite.Service.Model.Dto;

namespace Admin.Api.Controllers.Branding
{
    [Authorize(Roles = "Admin")]
    [Route("api/brand")]
    public class BrandController : Controller {
        private readonly IShellHost _orchardHost;
        private readonly ApplicationDbContext dbContext;
        private readonly IShellFeaturesManager _shellFeaturesManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IExtensionManager _extensionManager;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly ISiteService _siteService;

        public BrandController (
            ISiteService siteService,
            IShellHost orchardHost,
            ApplicationDbContext context,
            IShellSettingsManager shellSettingsManager,
            IExtensionManager extensionManager,
            IHostingEnvironment hostingEnvironment) {
            _siteService = siteService;
            _orchardHost = orchardHost;
            _hostingEnvironment = hostingEnvironment;
            _shellSettingsManager = shellSettingsManager;
            _extensionManager = extensionManager;
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<Brand>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBrands ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(d => d.Name.Contains(search));
            }

            var totalItems = await query.LongCountAsync();

            var brands = await query
                .OrderBy(c => c.Id)
                .Paged(p)
                .ToListAsync();

            var model = new PaginatedItemsVm<Brand>(p.PageIndex.Value + 1, p.PageSize.Value, totalItems, brands);

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateBrand ([FromForm]string name, [FromForm]string url) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var brnd = new Brand() {
                Name = name,
                Url = url
            };

            dbContext.Brands.Add(brnd);

            await dbContext.SaveChangesAsync();

            var shellSettings = new ShellSettings {
                Name = name,
                RequestUrlPrefix = $"brand/{name}",
                RequestUrlHost = url,
                State = TenantState.Running
            };

            shellSettings["ConnectionString"] = "";
            shellSettings["TablePrefix"] = "";
            shellSettings["DatabaseProvider"] = "";

            _shellSettingsManager.SaveSettings(shellSettings);
            var shellContext = await _orchardHost.GetOrCreateShellContextAsync(shellSettings);

            return CreatedAtAction(nameof(GetBrand), new { id = brnd.Id }, null);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(BrandViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetBrand (long id) {
            var brnd = await dbContext.Brands
                            .Include(b => b.BrandPlugins)
                            .Where(b => b.Id == id)
                            .FirstOrDefaultAsync();

            if (brnd == null) {
                return NotFound();
            }

            var vm = new BrandViewModel() {
                Id = brnd.Id,
                Name = brnd.Name,
                Url = brnd.Url,
                Theme = brnd.Theme,
                Plugins = brnd.BrandPlugins.Select(p => p.Name).ToList()
            };

            return Ok(vm);
        }

        [HttpGet("activate")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ActivatePlugin (long id, string pluginId) { // TODO: rebuild context for the brand host
            var activated = await dbContext.BrandPlugins
                                  .Where(p => p.Name == pluginId && p.BrandId == id)
                                  .AnyAsync();

            if (activated) {
                return Ok();
            }

            var brnd = await dbContext.Brands
                            .Where(b => b.Id == id)
                            .FirstOrDefaultAsync();
            
            if (brnd == null) {
                return NotFound();
            }

            var bp = new BrandPlugin() {
                Name = pluginId,
                BrandId = brnd.Id
            };

            dbContext.BrandPlugins.Add(bp);

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id:long}/departments")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PaginatedItemsVm<Department>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDepartments (long id, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0) {
            var totalItems = await dbContext.Departments.LongCountAsync();

            var itemsOnPage = await dbContext.Departments
                .Where(d => d.BrandId == id)
                .OrderBy(c => c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginatedItemsVm<Department>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBrand (long id) {
            var brnd = await dbContext.Brands.SingleOrDefaultAsync(b => b.Id == id);

            if (brnd == null) {
                return NotFound();
            }

            dbContext.Brands.Remove(brnd);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateBrand ([FromBody]Brand brandData) {
            var exists = await dbContext.Brands.AnyAsync(b => b.Id == brandData.Id);

            if (!exists) {
                return NotFound();
            }

            dbContext.Update(brandData);

            var brandPlugins = await dbContext.BrandPlugins
                                .Where(bp => bp.BrandId == brandData.Id)
                                .ToListAsync();

            if (brandPlugins.Count > 0) {
                dbContext.BrandPlugins.RemoveRange(brandPlugins);
            }

            if (brandData.Plugins.Count > 0) {
                dbContext.BrandPlugins.AddRange(brandData.Plugins.Select(bp => new BrandPlugin() {
                    Name = bp,
                    BrandId = brandData.Id
                }));
            }

            await dbContext.SaveChangesAsync();

            var shellContext = _orchardHost
                .ListShellContexts()
                .OrderBy(x => x.Settings.Name)
                .FirstOrDefault(x => string.Equals(x.Settings.Name, brandData.Name, StringComparison.OrdinalIgnoreCase));

            if (shellContext == null) {
                return NotFound();
            }
            
            var shellSettings = shellContext.Settings;
            await _orchardHost.ReloadShellContextAsync(shellSettings);
            
            return Ok();
        }

        private bool FeatureIsAllowed (IFeatureInfo feature) {
            // allow to modify only custom features
            return !feature.Id.StartsWith("Orchard") && feature.Id != _hostingEnvironment.ApplicationName;
        }
    }
}
