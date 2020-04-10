using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;
using System.Linq;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ModelExtensions
    {
        public const string FileTemplateSettingsStereotype = "File Template Settings";

        //public static CreationMode GetCreationMode(this IHasStereotypes model)
        //{
        //    switch (model.GetStereotypeProperty(FileTemplateSettingsStereotype, "Creation Mode", "File per Model"))
        //    {
        //        case "Single file":
        //            return CreationMode.SingleFile;
        //        case "File per Model":
        //            return CreationMode.FilePerModel;
        //        case "File per Project": // TODO: File per project doesn't always make sense.
        //            return CreationMode.FilePerProject;
        //        case "Custom":
        //            return CreationMode.Custom;
        //        default:
        //            return CreationMode.SingleFile;
        //    }
        //}

        //public static IModelerReference GetModeler(this FileTemplate model)
        //{
        //    return model.GetFileTemplateSettings().Modeler() != null ? new ModelerReference(model.GetFileTemplateSettings().Modeler()) : null;
        //}

        //public static IModelerReference GetModeler(this ICSharpTemplate model)
        //{
        //    return model.GetFileTemplateSettings().Modeler() != null ? new ModelerReference(model.GetFileTemplateSettings().Modeler()) : null;
        //}

        //public static IModelerModelType GetModelType(this FileTemplate model)
        //{
        //    return model.GetFileTemplateSettings().ModelType() != null ? new ModelerModelType(model.GetFileTemplateSettings().ModelType()) : null;
        //}

        //public static IModelerModelType GetModelType(this ICSharpTemplate model)
        //{
        //    return model.GetFileTemplateSettings().ModelType() != null ? new ModelerModelType(model.GetFileTemplateSettings().ModelType()) : null;
        //}

        //public static string GetTemplateBaseType(this FileTemplate model)
        //{
        //    return model.GetStereotypeProperty(FileTemplateSettingsStereotype, "Base Type", "IntentProjectItemTemplateBase");
        //}

        //public static string GetTemplateModelName(this IHasStereotypes model)
        //{
        //    var modelType = model.GetModelType();
        //    if (model.GetCreationMode() == CreationMode.SingleFile)
        //    {
        //        return  modelType == null ? "object" : $"IList<{modelType.InterfaceName}>";
        //    }

        //    return modelType.InterfaceName;
        //}

        //public static string GetModelerName(this IHasStereotypes model)
        //{
        //    return model.GetModeler().Name;
        //}

        //public static string GetModelTypeName(this IHasStereotypes model)
        //{
        //    return model.GetModelType().InterfaceName;
        //}

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

        public static string GetImplementerDecoratorContractType(this TemplateRegistration model)
        {
            return model.GetStereotypeProperty<string>("Implements Decorator Contract", "Type Fullname");
        }

        public static string GetExposedDecoratorContractType(this TemplateRegistration model)
        {
            return model.GetStereotypeProperty<string>("Exposes Decorator Contract", "Type Fullname");
        }
    }
}