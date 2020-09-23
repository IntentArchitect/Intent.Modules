using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class IntentDesignerPackageModelExtensions
    {
        public static ModuleSettings GetModuleSettings(this IntentDesignerPackageModel model)
        {
            var stereotype = model.GetStereotype("Module Settings");
            return stereotype != null ? new ModuleSettings(stereotype) : null;
        }

        public static bool HasModuleSettings(this IntentDesignerPackageModel model)
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

            public bool IncludeInModule()
            {
                return _stereotype.GetProperty<bool>("Include in Module");
            }

            public IElement[] ReferenceInDesigner()
            {
                return _stereotype.GetProperty<IElement[]>("Reference in Designer");
            }

        }

    }
}