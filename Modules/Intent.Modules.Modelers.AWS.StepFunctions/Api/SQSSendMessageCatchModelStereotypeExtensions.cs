using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    public static class SQSSendMessageCatchModelStereotypeExtensions
    {
        public static SQSSendMessageCatchSettings GetSQSSendMessageCatchSettings(this SQSSendMessageCatchModel model)
        {
            var stereotype = model.GetStereotype("SQS Send Message Catch Settings");
            return stereotype != null ? new SQSSendMessageCatchSettings(stereotype) : null;
        }


        public static bool HasSQSSendMessageCatchSettings(this SQSSendMessageCatchModel model)
        {
            return model.HasStereotype("SQS Send Message Catch Settings");
        }

        public static bool TryGetSQSSendMessageCatchSettings(this SQSSendMessageCatchModel model, out SQSSendMessageCatchSettings stereotype)
        {
            if (!HasSQSSendMessageCatchSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SQSSendMessageCatchSettings(model.GetStereotype("SQS Send Message Catch Settings"));
            return true;
        }

        public class SQSSendMessageCatchSettings
        {
            private IStereotype _stereotype;

            public SQSSendMessageCatchSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ResultPath()
            {
                return _stereotype.GetProperty<string>("Result Path");
            }

        }

    }
}