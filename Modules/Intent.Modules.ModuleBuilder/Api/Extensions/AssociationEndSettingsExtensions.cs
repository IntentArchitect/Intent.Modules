using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class AssociationEndSettingsExtensions
    {
        public static AdditionalProperties GetAdditionalProperties(this IAssociationEndSettings model)
        {
            var stereotype = model.GetStereotype("Additional Properties");
            return stereotype != null ? new AdditionalProperties(stereotype) : null;
        }


        public class AdditionalProperties
        {
            private IStereotype _stereotype;

            public AdditionalProperties(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types");
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

        }

    }
}