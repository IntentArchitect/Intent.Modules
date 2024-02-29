using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.AppSync.Api
{
    [IntentManaged(Mode.Merge)]
    public class GraphQLEndpointPackageExtensionsModel : AWSPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public GraphQLEndpointPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<GraphQLEndpointModel> GraphQLEndpoints => UnderlyingPackage.ChildElements
            .GetElementsOfType(GraphQLEndpointModel.SpecializationTypeId)
            .Select(x => new GraphQLEndpointModel(x))
            .ToList();

    }
}