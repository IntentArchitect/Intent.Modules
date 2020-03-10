using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;
using Intent.Modules.ModuleBuilder.Api;

namespace Intent.Modules.ModuleBuilder.Helpers
{
    public static class ModelExtensions
    {
        public const string FileTemplateSettingsStereotype = "File Template Settings";

        public static CreationMode GetCreationMode(this IModuleBuilderElement model)
        {
            switch (model.GetStereotypeProperty(FileTemplateSettingsStereotype, "Creation Mode", "File per Model"))
            {
                case "Single file":
                    return CreationMode.SingleFile;
                case "File per model":
                    return CreationMode.FilePerModel;
                case "File per project": // TODO: File per project doesn't always make sense.
                    return CreationMode.FilePerProject;
                case "Custom":
                    return CreationMode.Custom;
                default:
                    return CreationMode.SingleFile;
            }
        }

        public static string GetTemplateBaseType(this IFileTemplate model)
        {
            return model.GetStereotypeProperty(FileTemplateSettingsStereotype, "Base Type", "IntentProjectItemTemplateBase");
        }

        public static string GetTemplateModelName(this IFileTemplate model)
        {
            var modelType = model.GetModelType();
            if (model.GetCreationMode() == CreationMode.SingleFile)
            {
                return  modelType == null ? "object" : $"IList<{modelType.Name}>";
            }

            return modelType.Name;
        }

        //public static string GetModelerName(this IModuleBuilderElement model)
        //{
        //    return model.GetStereotypeProperty(TemplateSettingsStereotype, "Modeler", "Modeler");
        //}

        //public static string GetTargetModel(this IModuleBuilderElement model)
        //{
        //    if (model == null)
        //    {
        //        return string.Empty;
        //    }

        //    var selectedCreationMode = model.GetCreationMode();

        //    if (selectedCreationMode == CreationMode.SingleFileNoModel)
        //    {
        //        return "object";
        //    }

        //    if (GetModelerName(model) == "Custom")
        //    {
        //        var customModel = model.GetStereotypeProperty<string>(TemplateSettingsStereotype, "Custom Model", "IClass");
        //        if (string.IsNullOrWhiteSpace(customModel))
        //        {
        //            throw new Exception($"Model {model.Name} has a Creation Mode of 'Custom' but nothing specified in 'Custom Model'");
        //        }

        //        return customModel;
        //    }
        //    else
        //    {
        //        switch (selectedCreationMode)
        //        {
        //            case CreationMode.FilePerModel:
        //            case CreationMode.SingleFileListModel:
        //                return "IClass";
        //            case CreationMode.Custom:
        //            default:
        //                return "IClass";
        //        }
        //    }
        //}

        public static string GetImplementerDecoratorContractType(this IModuleBuilderElement model)
        {
            return model.GetStereotypeProperty<string>("Implements Decorator Contract", "Type Fullname");
        }

        public static string GetExposedDecoratorContractType(this IModuleBuilderElement model)
        {
            return model.GetStereotypeProperty<string>("Exposes Decorator Contract", "Type Fullname");
        }
    }
}