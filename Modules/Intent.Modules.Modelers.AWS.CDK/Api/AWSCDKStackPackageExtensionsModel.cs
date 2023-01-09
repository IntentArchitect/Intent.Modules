using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.CDK.Api
{
    [IntentManaged(Mode.Merge)]
    public class AWSCDKStackPackageExtensionsModel : AWSPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public AWSCDKStackPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<StackModel> Stacks => UnderlyingPackage.ChildElements
            .GetElementsOfType(StackModel.SpecializationTypeId)
            .Select(x => new StackModel(x))
            .ToList();

    }
}