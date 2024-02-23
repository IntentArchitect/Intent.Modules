using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class TextInputModelStereotypeExtensions
    {
        public static LabelAddon GetLabelAddon(this TextInputModel model)
        {
            var stereotype = model.GetStereotype("52c441a0-ad03-45e9-9d46-7babedfc10ff");
            return stereotype != null ? new LabelAddon(stereotype) : null;
        }


        public static bool HasLabelAddon(this TextInputModel model)
        {
            return model.HasStereotype("52c441a0-ad03-45e9-9d46-7babedfc10ff");
        }

        public static bool TryGetLabelAddon(this TextInputModel model, out LabelAddon stereotype)
        {
            if (!HasLabelAddon(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LabelAddon(model.GetStereotype("52c441a0-ad03-45e9-9d46-7babedfc10ff"));
            return true;
        }

        public class LabelAddon
        {
            private IStereotype _stereotype;

            public LabelAddon(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Label()
            {
                return _stereotype.GetProperty<string>("Label");
            }

        }

    }
}