using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ElementSettingsExtensions
    {
        public static bool AllowRename(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
        }

        public static bool AllowAbstract(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Abstract");
        }

        public static bool AllowGenericTypes(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Generic Types");
        }

        public static bool AllowMapping(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Mapping");
        }

        public static bool AllowSorting(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Sorting");
        }

        public static bool AllowFindinView(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
        }

        public static bool AllowTypeReference(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Type Reference");
        }

        public static IElement[] TargetTypes(this IElementSettings model)
        {
            return model.GetStereotypeProperty<IElement[]>("Additional Properties", "Target Types");
        }

        public static bool IsStereotypePropertyTarget(this IElementSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Is Stereotype Property Target");
        }

        public static string Text(this IElementSettings model)
        {
            return model.GetStereotypeProperty<string>("Default Creation Options", "Text");
        }

        public static string Shortcut(this IElementSettings model)
        {
            return model.GetStereotypeProperty<string>("Default Creation Options", "Shortcut");
        }

        public static string DefaultName(this IElementSettings model)
        {
            return model.GetStereotypeProperty<string>("Default Creation Options", "Default Name");
        }

        public static int? TypeOrder(this IElementSettings model)
        {
            return model.GetStereotypeProperty<int?>("Default Creation Options", "Type Order");
        }

        public static TypeOptions Type(this IElementSettings model)
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

        public static string Source(this IElementSettings model)
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