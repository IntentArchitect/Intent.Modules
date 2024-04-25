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
        public static ComponentSettings GetComponentSettings(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(ComponentSettings.DefinitionId);
            return stereotype != null ? new ComponentSettings(stereotype) : null;
        }


        public static bool HasComponentSettings(this ComponentModel model)
        {
            return model.HasStereotype(ComponentSettings.DefinitionId);
        }

        public static bool TryGetComponentSettings(this ComponentModel model, out ComponentSettings stereotype)
        {
            if (!HasComponentSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ComponentSettings(model.GetStereotype(ComponentSettings.DefinitionId));
            return true;
        }

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
        public static SupportForChildren GetSupportForChildren(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(SupportForChildren.DefinitionId);
            return stereotype != null ? new SupportForChildren(stereotype) : null;
        }


        public static bool HasSupportForChildren(this ComponentModel model)
        {
            return model.HasStereotype(SupportForChildren.DefinitionId);
        }

        public static bool TryGetSupportForChildren(this ComponentModel model, out SupportForChildren stereotype)
        {
            if (!HasSupportForChildren(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SupportForChildren(model.GetStereotype(SupportForChildren.DefinitionId));
            return true;
        }

        public class ComponentSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "93f869a6-f3e1-498e-bf40-6cfe99f012c5";

            public ComponentSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

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

        public class SupportForChildren
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a41c8945-0ca4-4597-b760-66473153b6ab";

            public SupportForChildren(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}