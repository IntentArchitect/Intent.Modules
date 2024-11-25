using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    [IntentManaged(Mode.Merge)]
    public class AttributeExtensionModel : AttributeModel
    {
        [IntentManaged(Mode.Ignore)]
        public AttributeExtensionModel(IElement element) : base(element)
        {
        }

    }
}