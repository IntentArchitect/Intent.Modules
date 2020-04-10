using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class CoreTypeExtensions
    {
        public static DefaultCreationOptions GetDefaultCreationOptions(this CoreType model)
        {
            var stereotype = model.GetStereotype("Default Creation Options");
            return stereotype != null ? new DefaultCreationOptions(stereotype) : null;
        }

        public static IconFull GetIconFull(this CoreType model)
        {
            var stereotype = model.GetStereotype("Icon (Full)");
            return stereotype != null ? new IconFull(stereotype) : null;
        }


        public class DefaultCreationOptions
        {
            private IStereotype _stereotype;

            public DefaultCreationOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Shortcut()
            {
                return _stereotype.GetProperty<string>("Shortcut");
            }

            public string DefaultName()
            {
                return _stereotype.GetProperty<string>("Default Name");
            }

            public int? TypeOrder()
            {
                return _stereotype.GetProperty<int?>("Type Order");
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