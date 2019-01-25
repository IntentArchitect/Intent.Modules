using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder
{
    public static class ModelExtensions
    {
        public static RegistrationType GetRegistrationType(this IClass model)
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

        public static string GetModelerName(this IClass model)
        {
            return model.GetStereotypeProperty("Template Settings", "Modeler", "Domain");
        }

        public static bool IsCSharpTemplate(this IClass model)
        {
            return model.SpecializationType == "C# Template";
        }

        public static bool IsFileTemplate(this IClass model)
        {
            return model.SpecializationType == "File Template";
        }

        public static string GetTargetModel(this IClass model)
        {
            switch (model.GetRegistrationType())
            {
                case RegistrationType.SingleFileNoModel:
                    return "object";
                case RegistrationType.FilePerModel:
                case RegistrationType.SingleFileListModel:
                    return "IClass";
                case RegistrationType.Custom:
                    return "object";
                default:
                    return "object";
            }
        }
    }
}