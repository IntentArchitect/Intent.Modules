using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class LiteralSettingsExtensions
    {
        public static string Text(this ILiteralSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Text");
        }

        public static string Shortcut(this ILiteralSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Shortcut");
        }

        public static string DefaultName(this ILiteralSettings model)
        {
            return model.GetStereotypeProperty<string>("Additional Properties", "Default Name");
        }

        public static bool AllowRename(this ILiteralSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
        }

        public static bool AllowDuplicateNames(this ILiteralSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Duplicate Names");
        }

        public static bool AllowFindinView(this ILiteralSettings model)
        {
            return model.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
        }

        public static TypeOptions Type(this ILiteralSettings model)
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

        public static string Source(this ILiteralSettings model)
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