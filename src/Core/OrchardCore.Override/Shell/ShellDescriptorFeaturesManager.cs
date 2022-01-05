using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Descriptor.Models;

namespace OrchardCore.Override.Shell
{
    public class ShellDescriptorFeaturesManager : IShellDescriptorFeaturesManager
    {
        public Task<(IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>)> UpdateFeaturesAsync(
            ShellDescriptor shellDescriptor,
            IEnumerable<IFeatureInfo> featuresToDisable,
            IEnumerable<IFeatureInfo> featuresToEnable,
            bool force
        ) {
            throw new System.NotImplementedException();
        }
    }
}