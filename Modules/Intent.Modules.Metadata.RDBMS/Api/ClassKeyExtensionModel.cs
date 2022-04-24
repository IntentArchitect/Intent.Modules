using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ClassKeyExtensionModel : ClassModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ClassKeyExtensionModel(IElement element) : base(element)
        {
        }

    }
}