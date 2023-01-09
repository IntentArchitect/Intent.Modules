using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.CDK.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSCDKStackFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSCDKStackFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<StackModel> Stacks => _element.ChildElements
            .GetElementsOfType(StackModel.SpecializationTypeId)
            .Select(x => new StackModel(x))
            .ToList();

    }
}