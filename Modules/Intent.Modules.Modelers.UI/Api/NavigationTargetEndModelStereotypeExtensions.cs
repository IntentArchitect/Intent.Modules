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
    public static class NavigationTargetEndModelStereotypeExtensions
    {
        public static LayoutPlacement GetLayoutPlacement(this NavigationTargetEndModel model)
        {
            var stereotype = model.GetStereotype(LayoutPlacement.DefinitionId);
            return stereotype != null ? new LayoutPlacement(stereotype) : null;
        }


        public static bool HasLayoutPlacement(this NavigationTargetEndModel model)
        {
            return model.HasStereotype(LayoutPlacement.DefinitionId);
        }

        public static bool TryGetLayoutPlacement(this NavigationTargetEndModel model, out LayoutPlacement stereotype)
        {
            if (!HasLayoutPlacement(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LayoutPlacement(model.GetStereotype(LayoutPlacement.DefinitionId));
            return true;
        }

        public class LayoutPlacement
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "78475d50-f0e3-4fc7-96bd-1a6df1e8f71d";

            public LayoutPlacement(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement[] Regions()
            {
                return _stereotype.GetProperty<IElement[]>("Regions") ?? new IElement[0];
            }

        }

    }
}