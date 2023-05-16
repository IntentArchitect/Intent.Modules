using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.GraphQL.Api
{
    public static class ApiMetadataPackageExtensions
    {
        public static IList<GraphQLServicesPackageModel> GetGraphQLServicesPackageModels(this IDesigner designer)
        {
            return designer.GetPackagesOfType(GraphQLServicesPackageModel.SpecializationTypeId)
                .Select(x => new GraphQLServicesPackageModel(x))
                .ToList();
        }

        public static bool IsGraphQLServicesPackageModel(this IPackage package)
        {
            return package?.SpecializationTypeId == GraphQLServicesPackageModel.SpecializationTypeId;
        }


    }
}