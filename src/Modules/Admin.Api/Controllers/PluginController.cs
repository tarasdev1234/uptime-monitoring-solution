using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using Uptime.Data;
using Uptime.Mvc.Controllers;
using Admin.Api.ViewModels;
using OrchardCore.Environment.Extensions;
using OrchardCore.Features.Models;
using System.Collections.Generic;
using OrchardCore.DisplayManagement.Extensions;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Extensions.Features;
using Microsoft.AspNetCore.Hosting;
using Reliablesite.Service.Model.Dto;

namespace Admin.Api.Controllers
{
    [Route("api/plugins")]
    [Authorize(Roles = "Admin")]
    public class PluginController : BaseController {
        private readonly IExtensionManager _extensionManager;
        private readonly IShellFeaturesManager _shellFeaturesManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PluginController (
            IHostingEnvironment hostingEnvironment,
            IExtensionManager extensionManager,
            IShellFeaturesManager shellFeaturesManager,
            ApplicationDbContext ctx) : base(ctx) {
            _hostingEnvironment = hostingEnvironment;
            _extensionManager = extensionManager;
            _shellFeaturesManager = shellFeaturesManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<PluginViewModel>), (int)HttpStatusCode.OK)]
        public async System.Threading.Tasks.Task<IActionResult> GetAllAsync ([FromQuery]PagedQuery pagination, [FromQuery]string s) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var enabledFeatures = await _shellFeaturesManager.GetEnabledFeaturesAsync();

            var moduleFeatures = new List<ModuleFeature>();
            foreach (var moduleFeatureInfo in _extensionManager
                .GetFeatures()
                .Where(f => !f.Extension.IsTheme() && FeatureIsAllowed(f))) {
                var dependentFeatures = _extensionManager.GetDependentFeatures(moduleFeatureInfo.Id);
                var featureDependencies = _extensionManager.GetFeatureDependencies(moduleFeatureInfo.Id);

                var moduleFeature = new ModuleFeature {
                    Descriptor = moduleFeatureInfo,
                    IsEnabled = enabledFeatures.Contains(moduleFeatureInfo),
                    DependentFeatures = dependentFeatures.Where(x => x.Id != moduleFeatureInfo.Id).ToList(),
                    FeatureDependencies = featureDependencies.Where(d => d.Id != moduleFeatureInfo.Id).ToList()
                };

                moduleFeatures.Add(moduleFeature);
            }

            var view = new PaginatedItemsVm<PluginViewModel>(pagination.PageIndex.Value, pagination.PageSize.Value, moduleFeatures.LongCount(), moduleFeatures.Select(f => new PluginViewModel() {
                Id = f.Descriptor.Id,
                Name = f.Descriptor.Name,
                PluginInfo = f.Descriptor.Extension.Manifest
            }));

            return Ok(view);

            //return View(new FeaturesViewModel {
            //    Features = moduleFeatures,
            //    IsAllowed = FeatureIsAllowed
            //});

            //var query = Core.AppHost.Instance.Plugins.Select(kvp => kvp.Value).AsQueryable();

            //if (!string.IsNullOrEmpty(s)) {
            //    query = query.Where(p => p.Name.Contains(s, System.StringComparison.OrdinalIgnoreCase));
            //}

            //var count = query.LongCount();
            //var plugins = query
            //            .Select(p => new PluginViewModel() {
            //                Name = p.Name,
            //                PluginInfo = p.PluginInfo
            //            })
            //            .OrderBy(p => p.Name)
            //            .Paginated(pagination)
            //            .ToList();

            //var view = new PaginatedItemsVm<PluginViewModel>(pagination.PageIndex.Value, pagination.PageSize.Value, count, plugins);

            //return Ok(view);
        }

        [Route("custom")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<PluginViewModel>), (int)HttpStatusCode.OK)]
        public IActionResult GetCustomPlugins ([FromQuery]PagedQuery pagination, [FromQuery]string s) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var plgns = dbContext.Plugins.AsQueryable();

            if (!string.IsNullOrEmpty(s)) {
                plgns = plgns.Where(p => p.Name.Contains(s));
            }

            var count = plgns.LongCount();
            var plugins = plgns
                        .Select(p => new PluginViewModel() {
                            Name = p.Name,
                            Id = p.Id.ToString()
                        })
                        .OrderBy(p => p.Id)
                        .Paged(pagination)
                        .ToList();

            var view = new PaginatedItemsVm<PluginViewModel>(pagination.PageIndex.Value, pagination.PageSize.Value, count, plugins);

            return Ok(view);
        }
        
        private bool FeatureIsAllowed (IFeatureInfo feature) {
            // allow to modify only custom features
            return !feature.Id.StartsWith("Orchard") && feature.Id != _hostingEnvironment.ApplicationName;
        }
    }
}
