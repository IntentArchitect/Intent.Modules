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
    public static class StereotypesVisualSettingsModelStereotypeExtensions
    {
        public static PositionSettings GetPositionSettings(this StereotypesVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype(PositionSettings.DefinitionId);
            return stereotype != null ? new PositionSettings(stereotype) : null;
        }

        public static bool HasPositionSettings(this StereotypesVisualSettingsModel model)
        {
            return model.HasStereotype(PositionSettings.DefinitionId);
        }

        public static bool TryGetPositionSettings(this StereotypesVisualSettingsModel model, out PositionSettings stereotype)
        {
            if (!HasPositionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PositionSettings(model.GetStereotype(PositionSettings.DefinitionId));
            return true;
        }


        public class PositionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fc50924a-9e51-40b9-88b6-c7556febdaea";

            public PositionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string X()
            {
                return _stereotype.GetProperty<string>("X");
            }

            public string Y()
            {
                return _stereotype.GetProperty<string>("Y");
            }

            public string Width()
            {
                return _stereotype.GetProperty<string>("Width");
            }

            public string Height()
            {
                return _stereotype.GetProperty<string>("Height");
            }

        }

    }
}