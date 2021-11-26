using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

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

        public static bool HasMapToCommandMapping(this OperationModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "e4e36072-a2ce-42d3-93c5-0b496d79ef43";
        }

        public static IElementMapping GetMapToCommandMapping(this OperationModel type)
        {
            return type.HasMapToCommandMapping() ? type.InternalElement.MappedElement : null;
        }

        public static bool HasMapToQueryMapping(this OperationModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "f15fa935-434e-4983-b283-25cde2cb440e";
        }

        public static IElementMapping GetMapToQueryMapping(this OperationModel type)
        {
            return type.HasMapToQueryMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}