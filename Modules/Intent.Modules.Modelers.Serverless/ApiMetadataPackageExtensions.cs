using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<ServerlessPackageModel> GetServerlessPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(ServerlessPackageModel.SpecializationTypeId)
                .Select(x => new ServerlessPackageModel(x))
                .ToList();
        }


    }
}