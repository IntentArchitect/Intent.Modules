using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class DesignerModelExtensions
    {
        public static ModelerSettings GetModelerSettings(this DesignerModel model)
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

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string APINamespace()
            {
                return _stereotype.GetProperty<string>("API Namespace");
            }

            public bool IsReference()
            {
                return _stereotype.GetProperty<bool>("Is Reference");
            }

        }

    }
}