using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.GraphQL.Api
{
    public static class ApiMetadataProviderExtensions
    {

        public static IList<GraphQLMutationTypeModel> GetGraphQLMutationTypeModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(GraphQLMutationTypeModel.SpecializationTypeId)
                .Select(x => new GraphQLMutationTypeModel(x))
                .ToList();
        }

        public static IList<GraphQLQueryTypeModel> GetGraphQLQueryTypeModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(GraphQLQueryTypeModel.SpecializationTypeId)
                .Select(x => new GraphQLQueryTypeModel(x))
                .ToList();
        }

        public static IList<GraphQLSubscriptionTypeModel> GetGraphQLSubscriptionTypeModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(GraphQLSubscriptionTypeModel.SpecializationTypeId)
                .Select(x => new GraphQLSubscriptionTypeModel(x))
                .ToList();
        }

    }
}