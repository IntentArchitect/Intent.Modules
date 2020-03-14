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
        public static string TypeFullname(this ICSharpTemplate model)
        {
            return model.GetStereotypeProperty<string>("Exposes Decorator Contract", "Type Fullname");
        }

        public static CreationModeOptions CreationMode(this ICSharpTemplate model)
        {
            var result = model.GetStereotypeProperty<string>("File Template Settings", "Creation Mode");
            switch (result)
            {
                case "Single File (No Model)":
                    return CreationModeOptions.SingleFileNoModel;
                case "Single File (Model List)":
                    return CreationModeOptions.SingleFileModelList;
                case "File per Model":
                    return CreationModeOptions.FileperModel;
                case "Custom":
                    return CreationModeOptions.Custom;
                default:
                    throw new ArgumentOutOfRangeException("File Template Settings -> Creation Mode", result, $"Invalid value: {result}");
            }
        }

        public static IElement Modeler(this ICSharpTemplate model)
        {
            return model.GetStereotypeProperty<IElement>("File Template Settings", "Modeler");
        }

        public static IElement ModelType(this ICSharpTemplate model)
        {
            return model.GetStereotypeProperty<IElement>("File Template Settings", "Model Type");
        }

        public static string BaseType(this ICSharpTemplate model)
        {
            return model.GetStereotypeProperty<string>("File Template Settings", "Base Type");
        }

        public enum CreationModeOptions
        {
            SingleFileNoModel,
            SingleFileModelList,
            FileperModel,
            Custom,
        }

    }
}