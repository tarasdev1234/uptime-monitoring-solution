using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;

namespace OrchardCore.Override.Shell
{
    public class ShellFeaturesManager : IShellFeaturesManager
    {
        public Task<IEnumerable<IFeatureInfo>> GetEnabledFeaturesAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IFeatureInfo>> GetAlwaysEnabledFeaturesAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IFeatureInfo>> GetDisabledFeaturesAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<(IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>)> UpdateFeaturesAsync(IEnumerable<IFeatureInfo> featuresToDisable, IEnumerable<IFeatureInfo> featuresToEnable, bool force)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IExtensionInfo>> GetEnabledExtensionsAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}