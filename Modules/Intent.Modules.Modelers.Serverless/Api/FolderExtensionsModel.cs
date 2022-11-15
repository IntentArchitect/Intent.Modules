using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<PayloadModel> Payloads => _element.ChildElements
            .GetElementsOfType(PayloadModel.SpecializationTypeId)
            .Select(x => new PayloadModel(x))
            .ToList();

    }
}