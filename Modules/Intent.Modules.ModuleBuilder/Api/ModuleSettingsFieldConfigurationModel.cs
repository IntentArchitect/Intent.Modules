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
    public partial class ModuleSettingsFieldConfigurationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Module Settings Field Configuration";
        public const string SpecializationTypeId = "88e29cab-1342-40c7-b052-5fcd68ffafec";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ModuleSettingsFieldConfigurationModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(ModuleSettingsFieldConfigurationModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleSettingsFieldConfigurationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public IList<ModuleSettingsFieldOptionModel> Options => _element.ChildElements
            .GetElementsOfType(ModuleSettingsFieldOptionModel.SpecializationTypeId)
            .Select(x => new ModuleSettingsFieldOptionModel(x))
            .ToList();
    }

    [IntentManaged(Mode.Fully)]
    public static class ModuleSettingsFieldConfigurationModelExtensions
    {

        public static bool IsModuleSettingsFieldConfigurationModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ModuleSettingsFieldConfigurationModel.SpecializationTypeId;
        }

        public static ModuleSettingsFieldConfigurationModel AsModuleSettingsFieldConfigurationModel(this ICanBeReferencedType type)
        {
            return type.IsModuleSettingsFieldConfigurationModel() ? new ModuleSettingsFieldConfigurationModel((IElement)type) : null;
        }
    }
}