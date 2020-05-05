using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class FileTemplateModelExtensions
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

                public bool IsT4Template()
                {
                    return Value == "T4 Template";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

        }

    }
}