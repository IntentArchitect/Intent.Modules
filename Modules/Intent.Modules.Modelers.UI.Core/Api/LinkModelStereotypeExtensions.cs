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
    public static class LinkModelStereotypeExtensions
    {
        public static Interaction GetInteraction(this LinkModel model)
        {
            var stereotype = model.GetStereotype(Interaction.DefinitionId);
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this LinkModel model)
        {
            return model.HasStereotype(Interaction.DefinitionId);
        }

        public static bool TryGetInteraction(this LinkModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype(Interaction.DefinitionId));
            return true;
        }

        public class Interaction
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5d0a10f7-ef5c-4f1e-af13-3471671b46a7";

            public Interaction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string OnClick()
            {
                return _stereotype.GetProperty<string>("On Click");
            }

            public string LinkTo()
            {
                return _stereotype.GetProperty<string>("Link To");
            }

        }

    }
}