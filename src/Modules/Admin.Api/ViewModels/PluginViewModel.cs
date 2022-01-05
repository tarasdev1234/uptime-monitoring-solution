using OrchardCore.Environment.Extensions;

namespace Admin.Api.ViewModels {
    public class PluginViewModel {
        public string Id { get; set; }

        public string Name { get; set; }

        public IManifestInfo PluginInfo { get; set; }
    }
}
