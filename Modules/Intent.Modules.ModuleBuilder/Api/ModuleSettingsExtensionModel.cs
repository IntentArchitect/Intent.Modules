using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ModuleSettingsExtensionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Module Settings Extension";
        public const string SpecializationTypeId = "72044cdf-1f96-4ad2-86be-8d5468725e67";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ModuleSettingsExtensionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        public IList<ModuleSettingsFieldConfigurationModel> Fields => _element.ChildElements
            .GetElementsOfType(ModuleSettingsFieldConfigurationModel.SpecializationTypeId)
            .Select(x => new ModuleSettingsFieldConfigurationModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ModuleSettingsExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleSettingsExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ModuleSettingsExtensionModelExtensions
    {

        public static bool IsModuleSettingsExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ModuleSettingsExtensionModel.SpecializationTypeId;
        }

        public static ModuleSettingsExtensionModel AsModuleSettingsExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsModuleSettingsExtensionModel() ? new ModuleSettingsExtensionModel((IElement)type) : null;
        }
    }
}