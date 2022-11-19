using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Serverless.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Merge)]
    public class DynamoDBPackageExtensionsModel : AWSServerlessPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public DynamoDBPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<DynamoDBModel> DynamoDBs => UnderlyingPackage.ChildElements
            .GetElementsOfType(DynamoDBModel.SpecializationTypeId)
            .Select(x => new DynamoDBModel(x))
            .ToList();

    }
}