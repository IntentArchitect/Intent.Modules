using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Fully)]
    public class AWSServerlessPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "AWS Serverless Package";
        public const string SpecializationTypeId = "a95a0fde-f67b-4e4a-b9a8-745021efbdb6";

        [IntentManaged(Mode.Ignore)]
        public AWSServerlessPackageModel(IPackage package)
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

        public IList<APIGatewayEndpointModel> APIEndpoints => UnderlyingPackage.ChildElements
    .GetElementsOfType(APIGatewayEndpointModel.SpecializationTypeId)
    .Select(x => new APIGatewayEndpointModel(x))
    .ToList();

        public IList<DTOModel> DTOs => UnderlyingPackage.ChildElements
            .GetElementsOfType(DTOModel.SpecializationTypeId)
            .Select(x => new DTOModel(x))
            .ToList();

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<LambdaFunctionModel> LambdaFunctions => UnderlyingPackage.ChildElements
            .GetElementsOfType(LambdaFunctionModel.SpecializationTypeId)
            .Select(x => new LambdaFunctionModel(x))
            .ToList();

        public IList<SQSQueueModel> SQSQueues => UnderlyingPackage.ChildElements
            .GetElementsOfType(SQSQueueModel.SpecializationTypeId)
            .Select(x => new SQSQueueModel(x))
            .ToList();

        public IList<DiagramModel> Diagrams => UnderlyingPackage.ChildElements
            .GetElementsOfType(DiagramModel.SpecializationTypeId)
            .Select(x => new DiagramModel(x))
            .ToList();

    }
}