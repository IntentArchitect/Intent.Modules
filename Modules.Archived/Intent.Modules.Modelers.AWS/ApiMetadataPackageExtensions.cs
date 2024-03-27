using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<AWSPackageModel> GetAWSPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(AWSPackageModel.SpecializationTypeId)
                .Select(x => new AWSPackageModel(x))
                .ToList();
        }


    }
}