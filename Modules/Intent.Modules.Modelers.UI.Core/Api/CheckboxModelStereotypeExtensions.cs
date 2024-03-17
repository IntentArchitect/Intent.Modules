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
    public static class CheckboxModelStereotypeExtensions
    {
        public static Content GetContent(this CheckboxModel model)
        {
            var stereotype = model.GetStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20");
            return stereotype != null ? new Content(stereotype) : null;
        }


        public static bool HasContent(this CheckboxModel model)
        {
            return model.HasStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20");
        }

        public static bool TryGetContent(this CheckboxModel model, out Content stereotype)
        {
            if (!HasContent(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Content(model.GetStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20"));
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

        }

    }
}