using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class TemplateSettingsExtensions
    {
        public const string SingleFileId = "f65d2904-88c9-4501-873a-a4eec8303b1d";
        public const string FilePerModelId = "99cb461f-ec82-4dde-a804-e3393a5e2a8d";
        public const string CustomId = "d2931361-7cf0-4c79-9847-621c60886ef9";

        public static bool ReferencesSingleFile(this IElement element)
        {
            return element.TypeReference?.Element?.Id == SingleFileId;
        }

        public static bool ReferencesFilePerModel(this IElement element)
        {
            return element.TypeReference?.Element?.Id == FilePerModelId;
        }

        public static bool ReferencesCustom(this IElement element)
        {
            return element.TypeReference?.Element?.Id == CustomId;
        }

        public static TemplateSettings GetTemplateSettings(this TemplateRegistrationModel element)
        {
            var stereotype = element.GetStereotype("Template Settings");
            return stereotype != null ? new TemplateSettings(stereotype) : null;
        }

        public class TemplateSettings
        {
            private readonly IStereotype _stereotype;

            public TemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public DesignerModel Designer()
            {
                var designerElement = _stereotype.GetProperty<IElement>("Designer");
                return designerElement != null ? new DesignerModel(designerElement) : null;
            }

            public TemplateSettingsSource Source()
            {
                return new TemplateSettingsSource(_stereotype.GetProperty<string>("Source"));
            }

            public IElement ModelType()
            {
                return _stereotype.GetProperty<IElement>("Model Type");
            }

            public string ModelName()
            {
                return _stereotype.GetProperty<string>("Model Name");
            }

            public class TemplateSettingsSource
            {
                public readonly string Value;

                public TemplateSettingsSource(string value)
                {
                    Value = value;
                }

                public bool IsLookupType()
                {
                    return Value == "Lookup Type";
                }
                public bool IsCustomType()
                {
                    return Value == "Custom Type";
                }
            }

            public string GetRole()
            {
                return _stereotype.GetProperty<string>("Role");
            }

            public string GetDefaultLocation()
            {
                return _stereotype.GetProperty<string>("Default Location");
            }

            public SeverityOptions Severity()
            {
                return new SeverityOptions(_stereotype.GetProperty<string>("Severity"));
            }

            public string Classification()
            {
                return _stereotype.GetProperty<string>("Classification");
            }

            public class SeverityOptions
            {
                public readonly string Value;

                public SeverityOptions(string value)
                {
                    Value = value;
                }

                public SeverityOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "0 - None":
                            return SeverityOptionsEnum._0None;
                        case "1 - Low":
                            return SeverityOptionsEnum._1Low;
                        case "2 - Medium":
                            return SeverityOptionsEnum._2Medium;
                        case "3 - High":
                            return SeverityOptionsEnum._3High;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool Is0None()
                {
                    return Value == "0 - None";
                }
                public bool Is1Low()
                {
                    return Value == "1 - Low";
                }
                public bool Is2Medium()
                {
                    return Value == "2 - Medium";
                }
                public bool Is3High()
                {
                    return Value == "3 - High";
                }
            }

            public enum SeverityOptionsEnum
            {
                _0None,
                _1Low,
                _2Medium,
                _3High
            }
        }
    }
}
