using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<VisualStudioSolutionModel> GetVisualStudioSolutionModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(VisualStudioSolutionModel.SpecializationTypeId)
                .Select(x => new VisualStudioSolutionModel(x))
                .ToList();
        }


    }
}