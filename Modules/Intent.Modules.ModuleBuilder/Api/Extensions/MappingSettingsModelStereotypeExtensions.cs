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
    public static class MappingSettingsModelStereotypeExtensions
    {
        public static MappingSettings GetMappingSettings(this MappingSettingsModel model)
        {
            var stereotype = model.GetStereotype(MappingSettings.DefinitionId);
            return stereotype != null ? new MappingSettings(stereotype) : null;
        }


        public static bool HasMappingSettings(this MappingSettingsModel model)
        {
            return model.HasStereotype(MappingSettings.DefinitionId);
        }

        public static bool TryGetMappingSettings(this MappingSettingsModel model, out MappingSettings stereotype)
        {
            if (!HasMappingSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappingSettings(model.GetStereotype(MappingSettings.DefinitionId));
            return true;
        }

        public class MappingSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "cb5032a1-f531-40ee-8fe5-824ed8b89946";

            public MappingSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string IsRequiredFunction()
            {
                return _stereotype.GetProperty<string>("Is Required Function");
            }

        }

    }
}