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
    public class ServiceExtensionModel : ServiceModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServiceExtensionModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class ServiceExtensionModelExtensions
    {

        public static bool HasMapFromFolderMapping(this ServiceModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "0c594279-e76c-4c49-8512-dd362a49f302";
        }

        public static IElementMapping GetMapFromFolderMapping(this ServiceModel type)
        {
            return type.HasMapFromFolderMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}