using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<ApplicationTemplateModel> GetApplicationTemplateModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(ApplicationTemplateModel.SpecializationTypeId)
                .Select(x => new ApplicationTemplateModel(x))
                .ToList();
        }


    }
}