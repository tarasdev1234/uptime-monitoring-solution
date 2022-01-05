using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Descriptor;

// overrides
using ShellStateManager = OrchardCore.Override.Shell.ShellStateManager;
using ShellDescriptorManager = OrchardCore.Override.Shell.ShellDescriptorManager;
using ShellSettingsManager = OrchardCore.Override.Shell.ShellSettingsManager;

// default
using ShellDescriptorFeaturesManager = OrchardCore.Environment.Shell.ShellDescriptorFeaturesManager;
using ShellFeaturesManager = OrchardCore.Environment.Shell.ShellFeaturesManager;

namespace OrchardCore.Override {
    public static class ShellBuilderExtensions {
        /// <summary>
        /// Adds services at the host level to load site settings from the file system
        /// and tenant level services to store states and descriptors in the database.
        /// </summary>
        public static OrchardCoreBuilder AddShellConfiguration(this OrchardCoreBuilder builder) {
            builder.AddSitesFolder();
            
            var svcs = builder.ApplicationServices;
            
            svcs.Replace(ServiceDescriptor.Singleton<IShellSettingsManager, ShellSettingsManager>());
            
            //builder.AddSitesFolder()
            builder.ConfigureServices(services => {
                services.AddScoped<IShellDescriptorManager, ShellDescriptorManager>();
                services.AddScoped<IShellStateManager, ShellStateManager>();
                services.AddScoped<IShellFeaturesManager, ShellFeaturesManager>();
                services.AddScoped<IShellDescriptorFeaturesManager, ShellDescriptorFeaturesManager>();
            });

            return builder;
        }
    }
}