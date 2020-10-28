using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class PackageSettingsModelExtensions
    {
        public static PackageSettings GetPackageSettings(this PackageSettingsModel model)
        {
            var stereotype = model.GetStereotype("Package Settings");
            return stereotype != null ? new PackageSettings(stereotype) : null;
        }

        public static bool HasPackageSettings(this PackageSettingsModel model)
        {
            return model.HasStereotype("Package Settings");
        }


        public class PackageSettings
        {
            private IStereotype _stereotype;

            public PackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string DefaultName()
            {
                return _stereotype.GetProperty<string>("Default Name");
            }

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

        }

    }
}