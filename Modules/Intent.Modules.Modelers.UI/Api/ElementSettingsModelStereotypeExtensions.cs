using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class ElementSettingsModelStereotypeExtensions
    {
        public static Component GetComponent(this ElementSettingsModel model)
        {
            var stereotype = model.GetStereotype("b407aace-ad14-484e-ac93-4762a3d182d8");
            return stereotype != null ? new Component(stereotype) : null;
        }


        public static bool HasComponent(this ElementSettingsModel model)
        {
            return model.HasStereotype("b407aace-ad14-484e-ac93-4762a3d182d8");
        }

        public static bool TryGetComponent(this ElementSettingsModel model, out Component stereotype)
        {
            if (!HasComponent(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Component(model.GetStereotype("b407aace-ad14-484e-ac93-4762a3d182d8"));
            return true;
        }

        public class Component
        {
            private IStereotype _stereotype;

            public Component(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}