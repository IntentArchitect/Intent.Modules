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
    public static class ConditionalStatementModelStereotypeExtensions
    {
        public static ConditionalStatementSettings GetConditionalStatementSettings(this ConditionalStatementModel model)
        {
            var stereotype = model.GetStereotype("Conditional Statement Settings");
            return stereotype != null ? new ConditionalStatementSettings(stereotype) : null;
        }


        public static bool HasConditionalStatementSettings(this ConditionalStatementModel model)
        {
            return model.HasStereotype("Conditional Statement Settings");
        }

        public static bool TryGetConditionalStatementSettings(this ConditionalStatementModel model, out ConditionalStatementSettings stereotype)
        {
            if (!HasConditionalStatementSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ConditionalStatementSettings(model.GetStereotype("Conditional Statement Settings"));
            return true;
        }

        public class ConditionalStatementSettings
        {
            private IStereotype _stereotype;

            public ConditionalStatementSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool NOT()
            {
                return _stereotype.GetProperty<bool>("NOT");
            }

            public string Variable()
            {
                return _stereotype.GetProperty<string>("Variable");
            }

            public OperatorOptions Operator()
            {
                return new OperatorOptions(_stereotype.GetProperty<string>("Operator"));
            }

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public ValueTypeOptions ValueType()
            {
                return new ValueTypeOptions(_stereotype.GetProperty<string>("Value Type"));
            }

            public string Value()
            {
                return _stereotype.GetProperty<string>("Value");
            }

            public string BooleanValue()
            {
                return _stereotype.GetProperty<string>("Boolean Value");
            }

            public class OperatorOptions
            {
                public readonly string Value;

                public OperatorOptions(string value)
                {
                    Value = value;
                }

                public OperatorOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "is present":
                            return OperatorOptionsEnum.IsPresent;
                        case "is of type":
                            return OperatorOptionsEnum.IsOfType;
                        case "is equal to":
                            return OperatorOptionsEnum.IsEqualTo;
                        case "is greater than":
                            return OperatorOptionsEnum.IsGreaterThan;
                        case "is greater than or equal to":
                            return OperatorOptionsEnum.IsGreaterThanOrEqualTo;
                        case "is less than or equal to":
                            return OperatorOptionsEnum.IsLessThanOrEqualTo;
                        case "matches string":
                            return OperatorOptionsEnum.MatchesString;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsIsPresent()
                {
                    return Value == "is present";
                }
                public bool IsIsOfType()
                {
                    return Value == "is of type";
                }
                public bool IsIsEqualTo()
                {
                    return Value == "is equal to";
                }
                public bool IsIsGreaterThan()
                {
                    return Value == "is greater than";
                }
                public bool IsIsGreaterThanOrEqualTo()
                {
                    return Value == "is greater than or equal to";
                }
                public bool IsIsLessThanOrEqualTo()
                {
                    return Value == "is less than or equal to";
                }
                public bool IsMatchesString()
                {
                    return Value == "matches string";
                }
            }

            public enum OperatorOptionsEnum
            {
                IsPresent,
                IsOfType,
                IsEqualTo,
                IsGreaterThan,
                IsGreaterThanOrEqualTo,
                IsLessThanOrEqualTo,
                MatchesString
            }
            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public TypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Number":
                            return TypeOptionsEnum.Number;
                        case "Timestamp":
                            return TypeOptionsEnum.Timestamp;
                        case "Boolean":
                            return TypeOptionsEnum.Boolean;
                        case "String":
                            return TypeOptionsEnum.String;
                        case "Null":
                            return TypeOptionsEnum.Null;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNumber()
                {
                    return Value == "Number";
                }
                public bool IsTimestamp()
                {
                    return Value == "Timestamp";
                }
                public bool IsBoolean()
                {
                    return Value == "Boolean";
                }
                public bool IsString()
                {
                    return Value == "String";
                }
                public bool IsNull()
                {
                    return Value == "Null";
                }
            }

            public enum TypeOptionsEnum
            {
                Number,
                Timestamp,
                Boolean,
                String,
                Null
            }
            public class ValueTypeOptions
            {
                public readonly string Value;

                public ValueTypeOptions(string value)
                {
                    Value = value;
                }

                public ValueTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Number constant":
                            return ValueTypeOptionsEnum.NumberConstant;
                        case "Number variable":
                            return ValueTypeOptionsEnum.NumberVariable;
                        case "String constant":
                            return ValueTypeOptionsEnum.StringConstant;
                        case "String variable":
                            return ValueTypeOptionsEnum.StringVariable;
                        case "Timestamp constant":
                            return ValueTypeOptionsEnum.TimestampConstant;
                        case "Timestamp variable":
                            return ValueTypeOptionsEnum.TimestampVariable;
                        case "Boolean constant":
                            return ValueTypeOptionsEnum.BooleanConstant;
                        case "Boolean variable":
                            return ValueTypeOptionsEnum.BooleanVariable;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNumberConstant()
                {
                    return Value == "Number constant";
                }
                public bool IsNumberVariable()
                {
                    return Value == "Number variable";
                }
                public bool IsStringConstant()
                {
                    return Value == "String constant";
                }
                public bool IsStringVariable()
                {
                    return Value == "String variable";
                }
                public bool IsTimestampConstant()
                {
                    return Value == "Timestamp constant";
                }
                public bool IsTimestampVariable()
                {
                    return Value == "Timestamp variable";
                }
                public bool IsBooleanConstant()
                {
                    return Value == "Boolean constant";
                }
                public bool IsBooleanVariable()
                {
                    return Value == "Boolean variable";
                }
            }

            public enum ValueTypeOptionsEnum
            {
                NumberConstant,
                NumberVariable,
                StringConstant,
                StringVariable,
                TimestampConstant,
                TimestampVariable,
                BooleanConstant,
                BooleanVariable
            }
        }

    }
}