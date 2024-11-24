using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.TypeScript.Api
{
    public static class TypescriptFileTemplateModelStereotypeExtensions
    {
        public static TypeScriptTemplateSettings GetTypeScriptTemplateSettings(this TypescriptFileTemplateModel model)
        {
            var stereotype = model.GetStereotype(TypeScriptTemplateSettings.DefinitionId);
            return stereotype != null ? new TypeScriptTemplateSettings(stereotype) : null;
        }


        public static bool HasTypeScriptTemplateSettings(this TypescriptFileTemplateModel model)
        {
            return model.HasStereotype(TypeScriptTemplateSettings.DefinitionId);
        }

        public static bool TryGetTypeScriptTemplateSettings(this TypescriptFileTemplateModel model, out TypeScriptTemplateSettings stereotype)
        {
            if (!HasTypeScriptTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TypeScriptTemplateSettings(model.GetStereotype(TypeScriptTemplateSettings.DefinitionId));
            return true;
        }

        public class TypeScriptTemplateSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8f73a680-1a9e-4df6-a396-759bb078a99e";

            public TypeScriptTemplateSettings(IStereotype stereotype)
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
                        case "Type Script File Builder":
                            return TemplatingMethodOptionsEnum.TypeScriptFileBuilder;
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
                public bool IsTypeScriptFileBuilder()
                {
                    return Value == "Type Script File Builder";
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
                TypeScriptFileBuilder,
                Custom
            }
        }

    }
}