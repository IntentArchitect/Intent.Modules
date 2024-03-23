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
        public static Interaction GetInteraction(this CheckboxModel model)
        {
            var stereotype = model.GetStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20");
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this CheckboxModel model)
        {
            return model.HasStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20");
        }

        public static bool TryGetInteraction(this CheckboxModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype("6e04ff81-f043-4ac6-8632-798aedbaaf20"));
            return true;
        }

        public class Interaction
        {
            private IStereotype _stereotype;

            public Interaction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsRequired()
            {
                return _stereotype.GetProperty<bool>("Is Required");
            }

            public bool IsEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Enabled");
            }

        }

    }
}