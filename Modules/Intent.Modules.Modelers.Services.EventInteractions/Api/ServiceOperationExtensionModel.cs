using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ServiceOperationExtensionModel : OperationModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceOperationExtensionModel(IElement element) : base(element)
        {
        }

    }
}