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
    public class MappingSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Mapping Settings";
        public const string SpecializationTypeId = "a901c634-6482-4993-ae3c-bd1b637f78d4";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappingSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public SourceMappingSettingsModel SourceMapping => _element.ChildElements
            .GetElementsOfType(SourceMappingSettingsModel.SpecializationTypeId)
            .Select(x => new SourceMappingSettingsModel(x))
            .SingleOrDefault();

        public TargetMappingSettingsModel TargetMapping => _element.ChildElements
            .GetElementsOfType(TargetMappingSettingsModel.SpecializationTypeId)
            .Select(x => new TargetMappingSettingsModel(x))
            .SingleOrDefault();

        public IList<MappingTypeSettingsModel> MappingTypes => _element.ChildElements
            .GetElementsOfType(MappingTypeSettingsModel.SpecializationTypeId)
            .Select(x => new MappingTypeSettingsModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappingSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappingSettingsModel)obj);
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
                SourceRootElementFunction = SourceMapping.GetMappingEndSettings().RootElementFunction(),
                TargetRootElementFunction = TargetMapping.GetMappingEndSettings().RootElementFunction(),
                Title = Name,
                SourceSettings = SourceMapping?.ElementMappings.Select(x => x.ToPersistable()).ToList(),
                TargetSettings = TargetMapping?.ElementMappings.Select(x => x.ToPersistable()).ToList(),
                MappingTypes = MappingTypes.Select(x => x.ToPersistable()).ToList()
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappingSettingsModelExtensions
    {

        public static bool IsMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappingSettingsModel.SpecializationTypeId;
        }

        public static MappingSettingsModel AsMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsMappingSettingsModel() ? new MappingSettingsModel((IElement)type) : null;
        }
    }
}