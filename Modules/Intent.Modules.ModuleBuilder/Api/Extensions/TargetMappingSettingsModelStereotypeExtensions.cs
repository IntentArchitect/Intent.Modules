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
    public static class TargetMappingSettingsModelStereotypeExtensions
    {
        public static MappingTypeSettings GetMappingTypeSettings(this TargetMappingSettingsModel model)
        {
            var stereotype = model.GetStereotype("Mapping Type Settings");
            return stereotype != null ? new MappingTypeSettings(stereotype) : null;
        }


        public static bool HasMappingTypeSettings(this TargetMappingSettingsModel model)
        {
            return model.HasStereotype("Mapping Type Settings");
        }

        public static bool TryGetMappingTypeSettings(this TargetMappingSettingsModel model, out MappingTypeSettings stereotype)
        {
            if (!HasMappingTypeSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappingTypeSettings(model.GetStereotype("Mapping Type Settings"));
            return true;
        }

        public class MappingTypeSettings
        {
            private IStereotype _stereotype;

            public MappingTypeSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string RootElementFunction()
            {
                return _stereotype.GetProperty<string>("Root Element Function");
            }

        }

    }
}