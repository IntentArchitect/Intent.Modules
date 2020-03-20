using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class CSharpTemplateExtensions
    {
        public static ExposesDecoratorContract GetExposesDecoratorContract(this ICSharpTemplate model)
        {
            var stereotype = model.GetStereotype("Exposes Decorator Contract");
            return stereotype != null ? new ExposesDecoratorContract(stereotype) : null;
        }


        public class ExposesDecoratorContract
        {
            private IStereotype _stereotype;

            public ExposesDecoratorContract(IStereotype stereotype)
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