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
        public static string TypeFullname(this IFileTemplate model)
        {
            return model.GetStereotypeProperty<string>("Exposes Decorator Contract", "Type Fullname");
        }

        public static CreationModeOptions CreationMode(this IFileTemplate model)
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

        public static IElement Modeler(this IFileTemplate model)
        {
            return model.GetStereotypeProperty<IElement>("File Template Settings", "Modeler");
        }

        public static IElement ModelType(this IFileTemplate model)
        {
            return model.GetStereotypeProperty<IElement>("File Template Settings", "Model Type");
        }

        public static string FileExtension(this IFileTemplate model)
        {
            return model.GetStereotypeProperty<string>("File Template Settings", "File Extension");
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