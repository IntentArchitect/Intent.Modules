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
    public static class SQSSendMessageRetryModelStereotypeExtensions
    {
        public static RetrySettings GetRetrySettings(this SQSSendMessageRetryModel model)
        {
            var stereotype = model.GetStereotype("Retry Settings");
            return stereotype != null ? new RetrySettings(stereotype) : null;
        }


        public static bool HasRetrySettings(this SQSSendMessageRetryModel model)
        {
            return model.HasStereotype("Retry Settings");
        }

        public static bool TryGetRetrySettings(this SQSSendMessageRetryModel model, out RetrySettings stereotype)
        {
            if (!HasRetrySettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RetrySettings(model.GetStereotype("Retry Settings"));
            return true;
        }

        public class RetrySettings
        {
            private IStereotype _stereotype;

            public RetrySettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? MaxAttempts()
            {
                return _stereotype.GetProperty<int?>("Max Attempts");
            }

            public int? BackoffRate()
            {
                return _stereotype.GetProperty<int?>("Backoff Rate");
            }

            public IntervalTypeOptions IntervalType()
            {
                return new IntervalTypeOptions(_stereotype.GetProperty<string>("Interval Type"));
            }

            public int? IntervalAmount()
            {
                return _stereotype.GetProperty<int?>("Interval Amount");
            }

            public class IntervalTypeOptions
            {
                public readonly string Value;

                public IntervalTypeOptions(string value)
                {
                    Value = value;
                }

                public IntervalTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "millis":
                            return IntervalTypeOptionsEnum.Millis;
                        case "seconds":
                            return IntervalTypeOptionsEnum.Seconds;
                        case "minutes":
                            return IntervalTypeOptionsEnum.Minutes;
                        case "hours":
                            return IntervalTypeOptionsEnum.Hours;
                        case "days":
                            return IntervalTypeOptionsEnum.Days;
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

            public enum IntervalTypeOptionsEnum
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