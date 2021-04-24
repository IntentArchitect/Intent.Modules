using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ModuleModelExtensions
    {
        public static ModuleSettings GetModuleSettings(this ModuleModel model)
        {
            var stereotype = model.GetStereotype("Module Settings");
            return stereotype != null ? new ModuleSettings(stereotype) : null;
        }

        public static bool HasModuleSettings(this ModuleModel model)
        {
            return model.HasStereotype("Module Settings");
        }


        public class ModuleSettings
        {
            private IStereotype _stereotype;

            public ModuleSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Version()
            {
                return _stereotype.GetProperty<string>("Version");
            }

        }

    }
}