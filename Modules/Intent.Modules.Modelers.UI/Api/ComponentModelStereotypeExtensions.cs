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
    public static class ComponentModelStereotypeExtensions
    {

        public static Page GetPage(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Page.DefinitionId);
            return stereotype != null ? new Page(stereotype) : null;
        }


        public static bool HasPage(this ComponentModel model)
        {
            return model.HasStereotype(Page.DefinitionId);
        }

        public static bool TryGetPage(this ComponentModel model, out Page stereotype)
        {
            if (!HasPage(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Page(model.GetStereotype(Page.DefinitionId));
            return true;
        }

        public class Page
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "ea4adc09-8978-4ede-ba5f-265debb2b60c";

            public Page(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

            public string Title()
            {
                return _stereotype.GetProperty<string>("Title");
            }

        }

    }
}