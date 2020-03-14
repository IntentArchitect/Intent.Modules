using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class AttributeSettingsExtensions
    {
        public static string Text(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Text");
        }

        public static string Shortcut(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Shortcut");
        }

        public static string DisplayFunction(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Display Function");
        }

        public static string DefaultName(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Default Name");
        }

        public static bool AllowRename(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
        }

        public static bool AllowDuplicateNames(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Duplicate Names");
        }

        public static bool AllowFindinView(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
        }

        public static string DefaultTypeId(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Default Type Id");
        }

        public static bool IsStereotypePropertyTarget(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Is Stereotype Property Target");
        }

        public static IElement[] TargetTypes(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<IElement[]>("Additional Properties", "Target Types");
        }

        public static TypeOptions Type(this IAttributeSettings model)
        {
            var result = model.GetStereotypeProperty<string>("Icon (Full)", "Type");
            switch (result)
            {
                case "UrlImagePath":
                    return TypeOptions.UrlImagePath;
                case "RelativeImagePath":
                    return TypeOptions.RelativeImagePath;
                case "FontAwesome":
                    return TypeOptions.FontAwesome;
                case "CharacterBox":
                    return TypeOptions.CharacterBox;
                case "Internal":
                    return TypeOptions.Internal;
                default:
                    throw new ArgumentOutOfRangeException("Icon (Full) -> Type", result, $"Invalid value: {result}");
            }
        }

        public static string Source(this IAttributeSettings model)
        {
            return model.GetStereotypeProperty<string>("Icon (Full)", "Source");
        }

        public enum TypeOptions
        {
            UrlImagePath,
            RelativeImagePath,
            FontAwesome,
            CharacterBox,
            Internal,
        }

    }
}