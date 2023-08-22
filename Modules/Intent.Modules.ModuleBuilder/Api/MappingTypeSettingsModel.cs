using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Mappings;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MappingTypeSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Mapping Type Settings";
        public const string SpecializationTypeId = "a901c634-6482-4993-ae3c-bd1b637f78d4";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappingTypeSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public FromMappingSettingsModel FromMapping => _element.ChildElements
            .GetElementsOfType(FromMappingSettingsModel.SpecializationTypeId)
            .Select(x => new FromMappingSettingsModel(x))
            .SingleOrDefault();

        public ToMappingSettingsModel ToMapping => _element.ChildElements
            .GetElementsOfType(ToMappingSettingsModel.SpecializationTypeId)
            .Select(x => new ToMappingSettingsModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappingTypeSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappingTypeSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Ignore)]
        public MappingElementToElementSettingsPersistable ToPersistable()
        {
            return new MappingElementToElementSettingsPersistable()
            {
                MappingTypeId = Id,
                MappingType = Name,
                FromRootElementFunction = null,
                Title = Name,
                FromMappings = FromMapping?.ElementMappings.Select(x => x.ToPersistable()).ToList(),
                ToMappings = ToMapping?.ElementMappings.Select(x => x.ToPersistable()).ToList(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappingTypeSettingsModelExtensions
    {

        public static bool IsMappingTypeSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappingTypeSettingsModel.SpecializationTypeId;
        }

        public static MappingTypeSettingsModel AsMappingTypeSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsMappingTypeSettingsModel() ? new MappingTypeSettingsModel((IElement)type) : null;
        }
    }
}