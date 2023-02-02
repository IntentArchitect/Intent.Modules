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
    public static class WaitModelStereotypeExtensions
    {
        public static WaitSettings GetWaitSettings(this WaitModel model)
        {
            var stereotype = model.GetStereotype("Wait Settings");
            return stereotype != null ? new WaitSettings(stereotype) : null;
        }


        public static bool HasWaitSettings(this WaitModel model)
        {
            return model.HasStereotype("Wait Settings");
        }

        public static bool TryGetWaitSettings(this WaitModel model, out WaitSettings stereotype)
        {
            if (!HasWaitSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new WaitSettings(model.GetStereotype("Wait Settings"));
            return true;
        }

        public class WaitSettings
        {
            private IStereotype _stereotype;

            public WaitSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public DurationTypeOptions DurationType()
            {
                return new DurationTypeOptions(_stereotype.GetProperty<string>("Duration Type"));
            }

            public int? DurationAmount()
            {
                return _stereotype.GetProperty<int?>("Duration Amount");
            }

            public class DurationTypeOptions
            {
                public readonly string Value;

                public DurationTypeOptions(string value)
                {
                    Value = value;
                }

                public DurationTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "millis":
                            return DurationTypeOptionsEnum.Millis;
                        case "seconds":
                            return DurationTypeOptionsEnum.Seconds;
                        case "minutes":
                            return DurationTypeOptionsEnum.Minutes;
                        case "hours":
                            return DurationTypeOptionsEnum.Hours;
                        case "days":
                            return DurationTypeOptionsEnum.Days;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsMillis()
                {
                    return Value == "millis";
                }
                public bool IsSeconds()
                {
                    return Value == "seconds";
                }
                public bool IsMinutes()
                {
                    return Value == "minutes";
                }
                public bool IsHours()
                {
                    return Value == "hours";
                }
                public bool IsDays()
                {
                    return Value == "days";
                }
            }

            public enum DurationTypeOptionsEnum
            {
                Millis,
                Seconds,
                Minutes,
                Hours,
                Days
            }
        }

    }
}