using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.OutputTargets.Folders.Api
{
    public static class TemplateOutputModelStereotypeExtensions
    {
        public static TemplateOutputSettings GetTemplateOutputSettings(this TemplateOutputModel model)
        {
            var stereotype = model.GetStereotype(TemplateOutputSettings.DefinitionId);
            return stereotype != null ? new TemplateOutputSettings(stereotype) : null;
        }


        public static bool HasTemplateOutputSettings(this TemplateOutputModel model)
        {
            return model.HasStereotype(TemplateOutputSettings.DefinitionId);
        }

        public static bool TryGetTemplateOutputSettings(this TemplateOutputModel model, out TemplateOutputSettings stereotype)
        {
            if (!HasTemplateOutputSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TemplateOutputSettings(model.GetStereotype(TemplateOutputSettings.DefinitionId));
            return true;
        }

        public class TemplateOutputSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "ad834ed1-5737-4637-aa3d-889216569468";

            public TemplateOutputSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string RegistrationFilter()
            {
                return _stereotype.GetProperty<string>("Registration Filter");
            }

        }

    }
}