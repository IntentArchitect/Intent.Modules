using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.GraphQL.Api
{
    [IntentManaged(Mode.Fully)]
    public class GraphQLServicesPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "GraphQL Services Package";
        public const string SpecializationTypeId = "4aeae542-361b-4c38-b575-10f0224b17d5";

        [IntentManaged(Mode.Ignore)]
        public GraphQLServicesPackageModel(IPackage package)
        {
            if (!SpecializationType.Equals(package.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
        }

        public IPackage UnderlyingPackage { get; }
        public string Id => UnderlyingPackage.Id;
        public string Name => UnderlyingPackage.Name;
        public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;
        public string FileLocation => UnderlyingPackage.FileLocation;

        public IList<GraphQLQueryTypeModel> QueryTypes => UnderlyingPackage.ChildElements
            .GetElementsOfType(GraphQLQueryTypeModel.SpecializationTypeId)
            .Select(x => new GraphQLQueryTypeModel(x))
            .ToList();

        public IList<GraphQLMutationTypeModel> MutationTypes => UnderlyingPackage.ChildElements
            .GetElementsOfType(GraphQLMutationTypeModel.SpecializationTypeId)
            .Select(x => new GraphQLMutationTypeModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => UnderlyingPackage.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

    }
}