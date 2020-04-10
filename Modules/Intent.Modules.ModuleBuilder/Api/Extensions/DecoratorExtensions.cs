using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class DecoratorExtensions
    {
        public static DecoratorSettings GetDecoratorSettings(this Decorator model)
        {
            var stereotype = model.GetStereotype("Decorator Settings");
            return stereotype != null ? new DecoratorSettings(stereotype) : null;
        }

        public static ImplementsDecoratorContract GetImplementsDecoratorContract(this Decorator model)
        {
            var stereotype = model.GetStereotype("Implements Decorator Contract");
            return stereotype != null ? new ImplementsDecoratorContract(stereotype) : null;
        }


        public class DecoratorSettings
        {
            private IStereotype _stereotype;

            public DecoratorSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public bool DeclareUsings()
            {
                return _stereotype.GetProperty<bool>("Declare Usings");
            }

        }

        public class ImplementsDecoratorContract
        {
            private IStereotype _stereotype;

            public ImplementsDecoratorContract(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string TypeFullname()
            {
                return _stereotype.GetProperty<string>("Type Fullname");
            }

        }

    }
}