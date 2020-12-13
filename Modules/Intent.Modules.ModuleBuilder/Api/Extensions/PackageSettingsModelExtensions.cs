using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
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

            public SortingOptions Sorting()
            {
                return new SortingOptions(_stereotype.GetProperty<string>("Sorting"));
            }

            public class SortingOptions
            {
                public readonly string Value;

                public SortingOptions(string value)
                {
                    Value = value;
                }

                public bool IsManually()
                {
                    return Value == "Manually";
                }
                public bool IsByTypeThenManually()
                {
                    return Value == "By type, then manually";
                }
                public bool IsByTypeThenByName()
                {
                    return Value == "By type, then by name";
                }
                public bool IsByName()
                {
                    return Value == "By name";
                }
            }

        }

    }
}