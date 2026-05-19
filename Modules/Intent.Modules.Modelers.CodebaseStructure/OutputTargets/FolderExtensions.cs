using Intent.Configuration;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modelers.CodebaseStructure.OutputTargets
{
    internal static class FolderExtensions
    {
        public static IOutputTargetConfig ToOutputTargetConfig(this FolderModel model)
        {
            return new FolderOutputTarget(model);
        }
    }
}
