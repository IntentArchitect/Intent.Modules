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
    public static class ComponentModuleModelStereotypeExtensions
    {
        public static ModuleSettings GetModuleSettings(this ComponentModuleModel model)
        {
            var stereotype = model.GetStereotype(ModuleSettings.DefinitionId);
            return stereotype != null ? new ModuleSettings(stereotype) : null;
        }

        public static bool HasModuleSettings(this ComponentModuleModel model)
        {
            return model.HasStereotype(ModuleSettings.DefinitionId);
        }

        public static bool TryGetModuleSettings(this ComponentModuleModel model, out ModuleSettings stereotype)
        {
            if (!HasModuleSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ModuleSettings(model.GetStereotype(ModuleSettings.DefinitionId));
            return true;
        }


        public class ModuleSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7033e6f4-2a22-4357-bd65-f0ec06c516d5";

            public ModuleSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Version()
            {
                return _stereotype.GetProperty<string>("Version");
            }

            public bool IncludeByDefault()
            {
                return _stereotype.GetProperty<bool>("Include By Default");
            }

            public bool IsRequired()
            {
                return _stereotype.GetProperty<bool>("Is Required");
            }

            public bool InstallMetadataOnly()
            {
                return _stereotype.GetProperty<bool>("Install Metadata Only");
            }

        }

    }
}