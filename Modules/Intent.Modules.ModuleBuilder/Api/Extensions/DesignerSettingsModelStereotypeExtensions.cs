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
    public static class DesignerSettingsModelStereotypeExtensions
    {
        public static DesignerSettings GetDesignerSettings(this DesignerSettingsModel model)
        {
            var stereotype = model.GetStereotype(DesignerSettings.DefinitionId);
            return stereotype != null ? new DesignerSettings(stereotype) : null;
        }

        public static bool HasDesignerSettings(this DesignerSettingsModel model)
        {
            return model.HasStereotype(DesignerSettings.DefinitionId);
        }

        public static bool TryGetDesignerSettings(this DesignerSettingsModel model, out DesignerSettings stereotype)
        {
            if (!HasDesignerSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DesignerSettings(model.GetStereotype(DesignerSettings.DefinitionId));
            return true;
        }


        public class DesignerSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "81e5637b-05d7-43a4-9619-6ef873be9f47";

            public DesignerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsReference()
            {
                return _stereotype.GetProperty<bool>("Is Reference");
            }

            public IElement[] ExtendDesigners()
            {
                return _stereotype.GetProperty<IElement[]>("Extend Designers") ?? new IElement[0];
            }

        }

    }
}