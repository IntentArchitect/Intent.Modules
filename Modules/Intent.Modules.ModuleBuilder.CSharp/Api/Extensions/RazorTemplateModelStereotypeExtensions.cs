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
    public static class RazorTemplateModelStereotypeExtensions
    {
        public static RazorTemplateSettings GetRazorTemplateSettings(this RazorTemplateModel model)
        {
            var stereotype = model.GetStereotype(RazorTemplateSettings.DefinitionId);
            return stereotype != null ? new RazorTemplateSettings(stereotype) : null;
        }


        public static bool HasRazorTemplateSettings(this RazorTemplateModel model)
        {
            return model.HasStereotype(RazorTemplateSettings.DefinitionId);
        }

        public static bool TryGetRazorTemplateSettings(this RazorTemplateModel model, out RazorTemplateSettings stereotype)
        {
            if (!HasRazorTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RazorTemplateSettings(model.GetStereotype(RazorTemplateSettings.DefinitionId));
            return true;
        }

        public class RazorTemplateSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "715cfb40-dfcf-47e7-8129-46bdb8e40ee7";

            public RazorTemplateSettings(IStereotype stereotype)
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
                        case "String Interpolation":
                            return TemplatingMethodOptionsEnum.StringInterpolation;
                        case "Razor File Builder":
                            return TemplatingMethodOptionsEnum.RazorFileBuilder;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsStringInterpolation()
                {
                    return Value == "String Interpolation";
                }
                public bool IsRazorFileBuilder()
                {
                    return Value == "Razor File Builder";
                }
            }

            public enum TemplatingMethodOptionsEnum
            {
                StringInterpolation,
                RazorFileBuilder
            }
        }

    }
}