using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    public static class ContainerModelStereotypeExtensions
    {
        public static Appearance GetAppearance(this ContainerModel model)
        {
            var stereotype = model.GetStereotype("7513c78d-4825-442f-9e1d-5cb76fde6de9");
            return stereotype != null ? new Appearance(stereotype) : null;
        }


        public static bool HasAppearance(this ContainerModel model)
        {
            return model.HasStereotype("7513c78d-4825-442f-9e1d-5cb76fde6de9");
        }

        public static bool TryGetAppearance(this ContainerModel model, out Appearance stereotype)
        {
            if (!HasAppearance(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Appearance(model.GetStereotype("7513c78d-4825-442f-9e1d-5cb76fde6de9"));
            return true;
        }

        public class Appearance
        {
            private IStereotype _stereotype;

            public Appearance(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}