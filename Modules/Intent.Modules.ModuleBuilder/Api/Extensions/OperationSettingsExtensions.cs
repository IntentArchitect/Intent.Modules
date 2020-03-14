using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class OperationSettingsExtensions
    {
        public static string Text(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Text");
        }

        public static string Shortcut(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Shortcut");
        }

        public static string DisplayFunction(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Display Function");
        }

        public static string DefaultName(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Default Name");
        }

        public static bool AllowRename(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
        }

        public static bool AllowDuplicateNames(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Duplicate Names");
        }

        public static bool AllowFindinView(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
        }

        public static string DefaultTypeId(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Default Type Id");
        }

        public static bool IsStereotypePropertyTarget(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Is Stereotype Property Target");
        }

        public static IElement[] TargetTypes(this IOperationSettings model)
        {
            return model.GetStereotypeProperty<IElement[]>("Additional Properties", "Target Types");
        }

    }
}