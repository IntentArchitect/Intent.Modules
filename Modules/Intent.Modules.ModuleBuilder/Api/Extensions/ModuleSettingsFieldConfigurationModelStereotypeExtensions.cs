using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ModuleSettingsFieldConfigurationModelStereotypeExtensions
    {
        public static FieldConfiguration GetFieldConfiguration(this ModuleSettingsFieldConfigurationModel model)
        {
            var stereotype = model.GetStereotype(FieldConfiguration.DefinitionId);
            return stereotype != null ? new FieldConfiguration(stereotype) : null;
        }

        public static bool HasFieldConfiguration(this ModuleSettingsFieldConfigurationModel model)
        {
            return model.HasStereotype(FieldConfiguration.DefinitionId);
        }

        public static bool TryGetFieldConfiguration(this ModuleSettingsFieldConfigurationModel model, out FieldConfiguration stereotype)
        {
            if (!HasFieldConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FieldConfiguration(model.GetStereotype(FieldConfiguration.DefinitionId));
            return true;
        }


        public class FieldConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "4805c791-5be3-4049-b929-6046d7be9944";

            public FieldConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ControlTypeOptions ControlType()
            {
                return new ControlTypeOptions(_stereotype.GetProperty<string>("Control Type"));
            }

            public bool IsRequired()
            {
                return _stereotype.GetProperty<bool>("Is Required");
            }

            public string Hint()
            {
                return _stereotype.GetProperty<string>("Hint");
            }

            public string DefaultValue()
            {
                return _stereotype.GetProperty<string>("Default Value");
            }

            public string IsActiveFunction()
            {
                return _stereotype.GetProperty<string>("Is Active Function");
            }

            public string IsRequiredFunction()
            {
                return _stereotype.GetProperty<string>("Is Required Function");
            }

            public class ControlTypeOptions
            {
                public readonly string Value;

                public ControlTypeOptions(string value)
                {
                    Value = value;
                }

                public ControlTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Text Box":
                            return ControlTypeOptionsEnum.TextBox;
                        case "Number":
                            return ControlTypeOptionsEnum.Number;
                        case "Checkbox":
                            return ControlTypeOptionsEnum.Checkbox;
                        case "Switch":
                            return ControlTypeOptionsEnum.Switch;
                        case "Text Area":
                            return ControlTypeOptionsEnum.TextArea;
                        case "Select":
                            return ControlTypeOptionsEnum.Select;
                        case "Multi-Select":
                            return ControlTypeOptionsEnum.MultiSelect;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsTextBox()
                {
                    return Value == "Text Box";
                }
                public bool IsNumber()
                {
                    return Value == "Number";
                }
                public bool IsCheckbox()
                {
                    return Value == "Checkbox";
                }
                public bool IsSwitch()
                {
                    return Value == "Switch";
                }
                public bool IsTextArea()
                {
                    return Value == "Text Area";
                }
                public bool IsSelect()
                {
                    return Value == "Select";
                }
                public bool IsMultiSelect()
                {
                    return Value == "Multi-Select";
                }
            }

            public enum ControlTypeOptionsEnum
            {
                TextBox,
                Number,
                Checkbox,
                Switch,
                TextArea,
                Select,
                MultiSelect
            }
        }

    }
}