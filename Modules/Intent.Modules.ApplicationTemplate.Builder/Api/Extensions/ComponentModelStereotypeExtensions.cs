using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ComponentModelStereotypeExtensions
    {
        public static ComponentSettings GetComponentSettings(this ComponentModel model)
        {
            var stereotype = model.GetStereotype("Component Settings");
            return stereotype != null ? new ComponentSettings(stereotype) : null;
        }

        public static bool HasComponentSettings(this ComponentModel model)
        {
            return model.HasStereotype("Component Settings");
        }


        public class ComponentSettings
        {
            private IStereotype _stereotype;

            public ComponentSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string Description()
            {
                return _stereotype.GetProperty<string>("Description");
            }

            public bool IncludeByDefault()
            {
                return _stereotype.GetProperty<bool>("Include by Default");
            }

            public bool IsRequired()
            {
                return _stereotype.GetProperty<bool>("Is Required");
            }

            public IElement[] Dependencies()
            {
                return _stereotype.GetProperty<IElement[]>("Dependencies") ?? new IElement[0];
            }

        }

    }
}