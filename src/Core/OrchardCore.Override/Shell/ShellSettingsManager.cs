using OrchardCore.Environment.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell.Configuration;

namespace OrchardCore.Override.Shell {
    public class ShellSettingsManager : IShellSettingsManager {
        private readonly IServiceProvider sp;
        private List<ShellSettings> _settings = null;
        private readonly IConfiguration _configuration;

        public ShellSettingsManager (IServiceProvider sp,
            IConfiguration applicationConfiguration,
            IShellsConfigurationSources configurationSources
        ) {
            this.sp = sp;
            
            var lastProviders = (applicationConfiguration as IConfigurationRoot)?.Providers
                .Where(p => p is EnvironmentVariablesConfigurationProvider ||
                            p is CommandLineConfigurationProvider)
                .ToArray();

            var configurationBuilder = new ConfigurationBuilder()
                .AddConfiguration(applicationConfiguration)
                .AddSources(configurationSources);

            if (lastProviders.Any())
            {
                configurationBuilder.AddConfiguration(new ConfigurationRoot(lastProviders));
            }

            _configuration = configurationBuilder.Build().GetSection("OrchardCore");
        }

        public ShellSettings CreateDefaultSettings () {
            return new ShellSettings
            (
                new ShellConfiguration(_configuration),
                new ShellConfiguration(_configuration)
            );
        }

        public ShellSettings GetSettings (string name) {
            if (!TryGetSettings(name, out ShellSettings settings)) {
                throw new ArgumentException("The specified tenant name is not valid.", nameof(name));
            }

            return settings;
        }

        public IEnumerable<ShellSettings> LoadSettings () {
            if (_settings != null) {
                return _settings;
            }

            var settings = new List<ShellSettings>();

            using (var scope = sp.CreateScope()) {
                var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                settings = ctx.Brands
                    .Select(b => new ShellSettings() {
                        State = OrchardCore.Environment.Shell.Models.TenantState.Running,
                        Name = b.Name,
                        RequestUrlHost = b.Url
                    })
                    .ToList();
            }

            return settings;
        }

        public void SaveSettings (ShellSettings settings) {
            _settings = null;
        }

        public bool TryGetSettings (string name, out ShellSettings settings) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentException("The tenant name cannot be null or empty.", nameof(name));
            }

            settings = LoadSettings().FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.Ordinal));

            return settings != null;
        }
    }
}
