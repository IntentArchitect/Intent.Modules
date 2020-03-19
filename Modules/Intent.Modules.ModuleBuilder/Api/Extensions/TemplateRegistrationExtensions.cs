using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class TemplateSettingsExtensions
    {
        private static string[] TargetElementIds = new[]
        {
            "f65d2904-88c9-4501-873a-a4eec8303b1d", // Single File
            "99cb461f-ec82-4dde-a804-e3393a5e2a8d", // File per Model
            "d2931361-7cf0-4c79-9847-621c60886ef9"  // Custom
        };

        public static bool ReferencesSingleFile(IElement element)
        {
            return element.TypeReference.Element.Id == TargetElementIds[0];
        }

        public static bool ReferencesFilePerModel(IElement element)
        {
            return element.TypeReference.Element.Id == TargetElementIds[1];
        }

        public static bool ReferencesCustom(IElement element)
        {
            return element.TypeReference.Element.Id == TargetElementIds[2];
        }

        public static TemplateSettings GetTemplateSettings(IElement element)
        {
            if (TargetElementIds.Contains(element.TypeReference?.Element?.Id))
            {
                return new TemplateSettings(element.GetStereotype("Template Settings"));
            }
            return null;
        }

        public class TemplateSettings
        {
            private readonly IStereotype _stereotype;

            public TemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public IElement ModelType()
            {
                return _stereotype.GetProperty<IElement>("Model Type");
            }
        }
    }
}