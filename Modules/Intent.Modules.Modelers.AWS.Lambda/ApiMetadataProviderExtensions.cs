using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Lambda.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<LambdaFunctionModel> GetLambdaFunctionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(LambdaFunctionModel.SpecializationTypeId)
                .Select(x => new LambdaFunctionModel(x))
                .ToList();
        }

    }
}