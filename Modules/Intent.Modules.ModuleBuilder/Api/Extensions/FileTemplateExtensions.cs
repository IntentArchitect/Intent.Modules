using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class FileTemplateExtensions
    {
        public static ExposesDecoratorContract GetExposesDecoratorContract(this FileTemplateModel model)
        {
            var stereotype = model.GetStereotype("Exposes Decorator Contract");
            return stereotype != null ? new ExposesDecoratorContract(stereotype) : null;
        }

        public static FileSettings GetFileSettings(this FileTemplateModel model)
        {
            var stereotype = model.GetStereotype("File Settings");
            return stereotype != null ? new FileSettings(stereotype) : null;
        }


        public class ExposesDecoratorContract
        {
            private IStereotype _stereotype;

            public ExposesDecoratorContract(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string TypeFullname()
            {
                return _stereotype.GetProperty<string>("Type Fullname");
            }

        }

        public class FileSettings
        {
            private IStereotype _stereotype;

            public FileSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

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