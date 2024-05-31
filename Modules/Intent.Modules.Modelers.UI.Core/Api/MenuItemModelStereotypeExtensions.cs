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
    public static class MenuItemModelStereotypeExtensions
    {
        public static NavigationLink GetNavigationLink(this MenuItemModel model)
        {
            var stereotype = model.GetStereotype(NavigationLink.DefinitionId);
            return stereotype != null ? new NavigationLink(stereotype) : null;
        }


        public static bool HasNavigationLink(this MenuItemModel model)
        {
            return model.HasStereotype(NavigationLink.DefinitionId);
        }

        public static bool TryGetNavigationLink(this MenuItemModel model, out NavigationLink stereotype)
        {
            if (!HasNavigationLink(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NavigationLink(model.GetStereotype(NavigationLink.DefinitionId));
            return true;
        }

        public class NavigationLink
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "cec9d1b0-7803-4b8b-881d-6ed312fa4d3d";

            public NavigationLink(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement NavigateTo()
            {
                return _stereotype.GetProperty<IElement>("Navigate To");
            }

        }

    }
}