using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class UIElementSettingsExtensionModel : ElementSettingsModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UIElementSettingsExtensionModel(IElement element) : base(element)
        {
        }

        public IList<PropertyModel> Properties => _element.ChildElements
            .GetElementsOfType(PropertyModel.SpecializationTypeId)
            .Select(x => new PropertyModel(x))
            .ToList();

        public IList<EventEmitterModel> EventEmitters => _element.ChildElements
            .GetElementsOfType(EventEmitterModel.SpecializationTypeId)
            .Select(x => new EventEmitterModel(x))
            .ToList();

    }
}