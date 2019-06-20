using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder
{

    public static class FolderExtensions
    {
        public static IList<IElement> GetFolderPath(this IElement model)
        {
            List<IElement> result = new List<IElement>();

            var current = model.ParentElement?.SpecializationType == "Folder" ? model.ParentElement : null;
            while (current != null)
            {
                result.Insert(0, current);
                current = current.ParentElement;
            }
            return result;
        }
    }

    public static class ModelExtensions
    {
        public static RegistrationType GetRegistrationType(this IElement model)
        {
            switch (model.GetStereotypeProperty("Template Settings", "Creation Mode", "File per Model"))
            {
                case "Single File (No Model)":
                    return RegistrationType.SingleFileNoModel;
                case "File per Model":
                    return RegistrationType.FilePerModel;
                case "Single File (Model List)":
                    return RegistrationType.SingleFileListModel;
                case "Custom":
                    return RegistrationType.Custom;
                default:
                    return RegistrationType.SingleFileNoModel;
            }
        }

        public static string GetModelerName(this IElement model)
        {
            return model.GetStereotypeProperty("Template Settings", "Modeler", "Domain");
        }

        public static bool IsCSharpTemplate(this IElement model)
        {
            return model.SpecializationType == "C# Template";
        }

        public static bool IsFileTemplate(this IElement model)
        {
            return model.SpecializationType == "File Template";
        }

        public static string GetTargetModel(this IElement model)
        {
            switch (model.GetRegistrationType())
            {
                case RegistrationType.SingleFileNoModel:
                    return "object";
                case RegistrationType.FilePerModel:
                case RegistrationType.SingleFileListModel:
                    return "IElement";
                case RegistrationType.Custom:
                    return "object";
                default:
                    return "object";
            }
        }
    }
}