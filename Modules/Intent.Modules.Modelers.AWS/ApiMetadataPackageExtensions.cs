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
        public static IList<AWSServerlessPackageModel> GetAWSServerlessPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(AWSServerlessPackageModel.SpecializationTypeId)
                .Select(x => new AWSServerlessPackageModel(x))
                .ToList();
        }


    }
}