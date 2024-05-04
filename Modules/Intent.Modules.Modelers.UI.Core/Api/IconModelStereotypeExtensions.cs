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
    public static class IconModelStereotypeExtensions
    {
        public static IconAppearance GetIconAppearance(this IconModel model)
        {
            var stereotype = model.GetStereotype(IconAppearance.DefinitionId);
            return stereotype != null ? new IconAppearance(stereotype) : null;
        }


        public static bool HasIconAppearance(this IconModel model)
        {
            return model.HasStereotype(IconAppearance.DefinitionId);
        }

        public static bool TryGetIconAppearance(this IconModel model, out IconAppearance stereotype)
        {
            if (!HasIconAppearance(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new IconAppearance(model.GetStereotype(IconAppearance.DefinitionId));
            return true;
        }

        public class IconAppearance
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b71f0a4d-82b4-4685-b58f-a31ea77c45dd";

            public IconAppearance(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public IIconModel Name()
            {
                return _stereotype.GetProperty<IIconModel>("Name");
            }

        }

    }
}