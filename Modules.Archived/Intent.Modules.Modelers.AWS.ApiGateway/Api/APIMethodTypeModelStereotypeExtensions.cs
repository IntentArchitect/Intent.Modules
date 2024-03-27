using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.ApiGateway.Api
{
    public static class APIMethodTypeModelStereotypeExtensions
    {
        public static APIMethodTypeSettings GetAPIMethodTypeSettings(this APIMethodTypeModel model)
        {
            var stereotype = model.GetStereotype("API Method Type Settings");
            return stereotype != null ? new APIMethodTypeSettings(stereotype) : null;
        }


        public static bool HasAPIMethodTypeSettings(this APIMethodTypeModel model)
        {
            return model.HasStereotype("API Method Type Settings");
        }

        public static bool TryGetAPIMethodTypeSettings(this APIMethodTypeModel model, out APIMethodTypeSettings stereotype)
        {
            if (!HasAPIMethodTypeSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new APIMethodTypeSettings(model.GetStereotype("API Method Type Settings"));
            return true;
        }

        public class APIMethodTypeSettings
        {
            private IStereotype _stereotype;

            public APIMethodTypeSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

        }

    }
}