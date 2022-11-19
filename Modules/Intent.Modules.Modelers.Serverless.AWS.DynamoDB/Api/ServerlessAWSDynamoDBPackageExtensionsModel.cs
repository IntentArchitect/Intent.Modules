using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Modelers.Serverless.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServerlessAWSDynamoDBPackageExtensionsModel : ServerlessPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServerlessAWSDynamoDBPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        public IList<DynamoDBModel> DynamoDBs => UnderlyingPackage.ChildElements
            .GetElementsOfType(DynamoDBModel.SpecializationTypeId)
            .Select(x => new DynamoDBModel(x))
            .ToList();

    }
}