using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.AWS.Api
{
    public static class AWSLambdaFunctionModelStereotypeExtensions
    {
        public static LambdaSettings GetLambdaSettings(this AWSLambdaFunctionModel model)
        {
            var stereotype = model.GetStereotype("Lambda Settings");
            return stereotype != null ? new LambdaSettings(stereotype) : null;
        }


        public static bool HasLambdaSettings(this AWSLambdaFunctionModel model)
        {
            return model.HasStereotype("Lambda Settings");
        }

        public static bool TryGetLambdaSettings(this AWSLambdaFunctionModel model, out LambdaSettings stereotype)
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

        }

    }
}