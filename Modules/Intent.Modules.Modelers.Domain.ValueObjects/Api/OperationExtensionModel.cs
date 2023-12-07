using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.ValueObjects.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class OperationExtensionModel : OperationModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OperationExtensionModel(IElement element) : base(element)
        {
        }

    }
}