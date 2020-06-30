using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class DesignerSettingsModelExtensions
    {
        public static DesignerSettings GetDesignerSettings(this DesignerSettingsModel model)
        {
            var stereotype = model.GetStereotype("Designer Settings");
            return stereotype != null ? new DesignerSettings(stereotype) : null;
        }

        public static bool HasDesignerSettings(this DesignerSettingsModel model)
        {
            return model.HasStereotype("Designer Settings");
        }


        public class DesignerSettings
        {
            private IStereotype _stereotype;

            public DesignerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsReference()
            {
                return _stereotype.GetProperty<bool>("Is Reference");
            }

            public string APINamespace()
            {
                return _stereotype.GetProperty<string>("API Namespace");
            }

            public string NuGetPackageId()
            {
                return _stereotype.GetProperty<string>("NuGet Package Id");
            }

            public string NuGetPackageVersion()
            {
                return _stereotype.GetProperty<string>("NuGet Package Version");
            }

        }

    }
}