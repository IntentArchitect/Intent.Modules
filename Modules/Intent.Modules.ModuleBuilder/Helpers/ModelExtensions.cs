using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;

namespace Intent.Modules.ModuleBuilder.Helpers
{
    public static class ModelExtensions
    {
        private const string TemplateSettingsStereotype = "Template Settings";

        public static CreationMode GetCreationMode(this IClass model)
        {
            switch (model.GetStereotypeProperty(TemplateSettingsStereotype, "Creation Mode", "File per Model"))
            {
                case "Single File (No Model)":
                    return CreationMode.SingleFileNoModel;
                case "File per Model":
                    return CreationMode.FilePerModel;
                case "Single File (Model List)":
                    return CreationMode.SingleFileListModel;
                case "Custom":
                    return CreationMode.Custom;
                default:
                    return CreationMode.SingleFileNoModel;
            }
        }

        public static string GetModelerName(this IClass model)
        {
            return model.GetStereotypeProperty(TemplateSettingsStereotype, "Modeler", "Domain");
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
            if (model == null)
            {
                return string.Empty;
            }

            var selectedCreationMode = model.GetCreationMode();

            if (selectedCreationMode == CreationMode.SingleFileNoModel)
            {
                return "object";
            }

            if (GetModelerName(model) == "Custom")
            {
                var customModel = model.GetStereotypeProperty<string>(TemplateSettingsStereotype, "Custom Model", "IClass");
                if (string.IsNullOrWhiteSpace(customModel))
                {
                    throw new Exception($"Model {model.Name} has a Creation Mode of 'Custom' but nothing specified in 'Custom Model'");
                }

                return customModel;
            }
            else
            {
                switch (selectedCreationMode)
                {
                    case CreationMode.FilePerModel:
                    case CreationMode.SingleFileListModel:
                        return "IClass";
                    case CreationMode.Custom:
                    default:
                        return "IClass";
                }
            }
        }
    }
}