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
    public static class FileTemplateModelStereotypeExtensions
    {
        public static FileSettings GetFileSettings(this FileTemplateModel model)
        {
            var stereotype = model.GetStereotype("File Settings");
            return stereotype != null ? new FileSettings(stereotype) : null;
        }

        public static bool HasFileSettings(this FileTemplateModel model)
        {
            return model.HasStereotype("File Settings");
        }

        public static bool TryGetFileSettings(this FileTemplateModel model, out FileSettings stereotype)
        {
            if (!HasFileSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileSettings(model.GetStereotype("File Settings"));
            return true;
        }


        public class FileSettings
        {
            private IStereotype _stereotype;

            public FileSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string FileExtension()
            {
                return _stereotype.GetProperty<string>("File Extension");
            }

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