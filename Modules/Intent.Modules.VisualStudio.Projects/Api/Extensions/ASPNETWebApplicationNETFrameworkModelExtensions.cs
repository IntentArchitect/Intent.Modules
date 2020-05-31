using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class ASPNETWebApplicationNETFrameworkModelExtensions
    {
        public static NETFrameworkSettings GetNETFrameworkSettings(this ASPNETWebApplicationNETFrameworkModel model)
        {
            var stereotype = model.GetStereotype(".NET Framework Settings");
            return stereotype != null ? new NETFrameworkSettings(stereotype) : null;
        }

        public static bool HasNETFrameworkSettings(this ASPNETWebApplicationNETFrameworkModel model)
        {
            return model.HasStereotype(".NET Framework Settings");
        }


        public class NETFrameworkSettings
        {
            private IStereotype _stereotype;

            public NETFrameworkSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement TargetFramework()
            {
                return _stereotype.GetProperty<IElement>("Target Framework");
            }

            public class TargetFrameworkOptions
            {
                public readonly string Value;

                public TargetFrameworkOptions(string value)
                {
                    Value = value;
                }

                public bool IsNet452()
                {
                    return Value == "net452";
                }
                public bool IsNet462()
                {
                    return Value == "net462";
                }
                public bool IsNet472()
                {
                    return Value == "net472";
                }
            }

        }

    }
}