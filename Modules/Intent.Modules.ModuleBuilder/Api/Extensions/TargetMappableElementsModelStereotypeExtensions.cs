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
    public static class TargetMappableElementsModelStereotypeExtensions
    {
        public static MappingEndSettings GetMappingEndSettings(this TargetMappableElementsModel model)
        {
            var stereotype = model.GetStereotype("Mapping End Settings");
            return stereotype != null ? new MappingEndSettings(stereotype) : null;
        }


        public static bool HasMappingEndSettings(this TargetMappableElementsModel model)
        {
            return model.HasStereotype("Mapping End Settings");
        }

        public static bool TryGetMappingEndSettings(this TargetMappableElementsModel model, out MappingEndSettings stereotype)
        {
            if (!HasMappingEndSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappingEndSettings(model.GetStereotype("Mapping End Settings"));
            return true;
        }

        public class MappingEndSettings
        {
            private IStereotype _stereotype;

            public MappingEndSettings(IStereotype stereotype)
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