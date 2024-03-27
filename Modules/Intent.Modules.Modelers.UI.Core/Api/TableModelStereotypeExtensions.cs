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
    public static class TableModelStereotypeExtensions
    {
        public static Interaction GetInteraction(this TableModel model)
        {
            var stereotype = model.GetStereotype("74533b82-2078-44e1-84bb-4cd34d00ef16");
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this TableModel model)
        {
            return model.HasStereotype("74533b82-2078-44e1-84bb-4cd34d00ef16");
        }

        public static bool TryGetInteraction(this TableModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype("74533b82-2078-44e1-84bb-4cd34d00ef16"));
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

            public string OnRowClick()
            {
                return _stereotype.GetProperty<string>("On Row Click");
            }

        }

    }
}