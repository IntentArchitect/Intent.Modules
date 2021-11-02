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
    public class OperationExtensionModel : OperationModel
    {
        [IntentManaged(Mode.Ignore)]
        public OperationExtensionModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class OperationExtensionModelExtensions
    {

        public static bool HasMapToRequestMapping(this OperationModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "f15fa935-434e-4983-b283-25cde2cb440e";
        }

        public static IElementMapping GetMapToRequestMapping(this OperationModel type)
        {
            return type.HasMapToRequestMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}