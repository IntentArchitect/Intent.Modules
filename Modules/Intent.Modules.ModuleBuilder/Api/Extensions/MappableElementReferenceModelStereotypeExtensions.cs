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
    public static class MappableElementReferenceModelStereotypeExtensions
    {
        public static MappableReferenceSettings GetMappableReferenceSettings(this MappableElementReferenceModel model)
        {
            var stereotype = model.GetStereotype("Mappable Reference Settings");
            return stereotype != null ? new MappableReferenceSettings(stereotype) : null;
        }


        public static bool HasMappableReferenceSettings(this MappableElementReferenceModel model)
        {
            return model.HasStereotype("Mappable Reference Settings");
        }

        public static bool TryGetMappableReferenceSettings(this MappableElementReferenceModel model, out MappableReferenceSettings stereotype)
        {
            if (!HasMappableReferenceSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappableReferenceSettings(model.GetStereotype("Mappable Reference Settings"));
            return true;
        }

        public class MappableReferenceSettings
        {
            private IStereotype _stereotype;

            public MappableReferenceSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement ReferenceTarget()
            {
                return _stereotype.GetProperty<IElement>("Reference Target");
            }

        }

    }
}