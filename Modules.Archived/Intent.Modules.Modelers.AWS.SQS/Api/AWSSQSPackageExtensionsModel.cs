using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.SQS.Api
{
    [IntentManaged(Mode.Merge)]
    public class AWSSQSPackageExtensionsModel : AWSPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public AWSSQSPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<SQSQueueModel> SQSQueues => UnderlyingPackage.ChildElements
            .GetElementsOfType(SQSQueueModel.SpecializationTypeId)
            .Select(x => new SQSQueueModel(x))
            .ToList();

    }
}