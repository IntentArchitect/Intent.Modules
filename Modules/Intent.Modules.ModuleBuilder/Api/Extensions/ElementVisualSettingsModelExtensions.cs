using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ElementVisualSettingsModelExtensions
    {
        public static PositionSettings GetPositionSettings(this ElementVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("Position Settings");
            return stereotype != null ? new PositionSettings(stereotype) : null;
        }

        public static bool HasPositionSettings(this ElementVisualSettingsModel model)
        {
            return model.HasStereotype("Position Settings");
        }

        public static Settings GetSettings(this ElementVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("Settings");
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this ElementVisualSettingsModel model)
        {
            return model.HasStereotype("Settings");
        }


        public class PositionSettings
        {
            private IStereotype _stereotype;

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

        public class Settings
        {
            private IStereotype _stereotype;

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement TargetType()
            {
                return _stereotype.GetProperty<IElement>("Target Type");
            }

        }

    }
}