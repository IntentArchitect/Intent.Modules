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
    public class MappableElementSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Mappable Element Settings";
        public const string SpecializationTypeId = "62e5b1a9-0d36-4969-9d22-ce748155afbf";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappableElementSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TargetType => TypeReference?.Element != null ? TypeReference : null;

        public IElement InternalElement => _element;

        public IList<MappableElementSettingsModel> ElementMappings => _element.ChildElements
            .GetElementsOfType(MappableElementSettingsModel.SpecializationTypeId)
            .Select(x => new MappableElementSettingsModel(x))
            .ToList();

        public IList<StaticMappableSettingsModel> StaticMappings => _element.ChildElements
            .GetElementsOfType(StaticMappableSettingsModel.SpecializationTypeId)
            .Select(x => new StaticMappableSettingsModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappableElementSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappableElementSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Ignore)]
        public MappableElementSettingPersistable ToPersistable()
        {
            return new MappableElementSettingPersistable()
            {
                Version = "2.0.0",
                Id = Id,
                Name = Name,
                SpecializationType = TargetType.Element.Name,
                SpecializationTypeId = TargetType.Element.Id,
                Represents = this.GetMappableSettings().Represents().Value.ToLowerInvariant(),
                IconOverride = this.GetMappableSettings().IconOverride().ToPersistable(),
                FilterFunction = this.GetMappableSettings().FilterFunction(),
                IsMappableFunction = this.GetMappableSettings().IsMappableFunction(),
                AllowMultipleMappings = this.GetMappableSettings().AllowMultipleMappings(),
                IsRequiredFunction = this.GetMappableSettings().IsRequiredFunction(),
                IsTraversable = !this.GetMappableSettings().TraversableMode().IsNotTraversable(),
                TraversableTypes = this.GetMappableSettings().TraversableTypes().Select(x => x.Id).ToList(),
                GetTraversableTypeFunction = this.GetMappableSettings().GetTraversableTypeFunction(),
                UseChildSettingsFrom = this.GetMappableSettings().UseChildMappingsFrom()?.Id,
                ChildSettings = ElementMappings.Select(x => x.ToPersistable()).ToList(),
                CanBeModified = this.GetMappableSettings().CanBeModified(),
                CreateNameFunction = this.GetMappableSettings().CreateNameFunction(),
                SyncWith = this.GetMappableSettings().SyncMappingTo().IsElementValue() ? MappableElementSettingPersistable.SyncWithValue
                    : this.GetMappableSettings().SyncMappingTo().IsStereotypePropertyValue() ? MappableElementSettingPersistable.SyncWithStereotypeProperty
                    : null,
                SyncStereotypeId = this.GetMappableSettings().SyncStereotype()?.Id,
                SyncStereotypePropertyId = this.GetMappableSettings().SyncStereotypeProperty(),
                StaticMappableSettings = StaticMappings.Select(x => x.ToPersistable()).ToList(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappableElementSettingsModelExtensions
    {

        public static bool IsMappableElementSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappableElementSettingsModel.SpecializationTypeId;
        }

        public static MappableElementSettingsModel AsMappableElementSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsMappableElementSettingsModel() ? new MappableElementSettingsModel((IElement)type) : null;
        }
    }
}