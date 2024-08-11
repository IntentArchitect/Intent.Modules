using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MappingTypeSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Mapping Type Settings";
        public const string SpecializationTypeId = "dd8e8a63-140c-41c8-b812-0dc923012fac";
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

        [IntentIgnore]
        public AdvancedMappingTypePersistable ToPersistable()
        {
            return new AdvancedMappingTypePersistable
            {
                MappingTypeId = Id,
                MappingType = Name,
                SourceTypes = this.GetMappingTypeSettings().SourceTypes()?.Select(x => new MappableElementSettingIdentifierPersistable() { Id = x.Id, Name = x.Name }).ToList(),
                SourceFilterFunction = this.GetMappingTypeSettings().Sources().IsDataTypes() ? "return element.represents == 'data'"
                    : this.GetMappingTypeSettings().Sources().IsInvokableTypes() ? "return element.represents == 'invokable'"
                    : this.GetMappingTypeSettings().Sources().IsEventTypes() ? "return element.represents == 'event'"
                    : this.GetMappingTypeSettings().SourceTypesFilter(),
                SourceArrowFunction = this.GetMappingTypeSettings().SourceArrowType().IsSolidArrow()
                    ? "return `M ${x} ${y} l 10 5 l 0 -10 z`" : "return null",
                TargetTypes = this.GetMappingTypeSettings().TargetTypes()?.Select(x => new MappableElementSettingIdentifierPersistable() { Id = x.Id, Name = x.Name }).ToList(),
                TargetFilterFunction = this.GetMappingTypeSettings().Targets().IsDataTypes() ? "return element.represents == 'data'"
                    : this.GetMappingTypeSettings().Targets().IsInvokableTypes() ? "return element.represents == 'invokable'"
                    : this.GetMappingTypeSettings().Targets().IsEventTypes() ? "return element.represents == 'event'"
                    : this.GetMappingTypeSettings().SourceTypesFilter(),
                TargetArrowFunction = this.GetMappingTypeSettings().TargetArrowType().IsSolidArrow()
                    ? "return `M ${x} ${y} l -10 5 l 0 -10 z`" : "return null",
                SyncSourceChildTypes = this.GetMappingTypeSettings().SyncSourceChildTypes()?.Select(x => new MappableElementSettingIdentifierPersistable() { Id = x.Id, Name = x.Name }).ToList(),
                Represents = Enum.TryParse<ElementMappingRepresentation>(this.GetMappingTypeSettings().Represents().Value, out var represents) ? represents : ElementMappingRepresentation.Unknown,
                LineColor = this.GetMappingTypeSettings().LineColor(),
                LineDashArray = this.GetMappingTypeSettings().LineDashArray(),
                ValidationFunction = this.GetMappingTypeSettings().ValidationFunction(),
                AllowAutoMap = true, // TODO
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