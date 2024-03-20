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
    public static class TextInputModelStereotypeExtensions
    {
        public static Content GetContent(this TextInputModel model)
        {
            var stereotype = model.GetStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20");
            return stereotype != null ? new Content(stereotype) : null;
        }


        public static bool HasContent(this TextInputModel model)
        {
            return model.HasStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20");
        }

        public static bool TryGetContent(this TextInputModel model, out Content stereotype)
        {
            if (!HasContent(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Content(model.GetStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20"));
            return true;
        }
        public static LabelAddon GetLabelAddon(this TextInputModel model)
        {
            var stereotype = model.GetStereotype("2c099977-e5ca-4a80-ba70-6f2edc593681");
            return stereotype != null ? new LabelAddon(stereotype) : null;
        }


        public static bool HasLabelAddon(this TextInputModel model)
        {
            return model.HasStereotype("2c099977-e5ca-4a80-ba70-6f2edc593681");
        }

        public static bool TryGetLabelAddon(this TextInputModel model, out LabelAddon stereotype)
        {
            if (!HasLabelAddon(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LabelAddon(model.GetStereotype("2c099977-e5ca-4a80-ba70-6f2edc593681"));
            return true;
        }

        public class Content
        {
            private IStereotype _stereotype;

            public Content(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Model()
            {
                return _stereotype.GetProperty<string>("Model");
            }

            public bool IsRequired()
            {
                return _stereotype.GetProperty<bool>("Is Required");
            }

            public bool IsEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Enabled");
            }

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