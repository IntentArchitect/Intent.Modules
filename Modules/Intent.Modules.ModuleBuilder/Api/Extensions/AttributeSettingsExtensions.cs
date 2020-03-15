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
        public static AdditionalProperties GetAdditionalProperties(this IAttributeSettings model)
        {
            var stereotype = model.GetStereotype("Additional Properties");
            return stereotype != null ? new AdditionalProperties(stereotype) : null;
        }

        public static IconFull GetIconFull(this IAttributeSettings model)
        {
            var stereotype = model.GetStereotype("Icon (Full)");
            return stereotype != null ? new IconFull(stereotype) : null;
        }


        public class AdditionalProperties
        {
            private IStereotype _stereotype;

            public AdditionalProperties(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Text()
            {
                return _stereotype.GetProperty<string>("Text");
            }

            public string Shortcut()
            {
                return _stereotype.GetProperty<string>("Shortcut");
            }

            public string DisplayFunction()
            {
                return _stereotype.GetProperty<string>("Display Function");
            }

            public string DefaultName()
            {
                return _stereotype.GetProperty<string>("Default Name");
            }

            public bool AllowRename()
            {
                return _stereotype.GetProperty<bool>("Allow Rename");
            }

            public bool AllowDuplicateNames()
            {
                return _stereotype.GetProperty<bool>("Allow Duplicate Names");
            }

            public bool AllowFindinView()
            {
                return _stereotype.GetProperty<bool>("Allow Find in View");
            }

            public string DefaultTypeId()
            {
                return _stereotype.GetProperty<string>("Default Type Id");
            }

            public bool IsStereotypePropertyTarget()
            {
                return _stereotype.GetProperty<bool>("Is Stereotype Property Target");
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types");
            }

        }

        public class IconFull
        {
            private IStereotype _stereotype;

            public IconFull(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public string Source()
            {
                return _stereotype.GetProperty<string>("Source");
            }

            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public bool IsUrlImagePath()
                {
                    return Value == "UrlImagePath";
                }
                public bool IsRelativeImagePath()
                {
                    return Value == "RelativeImagePath";
                }
                public bool IsFontAwesome()
                {
                    return Value == "FontAwesome";
                }
                public bool IsCharacterBox()
                {
                    return Value == "CharacterBox";
                }
                public bool IsInternal()
                {
                    return Value == "Internal";
                }
            }

        }

    }
}