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
    public static class TextDrawSettingsModelStereotypeExtensions
    {
        public static PositionSettings GetPositionSettings(this TextDrawSettingsModel model)
        {
            var stereotype = model.GetStereotype("Position Settings");
            return stereotype != null ? new PositionSettings(stereotype) : null;
        }

        public static bool HasPositionSettings(this TextDrawSettingsModel model)
        {
            return model.HasStereotype("Position Settings");
        }

        public static TextSettings GetTextSettings(this TextDrawSettingsModel model)
        {
            var stereotype = model.GetStereotype("Text Settings");
            return stereotype != null ? new TextSettings(stereotype) : null;
        }

        public static bool HasTextSettings(this TextDrawSettingsModel model)
        {
            return model.HasStereotype("Text Settings");
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

        public class TextSettings
        {
            private IStereotype _stereotype;

            public TextSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Text()
            {
                return _stereotype.GetProperty<string>("Text");
            }

            public string Condition()
            {
                return _stereotype.GetProperty<string>("Condition");
            }

            public string Style()
            {
                return _stereotype.GetProperty<string>("Style");
            }

        }

    }
}