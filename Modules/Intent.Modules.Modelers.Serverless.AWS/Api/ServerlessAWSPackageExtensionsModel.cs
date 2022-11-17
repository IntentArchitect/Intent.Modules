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

        public IList<AWSAPIGatewayEndpointModel> AWSAPIGatewayEndpoints => UnderlyingPackage.ChildElements
            .GetElementsOfType(AWSAPIGatewayEndpointModel.SpecializationTypeId)
            .Select(x => new AWSAPIGatewayEndpointModel(x))
            .ToList();

        public IList<AWSLambdaFunctionModel> AWSLambdaFunctions => UnderlyingPackage.ChildElements
            .GetElementsOfType(AWSLambdaFunctionModel.SpecializationTypeId)
            .Select(x => new AWSLambdaFunctionModel(x))
            .ToList();

        public IList<DTOModel> DTOs => UnderlyingPackage.ChildElements
            .GetElementsOfType(DTOModel.SpecializationTypeId)
            .Select(x => new DTOModel(x))
            .ToList();

    }
}