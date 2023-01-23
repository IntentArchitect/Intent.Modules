using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.CRUD.ServiceDispatch.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DTOExtensionModel : DTOModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DTOExtensionModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class DTOExtensionModelExtensions
    {

        public static bool HasProjectToDomainMapping(this DTOModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "01d74d4f-e478-4fde-a2f0-9ea92255f3c5";
        }

        public static IElementMapping GetProjectToDomainMapping(this DTOModel type)
        {
            return type.HasProjectToDomainMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}