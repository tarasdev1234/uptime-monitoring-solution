using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Plugins;
using Uptime.Mvc.Services;

namespace OrchardCore.Override.Shell {
    public class ShellDescriptorManager : IShellDescriptorManager {
        private readonly ShellSettings _shellSettings;
        private readonly IEnumerable<ShellFeature> _alwaysEnabledFeatures;
        private readonly IEnumerable<IShellDescriptorManagerEventHandler> _shellDescriptorManagerEventHandlers;
        private readonly ApplicationDbContext dbContext;
        private readonly BrandContext _brandCtx;
        private readonly ILogger _logger;
        private ShellDescriptor _shellDescriptor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShellDescriptorManager (
            ShellSettings shellSettings,
            IEnumerable<ShellFeature> shellFeatures,
            IEnumerable<IShellDescriptorManagerEventHandler> shellDescriptorManagerEventHandlers,
            ApplicationDbContext ctx,
            BrandContext brandCtx,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ShellDescriptorManager> logger
        ) {
            _shellSettings = shellSettings;
            _brandCtx = brandCtx;
            _httpContextAccessor = httpContextAccessor;
            _alwaysEnabledFeatures = shellFeatures.Where(f => f.AlwaysEnabled).ToArray();
            _shellDescriptorManagerEventHandlers = shellDescriptorManagerEventHandlers;
            dbContext = ctx;
            _logger = logger;
        }

        public async Task<ShellDescriptor> GetShellDescriptorAsync () {
            var host = _httpContextAccessor.HttpContext.Request.Host.Host;

            // Prevent multiple queries during the same request
            if (_shellDescriptor == null) {
                var brand = await _brandCtx.GetCurrentBrandAsync();

                if (brand != null) {
                    _shellDescriptor = new ShellDescriptor() {
                        Features = brand.BrandPlugins.Select(bp => new ShellFeature(bp.Name)).ToList(),
                        Id = (int)brand.Id,
                        SerialNumber = (int)brand.Id
                    };
                    
                    if (_shellDescriptor != null) {
                        _shellDescriptor.Features = _alwaysEnabledFeatures
                            .Concat(
                                new List<ShellFeature> {
                                    new ShellFeature("Admin.Api", false),
                                    new ShellFeature("Admin.Web", false),
                                    new ShellFeature("Users", true),
                                    new ShellFeature("Uptime.Plugin", false),
                                    new ShellFeature("Uptime.Theme", false),
                                }
                            )
                            .Concat(_shellDescriptor.Features).Distinct().ToList();
                    }
                }
            }

            return _shellDescriptor;
        }

        public async Task UpdateShellDescriptorAsync (int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters) {
            var shellDescriptorRecord = await GetShellDescriptorAsync();
            var serialNumber = shellDescriptorRecord == null ? 0 : shellDescriptorRecord.SerialNumber;
            if (priorSerialNumber != serialNumber) {
                throw new InvalidOperationException("Invalid serial number for shell descriptor");
            }

            if (_logger.IsEnabled(LogLevel.Information)) {
                _logger.LogInformation("Updating shell descriptor for tenant '{TenantName}' ...", _shellSettings.Name);
            }

            if (shellDescriptorRecord == null) {
                shellDescriptorRecord = new ShellDescriptor { SerialNumber = 1 };
            } else {
                shellDescriptorRecord.SerialNumber++;
            }

            shellDescriptorRecord.Features = enabledFeatures.ToList();
            shellDescriptorRecord.Parameters = parameters.ToList();

            if (_logger.IsEnabled(LogLevel.Information)) {
                _logger.LogInformation("Shell descriptor updated for tenant '{TenantName}'.", _shellSettings.Name);
            }

            var brnd = await _brandCtx.GetCurrentBrandAsync();

            brnd.BrandPlugins = shellDescriptorRecord.Features.Select(f => new BrandPlugin() {
                Name = f.Id,
                BrandId = brnd.Id
            }).ToList();

            await dbContext.SaveChangesAsync();

            // Update cached reference
            _shellDescriptor = shellDescriptorRecord;

            await _shellDescriptorManagerEventHandlers.InvokeAsync(e => e.Changed(shellDescriptorRecord, _shellSettings.Name), _logger);
        }
    }
}
