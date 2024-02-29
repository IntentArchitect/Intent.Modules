using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Java.Api
{
    public static class JavaFileTemplateModelStereotypeExtensions
    {
        public static JavaTemplateSettings GetJavaTemplateSettings(this JavaFileTemplateModel model)
        {
            var stereotype = model.GetStereotype("d9a52e8e-dfa4-4604-9c04-8e023f2f7418");
            return stereotype != null ? new JavaTemplateSettings(stereotype) : null;
        }


        public static bool HasJavaTemplateSettings(this JavaFileTemplateModel model)
        {
            return model.HasStereotype("d9a52e8e-dfa4-4604-9c04-8e023f2f7418");
        }

        public static bool TryGetJavaTemplateSettings(this JavaFileTemplateModel model, out JavaTemplateSettings stereotype)
        {
            if (!HasJavaTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new JavaTemplateSettings(model.GetStereotype("d9a52e8e-dfa4-4604-9c04-8e023f2f7418"));
            return true;
        }


        public class JavaTemplateSettings
        {
            private IStereotype _stereotype;

            public JavaTemplateSettings(IStereotype stereotype)
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
                        case "Java File Builder":
                            return TemplatingMethodOptionsEnum.JavaFileBuilder;
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
                public bool IsJavaFileBuilder()
                {
                    return Value == "Java File Builder";
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
                JavaFileBuilder,
                Custom
            }
        }

    }
}