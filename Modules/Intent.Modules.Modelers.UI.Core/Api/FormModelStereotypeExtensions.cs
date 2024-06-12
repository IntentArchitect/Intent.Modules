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
    public static class FormModelStereotypeExtensions
    {
        public static Content GetContent(this FormModel model)
        {
            var stereotype = model.GetStereotype(Content.DefinitionId);
            return stereotype != null ? new Content(stereotype) : null;
        }


        public static bool HasContent(this FormModel model)
        {
            return model.HasStereotype(Content.DefinitionId);
        }

        public static bool TryGetContent(this FormModel model, out Content stereotype)
        {
            if (!HasContent(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Content(model.GetStereotype(Content.DefinitionId));
            return true;
        }

        public static Interaction GetInteraction(this FormModel model)
        {
            var stereotype = model.GetStereotype(Interaction.DefinitionId);
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this FormModel model)
        {
            return model.HasStereotype(Interaction.DefinitionId);
        }

        public static bool TryGetInteraction(this FormModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype(Interaction.DefinitionId));
            return true;
        }

        public class Content
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7f7bc148-138f-4b61-8fa1-a8d725dbed52";

            public Content(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Model()
            {
                return _stereotype.GetProperty<string>("Model");
            }

            public bool Header()
            {
                return _stereotype.GetProperty<bool>("Header");
            }

            public bool Body()
            {
                return _stereotype.GetProperty<bool>("Body");
            }

            public bool Footer()
            {
                return _stereotype.GetProperty<bool>("Footer");
            }

        }

        public class Interaction
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "e242cccf-5826-478f-9e7a-c1fd9df4e5c8";

            public Interaction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string OnSubmit()
            {
                return _stereotype.GetProperty<string>("On Submit");
            }

        }

    }
}