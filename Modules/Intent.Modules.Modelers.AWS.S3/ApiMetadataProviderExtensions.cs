using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.S3.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<S3BucketModel> GetS3BucketModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(S3BucketModel.SpecializationTypeId)
                .Select(x => new S3BucketModel(x))
                .ToList();
        }

    }
}