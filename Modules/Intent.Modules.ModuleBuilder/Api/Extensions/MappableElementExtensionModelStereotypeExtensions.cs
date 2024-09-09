using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class MappableElementExtensionModelStereotypeExtensions
    {
        public static MappableExtensionSettings GetMappableExtensionSettings(this MappableElementExtensionModel model)
        {
            var stereotype = model.GetStereotype(MappableExtensionSettings.DefinitionId);
            return stereotype != null ? new MappableExtensionSettings(stereotype) : null;
        }


        public static bool HasMappableExtensionSettings(this MappableElementExtensionModel model)
        {
            return model.HasStereotype(MappableExtensionSettings.DefinitionId);
        }

        public static bool TryGetMappableExtensionSettings(this MappableElementExtensionModel model, out MappableExtensionSettings stereotype)
        {
            if (!HasMappableExtensionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappableExtensionSettings(model.GetStereotype(MappableExtensionSettings.DefinitionId));
            return true;
        }

        public class MappableExtensionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "dfd7cc0c-7c7a-47dc-b16d-d3d9f0b7a9b2";

            public MappableExtensionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ValidateFunction()
            {
                return _stereotype.GetProperty<string>("Validate Function");
            }

            public IElement[] ExtendTargetSettings()
            {
                return _stereotype.GetProperty<IElement[]>("Extend Target Settings") ?? new IElement[0];
            }

        }

    }
}