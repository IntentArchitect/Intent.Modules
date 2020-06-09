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

        }

    }
}