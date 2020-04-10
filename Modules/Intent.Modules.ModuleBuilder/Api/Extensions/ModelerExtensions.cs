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
        public static ModelerSettings GetModelerSettings(this Modeler model)
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

            public ModelerTypeOptions ModelerType()
            {
                return new ModelerTypeOptions(_stereotype.GetProperty<string>("Modeler Type"));
            }

            public string APINamespace()
            {
                return _stereotype.GetProperty<string>("API Namespace");
            }

            public class ModelerTypeOptions
            {
                public readonly string Value;

                public ModelerTypeOptions(string value)
                {
                    Value = value;
                }

                public bool IsStandard()
                {
                    return Value == "Standard";
                }
                public bool IsExtension()
                {
                    return Value == "Extension";
                }
                public bool IsReference()
                {
                    return Value == "Reference";
                }
            }

        }

    }
}