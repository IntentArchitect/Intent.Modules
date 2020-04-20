using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class DesignerExtensionModelExtensions
    {
        public static DesignerSettings GetDesignerSettings(this DesignerExtensionModel model)
        {
            var stereotype = model.GetStereotype("Designer Settings");
            return stereotype != null ? new DesignerSettings(stereotype) : null;
        }

        public static bool HasDesignerSettings(this DesignerExtensionModel model)
        {
            return model.HasStereotype("Designer Settings");
        }


        public class DesignerSettings
        {
            private IStereotype _stereotype;

            public DesignerSettings(IStereotype stereotype)
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