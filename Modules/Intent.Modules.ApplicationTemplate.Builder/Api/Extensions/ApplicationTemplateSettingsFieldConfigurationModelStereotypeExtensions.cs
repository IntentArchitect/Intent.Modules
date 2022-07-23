using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions
    {
        public static SettingsFieldConfiguration GetSettingsFieldConfiguration(this ApplicationTemplateSettingsFieldConfigurationModel model)
        {
            var stereotype = model.GetStereotype("Settings Field Configuration");
            return stereotype != null ? new SettingsFieldConfiguration(stereotype) : null;
        }


        public static bool HasSettingsFieldConfiguration(this ApplicationTemplateSettingsFieldConfigurationModel model)
        {
            return model.HasStereotype("Settings Field Configuration");
        }


        public class SettingsFieldConfiguration
        {
            private IStereotype _stereotype;

            public SettingsFieldConfiguration(IStereotype stereotype)
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
                        case "Hidden":
                            return ControlTypeOptionsEnum.Hidden;
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
                public bool IsHidden()
                {
                    return Value == "Hidden";
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
                MultiSelect,
                Hidden
            }
        }

    }
}