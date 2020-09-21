using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    public static class ComponentModelExtensions
    {
        public static AngularComponentSettings GetAngularComponentSettings(this ComponentModel model)
        {
            var stereotype = model.GetStereotype("Angular Component Settings");
            return stereotype != null ? new AngularComponentSettings(stereotype) : null;
        }

        public static bool HasAngularComponentSettings(this ComponentModel model)
        {
            return model.HasStereotype("Angular Component Settings");
        }


        public class AngularComponentSettings
        {
            private IStereotype _stereotype;

            public AngularComponentSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement[] InjectServices()
            {
                return _stereotype.GetProperty<IElement[]>("Inject Services");
            }

        }

    }
}