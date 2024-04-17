using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    public static class LayoutSiderModelStereotypeExtensions
    {
        public static Content GetContent(this LayoutSiderModel model)
        {
            var stereotype = model.GetStereotype(Content.DefinitionId);
            return stereotype != null ? new Content(stereotype) : null;
        }


        public static bool HasContent(this LayoutSiderModel model)
        {
            return model.HasStereotype(Content.DefinitionId);
        }

        public static bool TryGetContent(this LayoutSiderModel model, out Content stereotype)
        {
            if (!HasContent(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Content(model.GetStereotype(Content.DefinitionId));
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

    }
}