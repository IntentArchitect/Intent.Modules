using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.SQS.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<SQSQueueModel> GetSQSQueueModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SQSQueueModel.SpecializationTypeId)
                .Select(x => new SQSQueueModel(x))
                .ToList();
        }

    }
}