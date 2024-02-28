using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Dart.Api
{
    public static class DartFileTemplateModelStereotypeExtensions
    {
        public static DartTemplateSettings GetDartTemplateSettings(this DartFileTemplateModel model)
        {
            var stereotype = model.GetStereotype("c38634e8-d893-435b-9d19-9aec9a2812cd");
            return stereotype != null ? new DartTemplateSettings(stereotype) : null;
        }


        public static bool HasDartTemplateSettings(this DartFileTemplateModel model)
        {
            return model.HasStereotype("c38634e8-d893-435b-9d19-9aec9a2812cd");
        }

        public static bool TryGetDartTemplateSettings(this DartFileTemplateModel model, out DartTemplateSettings stereotype)
        {
            if (!HasDartTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DartTemplateSettings(model.GetStereotype("c38634e8-d893-435b-9d19-9aec9a2812cd"));
            return true;
        }

        public class DartTemplateSettings
        {
            private IStereotype _stereotype;

            public DartTemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TemplatingMethodOptions TemplatingMethod()
            {
                return new TemplatingMethodOptions(_stereotype.GetProperty<string>("Templating Method"));
            }

            public class TemplatingMethodOptions
            {
                public readonly string Value;

                public TemplatingMethodOptions(string value)
                {
                    Value = value;
                }

                public TemplatingMethodOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "T4 Template":
                            return TemplatingMethodOptionsEnum.T4Template;
                        case "String Interpolation":
                            return TemplatingMethodOptionsEnum.StringInterpolation;
                        case "Custom":
                            return TemplatingMethodOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsT4Template()
                {
                    return Value == "T4 Template";
                }
                public bool IsStringInterpolation()
                {
                    return Value == "String Interpolation";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum TemplatingMethodOptionsEnum
            {
                T4Template,
                StringInterpolation,
                Custom
            }
        }

    }
}