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
    public static class MappableElementSettingsModelStereotypeExtensions
    {
        public static MappingSettings GetMappingSettings(this MappableElementSettingsModel model)
        {
            var stereotype = model.GetStereotype("Mapping Settings");
            return stereotype != null ? new MappingSettings(stereotype) : null;
        }


        public static bool HasMappingSettings(this MappableElementSettingsModel model)
        {
            return model.HasStereotype("Mapping Settings");
        }

        public static bool TryGetMappingSettings(this MappableElementSettingsModel model, out MappingSettings stereotype)
        {
            if (!HasMappingSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappingSettings(model.GetStereotype("Mapping Settings"));
            return true;
        }

        public class MappingSettings
        {
            private IStereotype _stereotype;

            public MappingSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool CanBeModified()
            {
                return _stereotype.GetProperty<bool>("Can Be Modified");
            }

            public string CreateNameFunction()
            {
                return _stereotype.GetProperty<string>("Create Name Function");
            }

            public string FilterFunction()
            {
                return _stereotype.GetProperty<string>("Filter Function");
            }

            public string IsRequiredFunction()
            {
                return _stereotype.GetProperty<string>("Is Required Function");
            }

            public string IsMappableFunction()
            {
                return _stereotype.GetProperty<string>("Is Mappable Function");
            }

            public bool AllowMultipleMappings()
            {
                return _stereotype.GetProperty<bool>("Allow Multiple Mappings");
            }

            public IElement[] TraversableTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Traversable Types") ?? new IElement[0];
            }

            public string IsTraversableFunction()
            {
                return _stereotype.GetProperty<string>("Is Traversable Function");
            }

            public IElement UseChildMappingsFrom()
            {
                return _stereotype.GetProperty<IElement>("Use Child Mappings From");
            }

        }

    }
}