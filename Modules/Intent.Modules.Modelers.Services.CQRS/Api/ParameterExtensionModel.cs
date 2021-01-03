using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Merge)]
    public class ParameterExtensionModel : ParameterModel
    {
        [IntentManaged(Mode.Ignore)]
        public ParameterExtensionModel(IElement element) : base(element)
        {
        }

    }
}