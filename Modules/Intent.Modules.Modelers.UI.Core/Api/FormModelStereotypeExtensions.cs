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
            var stereotype = model.GetStereotype("7f7bc148-138f-4b61-8fa1-a8d725dbed52");
            return stereotype != null ? new Content(stereotype) : null;
        }


        public static bool HasContent(this FormModel model)
        {
            return model.HasStereotype("7f7bc148-138f-4b61-8fa1-a8d725dbed52");
        }

        public static bool TryGetContent(this FormModel model, out Content stereotype)
        {
            if (!HasContent(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Content(model.GetStereotype("7f7bc148-138f-4b61-8fa1-a8d725dbed52"));
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