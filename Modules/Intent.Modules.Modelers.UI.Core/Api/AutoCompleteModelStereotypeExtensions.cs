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
    public static class AutoCompleteModelStereotypeExtensions
    {
        public static Interaction GetInteraction(this AutoCompleteModel model)
        {
            var stereotype = model.GetStereotype(Interaction.DefinitionId);
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this AutoCompleteModel model)
        {
            return model.HasStereotype(Interaction.DefinitionId);
        }

        public static bool TryGetInteraction(this AutoCompleteModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype(Interaction.DefinitionId));
            return true;
        }

        public static LabelAddon GetLabelAddon(this AutoCompleteModel model)
        {
            var stereotype = model.GetStereotype(LabelAddon.DefinitionId);
            return stereotype != null ? new LabelAddon(stereotype) : null;
        }


        public static bool HasLabelAddon(this AutoCompleteModel model)
        {
            return model.HasStereotype(LabelAddon.DefinitionId);
        }

        public static bool TryGetLabelAddon(this AutoCompleteModel model, out LabelAddon stereotype)
        {
            if (!HasLabelAddon(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LabelAddon(model.GetStereotype(LabelAddon.DefinitionId));
            return true;
        }

        public static Secured GetSecured(this AutoCompleteModel model)
        {
            var stereotype = model.GetStereotype(Secured.DefinitionId);
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static IReadOnlyCollection<Secured> GetSecureds(this AutoCompleteModel model)
        {
            var stereotypes = model
                .GetStereotypes(Secured.DefinitionId)
                .Select(stereotype => new Secured(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasSecured(this AutoCompleteModel model)
        {
            return model.HasStereotype(Secured.DefinitionId);
        }

        public static bool TryGetSecured(this AutoCompleteModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype(Secured.DefinitionId));
            return true;
        }

        public class Interaction
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "44dcdc3d-3445-435c-9013-197ab9836f09";

            public Interaction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string OnSelected()
            {
                return _stereotype.GetProperty<string>("On Selected");
            }

            public string SearchFunction()
            {
                return _stereotype.GetProperty<string>("Search Function");
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

        public class Secured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";

            public Secured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

            public string Policy()
            {
                return _stereotype.GetProperty<string>("Policy");
            }

        }

    }
}