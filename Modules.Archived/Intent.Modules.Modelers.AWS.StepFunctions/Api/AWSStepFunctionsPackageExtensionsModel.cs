using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    [IntentManaged(Mode.Merge)]
    public class AWSStepFunctionsPackageExtensionsModel : AWSPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public AWSStepFunctionsPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<StateMachineModel> StateMachines => UnderlyingPackage.ChildElements
            .GetElementsOfType(StateMachineModel.SpecializationTypeId)
            .Select(x => new StateMachineModel(x))
            .ToList();

    }
}