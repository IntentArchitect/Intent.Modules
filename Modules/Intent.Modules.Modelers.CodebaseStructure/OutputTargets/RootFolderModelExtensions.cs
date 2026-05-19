using Intent.Configuration;
using Intent.Modelers.CodebaseStructure.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modelers.CodebaseStructure.OutputTargets
{
    internal static class RootFolderModelExtensions
    {
        public static IOutputTargetConfig ToOutputTargetConfig(this RootFolderModel model)
        {
            return new RootFolderOutputTarget(model);
        }
    }
}
