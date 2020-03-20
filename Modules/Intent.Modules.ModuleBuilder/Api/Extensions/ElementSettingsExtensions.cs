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
        public static AdditionalProperties GetAdditionalProperties(this IElementSettings model)
        {
            var stereotype = model.GetStereotype("Additional Properties");
            return stereotype != null ? new AdditionalProperties(stereotype) : null;
        }

        public static DefaultCreationOptions GetDefaultCreationOptions(this IElementSettings model)
        {
            var stereotype = model.GetStereotype("Default Creation Options");
            return stereotype != null ? new DefaultCreationOptions(stereotype) : null;
        }

        public static IconFull GetIconFull(this IElementSettings model)
        {
            var stereotype = model.GetStereotype("Icon (Full)");
            return stereotype != null ? new IconFull(stereotype) : null;
        }

        public static IconFullExpanded GetIconFullExpanded(this IElementSettings model)
        {
            var stereotype = model.GetStereotype("Icon (Full, Expanded)");
            return stereotype != null ? new IconFullExpanded(stereotype) : null;
        }


        public class AdditionalProperties
        {
            private IStereotype _stereotype;

            public AdditionalProperties(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string DisplayTextFunction()
            {
                return _stereotype.GetProperty<string>("Display Text Function");
            }

            public bool AllowRename()
            {
                return _stereotype.GetProperty<bool>("Allow Rename");
            }

            public bool AllowAbstract()
            {
                return _stereotype.GetProperty<bool>("Allow Abstract");
            }

            public bool AllowGenericTypes()
            {
                return _stereotype.GetProperty<bool>("Allow Generic Types");
            }

            public bool AllowMapping()
            {
                return _stereotype.GetProperty<bool>("Allow Mapping");
            }

            public bool AllowSorting()
            {
                return _stereotype.GetProperty<bool>("Allow Sorting");
            }

            public bool AllowFindInView()
            {
                return _stereotype.GetProperty<bool>("Allow Find in View");
            }

            public TypeReferenceOptions TypeReference()
            {
                return new TypeReferenceOptions(_stereotype.GetProperty<string>("Type Reference"));
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types");
            }

            public string DefaultTypeId()
            {
                return _stereotype.GetProperty<string>("Default Type Id");
            }

            public bool AllowNullable()
            {
                return _stereotype.GetProperty<bool>("Allow Nullable");
            }

            public bool AllowCollection()
            {
                return _stereotype.GetProperty<bool>("Allow Collection");
            }

            public class TypeReferenceOptions
            {
                public readonly string Value;

                public TypeReferenceOptions(string value)
                {
                    Value = value;
                }

                public bool IsDisabled()
                {
                    return Value == "Disabled";
                }
                public bool IsOptional()
                {
                    return Value == "Optional";
                }
                public bool IsRequired()
                {
                    return Value == "Required";
                }
            }

        }

        public class DefaultCreationOptions
        {
            private IStereotype _stereotype;

            public DefaultCreationOptions(IStereotype stereotype)
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

        public class IconFullExpanded
        {
            private IStereotype _stereotype;

            public IconFullExpanded(IStereotype stereotype)
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