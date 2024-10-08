using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    public static class CSharpTemplateModelStereotypeExtensions
    {
        public static CSharpTemplateSettings GetCSharpTemplateSettings(this CSharpTemplateModel model)
        {
            var stereotype = model.GetStereotype(CSharpTemplateSettings.DefinitionId);
            return stereotype != null ? new CSharpTemplateSettings(stereotype) : null;
        }

        public static bool HasCSharpTemplateSettings(this CSharpTemplateModel model)
        {
            return model.HasStereotype(CSharpTemplateSettings.DefinitionId);
        }

        public static bool TryGetCSharpTemplateSettings(this CSharpTemplateModel model, out CSharpTemplateSettings stereotype)
        {
            if (!HasCSharpTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CSharpTemplateSettings(model.GetStereotype(CSharpTemplateSettings.DefinitionId));
            return true;
        }


        public class CSharpTemplateSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "78be0482-1cd7-4b9f-b457-5ebe1536786e";

            public CSharpTemplateSettings(IStereotype stereotype)
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
                        case "C# File Builder":
                            return TemplatingMethodOptionsEnum.CSharpFileBuilder;
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
                public bool IsCSharpFileBuilder()
                {
                    return Value == "C# File Builder";
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
                CSharpFileBuilder,
                Custom
            }
        }

    }
}