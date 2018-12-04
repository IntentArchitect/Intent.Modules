using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder
{
    public static class ModelExtensions
    {
        public static RegistrationType GetRegistrationType(this IClass model)
        {
            switch (model.GetStereotypeProperty("Template Settings", "Registration", "Per-Model"))
            {
                case "No-Model":
                    return RegistrationType.SingleFileNoModel;
                case "Per-Model":
                    return RegistrationType.FilePerModel;
                case "Per-Model-List":
                    return RegistrationType.SingleFileListModel;
                case "Custom":
                    return RegistrationType.Custom;
                default:
                    return RegistrationType.SingleFileNoModel;
            }
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