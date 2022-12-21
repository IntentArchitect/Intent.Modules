using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Gateway.Api
{
    [IntentManaged(Mode.Merge)]
    public class AWSGatewayPackageExtensionsModel : AWSPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public AWSGatewayPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<APIGatewayEndpointModel> APIEndpoints => UnderlyingPackage.ChildElements
            .GetElementsOfType(APIGatewayEndpointModel.SpecializationTypeId)
            .Select(x => new APIGatewayEndpointModel(x))
            .ToList();

    }
}