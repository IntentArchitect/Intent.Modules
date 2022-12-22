using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSStepFunctionsFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSStepFunctionsFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<StateMachineModel> StateMachines => _element.ChildElements
            .GetElementsOfType(StateMachineModel.SpecializationTypeId)
            .Select(x => new StateMachineModel(x))
            .ToList();

    }
}