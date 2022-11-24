using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    public static class ApiMetadataProviderExtensions
    {

        public static IList<DynamoDBTableModel> GetDynamoDBTableModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DynamoDBTableModel.SpecializationTypeId)
                .Select(x => new DynamoDBTableModel(x))
                .ToList();
        }

    }
}