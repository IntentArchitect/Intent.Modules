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
            var stereotype = model.GetStereotype(Interaction.DefinitionId);
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this CheckboxModel model)
        {
            return model.HasStereotype(Interaction.DefinitionId);
        }

        public static bool TryGetInteraction(this CheckboxModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype(Interaction.DefinitionId));
            return true;
        }

        public static LabelAddon GetLabelAddon(this CheckboxModel model)
        {
            var stereotype = model.GetStereotype(LabelAddon.DefinitionId);
            return stereotype != null ? new LabelAddon(stereotype) : null;
        }


        public static bool HasLabelAddon(this CheckboxModel model)
        {
            return model.HasStereotype(LabelAddon.DefinitionId);
        }

        public static bool TryGetLabelAddon(this CheckboxModel model, out LabelAddon stereotype)
        {
            if (!HasLabelAddon(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LabelAddon(model.GetStereotype(LabelAddon.DefinitionId));
            return true;
        }

        public class Interaction
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6e04ff81-f043-4ac6-8632-798aedbaaf20";

            public Interaction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string OnValueChanged()
            {
                return _stereotype.GetProperty<string>("On Value Changed");
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
            public const string DefinitionId = "2c099977-e5ca-4a80-ba70-6f2edc593681";

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