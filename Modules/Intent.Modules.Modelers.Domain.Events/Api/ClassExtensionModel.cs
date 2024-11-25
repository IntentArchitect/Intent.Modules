using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ClassExtensionModel : ClassModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ClassExtensionModel(IElement element) : base(element)
        {
        }

    }
}