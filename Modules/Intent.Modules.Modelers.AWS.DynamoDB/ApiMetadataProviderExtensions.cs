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
        public static IList<DynamoDBMapAttributeModel> GetDynamoDBMapAttributeModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DynamoDBMapAttributeModel.SpecializationTypeId)
                .Select(x => new DynamoDBMapAttributeModel(x))
                .ToList();
        }
        public static IList<DynamoDBModel> GetDynamoDBModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DynamoDBModel.SpecializationTypeId)
                .Select(x => new DynamoDBModel(x))
                .ToList();
        }

        public static IList<DynamoDBItemModel> GetDynamoDBItemModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DynamoDBItemModel.SpecializationTypeId)
                .Select(x => new DynamoDBItemModel(x))
                .ToList();
        }

        public static IList<DynamoDBTableModel> GetDynamoDBTableModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DynamoDBTableModel.SpecializationTypeId)
                .Select(x => new DynamoDBTableModel(x))
                .ToList();
        }

    }
}