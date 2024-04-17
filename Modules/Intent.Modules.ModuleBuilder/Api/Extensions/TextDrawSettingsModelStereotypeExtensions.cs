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
            var stereotype = model.GetStereotype(PositionSettings.DefinitionId);
            return stereotype != null ? new PositionSettings(stereotype) : null;
        }

        public static bool HasPositionSettings(this TextDrawSettingsModel model)
        {
            return model.HasStereotype(PositionSettings.DefinitionId);
        }

        public static bool TryGetPositionSettings(this TextDrawSettingsModel model, out PositionSettings stereotype)
        {
            if (!HasPositionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PositionSettings(model.GetStereotype(PositionSettings.DefinitionId));
            return true;
        }

        public static TextSettings GetTextSettings(this TextDrawSettingsModel model)
        {
            var stereotype = model.GetStereotype(TextSettings.DefinitionId);
            return stereotype != null ? new TextSettings(stereotype) : null;
        }

        public static bool HasTextSettings(this TextDrawSettingsModel model)
        {
            return model.HasStereotype(TextSettings.DefinitionId);
        }

        public static bool TryGetTextSettings(this TextDrawSettingsModel model, out TextSettings stereotype)
        {
            if (!HasTextSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TextSettings(model.GetStereotype(TextSettings.DefinitionId));
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

        public class TextSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1cb12b3c-f000-4331-b3fc-4250a5ced3fa";

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