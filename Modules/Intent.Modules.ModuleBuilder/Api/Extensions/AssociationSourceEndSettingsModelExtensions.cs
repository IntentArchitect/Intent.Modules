using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class AssociationSourceEndSettingsModelExtensions
    {
        public static Settings GetSettings(this AssociationSourceEndSettingsModel model)
        {
            var stereotype = model.GetStereotype("Settings");
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this AssociationSourceEndSettingsModel model)
        {
            return model.HasStereotype("Settings");
        }


        public class Settings
        {
            private IStereotype _stereotype;

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types");
            }

            public string ApiPropertyName()
            {
                return _stereotype.GetProperty<string>("Api Property Name");
            }

            public bool IsNavigableEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Navigable Enabled");
            }

            public bool IsNullableEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Nullable Enabled");
            }

            public bool IsCollectionEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Collection Enabled");
            }

            public bool IsNavigableDefault()
            {
                return _stereotype.GetProperty<bool>("Is Navigable Default");
            }

            public bool IsNullableDefault()
            {
                return _stereotype.GetProperty<bool>("Is Nullable Default");
            }

            public bool IsCollectionDefault()
            {
                return _stereotype.GetProperty<bool>("Is Collection Default");
            }

            public bool AllowMultiple()
            {
                return _stereotype.GetProperty<bool>("Allow Multiple");
            }

        }

    }
}