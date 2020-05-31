using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class ASPNETCoreWebApplicationModelExtensions
    {
        public static NETCoreSettings GetNETCoreSettings(this ASPNETCoreWebApplicationModel model)
        {
            var stereotype = model.GetStereotype(".NET Core Settings");
            return stereotype != null ? new NETCoreSettings(stereotype) : null;
        }

        public static bool HasNETCoreSettings(this ASPNETCoreWebApplicationModel model)
        {
            return model.HasStereotype(".NET Core Settings");
        }


        public class NETCoreSettings
        {
            private IStereotype _stereotype;

            public NETCoreSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool TargetMultipleFrameworks()
            {
                return _stereotype.GetProperty<bool>("Target Multiple Frameworks");
            }

            public IElement TargetFramework()
            {
                return _stereotype.GetProperty<IElement>("Target Framework");
            }

            public IElement[] TargetFrameworks()
            {
                return _stereotype.GetProperty<IElement[]>("Target Frameworks");
            }

            public class TargetFrameworkOptions
            {
                public readonly string Value;

                public TargetFrameworkOptions(string value)
                {
                    Value = value;
                }

                public bool IsNetcoreapp21()
                {
                    return Value == "netcoreapp2.1";
                }
                public bool IsNetcoreapp22()
                {
                    return Value == "netcoreapp2.2";
                }
                public bool IsNetcoreapp30()
                {
                    return Value == "netcoreapp3.0";
                }
                public bool IsNetcoreapp31()
                {
                    return Value == "netcoreapp3.1";
                }
                public bool IsNetcoreapp32()
                {
                    return Value == "netcoreapp3.2";
                }
            }

            public class TargetFrameworksOptions
            {
                public readonly string Value;

                public TargetFrameworksOptions(string value)
                {
                    Value = value;
                }

                public bool IsNetcoreapp21()
                {
                    return Value == "netcoreapp2.1";
                }
                public bool IsNetcoreapp22()
                {
                    return Value == "netcoreapp2.2";
                }
                public bool IsNetcoreapp30()
                {
                    return Value == "netcoreapp3.0";
                }
                public bool IsNetcoreapp31()
                {
                    return Value == "netcoreapp3.1";
                }
                public bool IsNetcoreapp32()
                {
                    return Value == "netcoreapp3.2";
                }
            }

        }

    }
}