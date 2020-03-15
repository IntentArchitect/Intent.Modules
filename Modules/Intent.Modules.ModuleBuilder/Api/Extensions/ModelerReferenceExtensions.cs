using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ModelerReferenceExtensions
    {
        public static ModelerReferenceSettings GetModelerReferenceSettings(this IModelerReference model)
        {
            var stereotype = model.GetStereotype("Modeler Reference Settings");
            return stereotype != null ? new ModelerReferenceSettings(stereotype) : null;
        }


        public class ModelerReferenceSettings
        {
            private IStereotype _stereotype;

            public ModelerReferenceSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string ModuleDependency()
            {
                return _stereotype.GetProperty<string>("Module Dependency");
            }

            public string ModuleVersion()
            {
                return _stereotype.GetProperty<string>("Module Version");
            }

            public string NuGetDependency()
            {
                return _stereotype.GetProperty<string>("NuGet Dependency");
            }

            public string NuGetVersion()
            {
                return _stereotype.GetProperty<string>("NuGet Version");
            }

        }

    }
}