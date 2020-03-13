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
            return model.GetStereotypeProperty<string>("File Template", "Type Fullname");
        }

        public static CreationModeOptions CreationMode(this IFileTemplate model)
        {
            var result = model.GetStereotypeProperty<string>("File Template", "Creation Mode");
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IElement Modeler(this IFileTemplate model)
        {
            return model.GetStereotypeProperty<IElement>("File Template", "Modeler");
        }

        public static IElement ModelType(this IFileTemplate model)
        {
            return model.GetStereotypeProperty<IElement>("File Template", "Model Type");
        }

        public static string BaseType(this IFileTemplate model)
        {
            return model.GetStereotypeProperty<string>("File Template", "Base Type");
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