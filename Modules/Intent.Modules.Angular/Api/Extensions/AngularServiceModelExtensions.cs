using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    public static class AngularServiceModelExtensions
    {
        public static AngularServiceSettings GetAngularServiceSettings(this AngularServiceModel model)
        {
            var stereotype = model.GetStereotype("Angular Service Settings");
            return stereotype != null ? new AngularServiceSettings(stereotype) : null;
        }

        public static bool HasAngularServiceSettings(this AngularServiceModel model)
        {
            return model.HasStereotype("Angular Service Settings");
        }


        public class AngularServiceSettings
        {
            private IStereotype _stereotype;

            public AngularServiceSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Location()
            {
                return _stereotype.GetProperty<string>("Location");
            }

        }

    }
}