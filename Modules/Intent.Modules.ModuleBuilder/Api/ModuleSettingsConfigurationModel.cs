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
    [IntentManaged(Mode.Merge)]
    public partial class ModuleSettingsConfigurationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Module Settings Configuration";
        public const string SpecializationTypeId = "37e678f8-370a-4b05-ae44-69bb3d2b39b6";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ModuleSettingsConfigurationModel(IElement element, string requiredType = SpecializationType)
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

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ModuleSettingsConfigurationModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleSettingsConfigurationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public IList<ModuleSettingsFieldConfigurationModel> Fields => _element.ChildElements
            .GetElementsOfType(ModuleSettingsFieldConfigurationModel.SpecializationTypeId)
            .Select(x => new ModuleSettingsFieldConfigurationModel(x))
            .ToList();
    }

    [IntentManaged(Mode.Fully)]
    public static class ModuleSettingsConfigurationModelExtensions
    {

        public static bool IsModuleSettingsConfigurationModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ModuleSettingsConfigurationModel.SpecializationTypeId;
        }

        public static ModuleSettingsConfigurationModel AsModuleSettingsConfigurationModel(this ICanBeReferencedType type)
        {
            return type.IsModuleSettingsConfigurationModel() ? new ModuleSettingsConfigurationModel((IElement)type) : null;
        }
    }
}