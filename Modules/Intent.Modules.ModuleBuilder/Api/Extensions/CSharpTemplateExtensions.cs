using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class CSharpTemplateExtensions
    {
        public static ExposesDecoratorContract GetExposesDecoratorContract(this ICSharpTemplate model)
        {
            var stereotype = model.GetStereotype("Exposes Decorator Contract");
            return stereotype != null ? new ExposesDecoratorContract(stereotype) : null;
        }

        public static FileTemplateSettings GetFileTemplateSettings(this ICSharpTemplate model)
        {
            var stereotype = model.GetStereotype("File Template Settings");
            return stereotype != null ? new FileTemplateSettings(stereotype) : null;
        }

        //public static TemplateSettings GetFileTemplateSettings(this ICSharpTemplate model)
        //{
        //    var stereotype = model.GetStereotype("Template Settings");
        //    return stereotype != null ? new TemplateSettings(stereotype) : null;
        //}

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

        public class FileTemplateSettings
        {
            private IStereotype _stereotype;

            public FileTemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public CreationModeOptions CreationMode()
            {
                return new CreationModeOptions(_stereotype.GetProperty<string>("Creation Mode"));
            }

            public IElement Modeler()
            {
                return _stereotype.GetProperty<IElement>("Modeler");
            }

            public IElement ModelType()
            {
                return _stereotype.GetProperty<IElement>("Model Type");
            }

            public string FileExtension()
            {
                return _stereotype.GetProperty<string>("File Extension");
            }

            public class CreationModeOptions
            {
                public readonly string Value;

                public CreationModeOptions(string value)
                {
                    Value = value;
                }

                public bool IsSingleFileNoModel()
                {
                    return Value == "Single File (No Model)";
                }
                public bool IsSingleFileModelList()
                {
                    return Value == "Single File (Model List)";
                }
                public bool IsFileperModel()
                {
                    return Value == "File per Model";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }
        }

        public class TemplateSettings
        {
            private IStereotype _stereotype;

            public TemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public IElement Modeler()
            {
                return _stereotype.GetProperty<IElement>("Modeler");
            }

            public IElement ModelType()
            {
                return _stereotype.GetProperty<IElement>("Model Type");
            }
        }
    }
}