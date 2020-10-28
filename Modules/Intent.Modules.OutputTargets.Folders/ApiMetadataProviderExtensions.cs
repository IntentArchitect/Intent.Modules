using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.OutputTargets.Folders.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<RoleModel> GetRoleModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(RoleModel.SpecializationTypeId)
                .Select(x => new RoleModel(x))
                .ToList();
        }

    }
}