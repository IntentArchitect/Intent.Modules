using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Modelers.Serverless.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServerlessAWSPackageExtensionsModel : ServerlessPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServerlessAWSPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<AWSAPIGatewayEndpointModel> APIGatewayEndpoints => UnderlyingPackage.ChildElements
            .GetElementsOfType(AWSAPIGatewayEndpointModel.SpecializationTypeId)
            .Select(x => new AWSAPIGatewayEndpointModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<DTOModel> DTOs => UnderlyingPackage.ChildElements
            .GetElementsOfType(DTOModel.SpecializationTypeId)
            .Select(x => new DTOModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<AWSLambdaFunctionModel> LambdaFunctions => UnderlyingPackage.ChildElements
            .GetElementsOfType(AWSLambdaFunctionModel.SpecializationTypeId)
            .Select(x => new AWSLambdaFunctionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<AWSSimpleQueueServiceModel> SimpleQueueServices => UnderlyingPackage.ChildElements
            .GetElementsOfType(AWSSimpleQueueServiceModel.SpecializationTypeId)
            .Select(x => new AWSSimpleQueueServiceModel(x))
            .ToList();

    }
}