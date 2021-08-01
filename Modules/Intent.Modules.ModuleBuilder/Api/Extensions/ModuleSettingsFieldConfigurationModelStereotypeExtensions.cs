using System;
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
            var stereotype = model.GetStereotype("Field Configuration");
            return stereotype != null ? new FieldConfiguration(stereotype) : null;
        }

        public static bool HasFieldConfiguration(this ModuleSettingsFieldConfigurationModel model)
        {
            return model.HasStereotype("Field Configuration");
        }


        public class FieldConfiguration
        {
            private IStereotype _stereotype;

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

            public class ControlTypeOptions
            {
                public readonly string Value;

                public ControlTypeOptions(string value)
                {
                    Value = value;
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

        }

    }
}