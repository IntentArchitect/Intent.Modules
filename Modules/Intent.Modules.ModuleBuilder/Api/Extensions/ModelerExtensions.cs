using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ModelerExtensions
    {
        public static ModelerSettings GetModelerSettings(this IModeler model)
        {
            var stereotype = model.GetStereotype("Modeler Settings");
            return stereotype != null ? new ModelerSettings(stereotype) : null;
        }


        public class ModelerSettings
        {
            private IStereotype _stereotype;

            public ModelerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string APINamespace()
            {
                return _stereotype.GetProperty<string>("API Namespace");
            }

        }

    }
}