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
    public static class ButtonModelStereotypeExtensions
    {
        public static Interaction GetInteraction(this ButtonModel model)
        {
            var stereotype = model.GetStereotype("a7de29e5-4bee-4e5d-93f0-740569ac6130");
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this ButtonModel model)
        {
            return model.HasStereotype("a7de29e5-4bee-4e5d-93f0-740569ac6130");
        }

        public static bool TryGetInteraction(this ButtonModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype("a7de29e5-4bee-4e5d-93f0-740569ac6130"));
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

            public string OnClick()
            {
                return _stereotype.GetProperty<string>("On Click");
            }

        }

    }
}