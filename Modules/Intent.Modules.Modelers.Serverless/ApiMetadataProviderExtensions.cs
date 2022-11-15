using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<PayloadModel> GetPayloadModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(PayloadModel.SpecializationTypeId)
                .Select(x => new PayloadModel(x))
                .ToList();
        }

    }
}