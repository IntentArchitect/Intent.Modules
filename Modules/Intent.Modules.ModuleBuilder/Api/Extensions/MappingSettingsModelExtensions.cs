using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class MappingSettingsModelExtensions
    {
        public static MappingSettings GetMappingSettings(this MappingSettingsModel model)
        {
            var stereotype = model.GetStereotype("Mapping Settings");
            return stereotype != null ? new MappingSettings(stereotype) : null;
        }

        public static bool HasMappingSettings(this MappingSettingsModel model)
        {
            return model.HasStereotype("Mapping Settings");
        }


        public class MappingSettings
        {
            private IStereotype _stereotype;

            public MappingSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement DefaultDesigner()
            {
                return _stereotype.GetProperty<IElement>("Default Designer");
            }

        }

    }
}