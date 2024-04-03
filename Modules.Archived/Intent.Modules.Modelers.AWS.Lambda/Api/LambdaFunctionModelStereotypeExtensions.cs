using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Lambda.Api
{
    public static class LambdaFunctionModelStereotypeExtensions
    {
        public static LambdaSettings GetLambdaSettings(this LambdaFunctionModel model)
        {
            var stereotype = model.GetStereotype("Lambda Settings");
            return stereotype != null ? new LambdaSettings(stereotype) : null;
        }


        public static bool HasLambdaSettings(this LambdaFunctionModel model)
        {
            return model.HasStereotype("Lambda Settings");
        }

        public static bool TryGetLambdaSettings(this LambdaFunctionModel model, out LambdaSettings stereotype)
        {
            if (!HasLambdaSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LambdaSettings(model.GetStereotype("Lambda Settings"));
            return true;
        }

        public class LambdaSettings
        {
            private IStereotype _stereotype;

            public LambdaSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement Runtime()
            {
                return _stereotype.GetProperty<IElement>("Runtime");
            }

        }

    }
}