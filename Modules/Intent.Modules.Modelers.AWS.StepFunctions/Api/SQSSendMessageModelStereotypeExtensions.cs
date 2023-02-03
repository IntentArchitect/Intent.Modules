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
    public static class SQSSendMessageModelStereotypeExtensions
    {
        public static SQSSendMessageSettings GetSQSSendMessageSettings(this SQSSendMessageModel model)
        {
            var stereotype = model.GetStereotype("SQS Send Message Settings");
            return stereotype != null ? new SQSSendMessageSettings(stereotype) : null;
        }


        public static bool HasSQSSendMessageSettings(this SQSSendMessageModel model)
        {
            return model.HasStereotype("SQS Send Message Settings");
        }

        public static bool TryGetSQSSendMessageSettings(this SQSSendMessageModel model, out SQSSendMessageSettings stereotype)
        {
            if (!HasSQSSendMessageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SQSSendMessageSettings(model.GetStereotype("SQS Send Message Settings"));
            return true;
        }

        public class SQSSendMessageSettings
        {
            private IStereotype _stereotype;

            public SQSSendMessageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public MessageOptions Message()
            {
                return new MessageOptions(_stereotype.GetProperty<string>("Message"));
            }

            public string MessageContent()
            {
                return _stereotype.GetProperty<string>("Message Content");
            }

            public class MessageOptions
            {
                public readonly string Value;

                public MessageOptions(string value)
                {
                    Value = value;
                }

                public MessageOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Use state input as message":
                            return MessageOptionsEnum.UseStateInputAsMessage;
                        case "Enter message":
                            return MessageOptionsEnum.EnterMessage;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsUseStateInputAsMessage()
                {
                    return Value == "Use state input as message";
                }
                public bool IsEnterMessage()
                {
                    return Value == "Enter message";
                }
            }

            public enum MessageOptionsEnum
            {
                UseStateInputAsMessage,
                EnterMessage
            }
        }

    }
}