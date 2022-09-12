using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class StaticContentTemplateModelStereotypeExtensions
    {
        public static TemplateSettings GetTemplateSettings(this StaticContentTemplateModel model)
        {
            var stereotype = model.GetStereotype("Template Settings");
            return stereotype != null ? new TemplateSettings(stereotype) : null;
        }

        public static bool HasTemplateSettings(this StaticContentTemplateModel model)
        {
            return model.HasStereotype("Template Settings");
        }

        public static bool TryGetTemplateSettings(this StaticContentTemplateModel model, out TemplateSettings stereotype)
        {
            if (!HasTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TemplateSettings(model.GetStereotype("Template Settings"));
            return true;
        }


        public class TemplateSettings
        {
            private IStereotype _stereotype;

            public TemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ContentSubfolder()
            {
                return _stereotype.GetProperty<string>("Content Subfolder");
            }

            public string Role()
            {
                return _stereotype.GetProperty<string>("Role");
            }

            public string DefaultLocation()
            {
                return _stereotype.GetProperty<string>("Default Location");
            }

        }

    }
}