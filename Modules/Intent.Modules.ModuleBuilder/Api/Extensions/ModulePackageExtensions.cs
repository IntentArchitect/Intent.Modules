using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ModulePackageExtensions
    {
        public static ModulePackageSettings GetModulePackageSettings(this IModulePackage model)
        {
            var stereotype = model.GetStereotype("Module Package Settings");
            return stereotype != null ? new ModulePackageSettings(stereotype) : null;
        }


        public class ModulePackageSettings
        {
            private IStereotype _stereotype;

            public ModulePackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public IElement[] TargetModelers()
            {
                return _stereotype.GetProperty<IElement[]>("Target Modelers");
            }

        }

    }
}