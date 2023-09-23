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
    public class ElementMappingSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "Element Mapping Settings";
        public const string SpecializationTypeId = "62e5b1a9-0d36-4969-9d22-ce748155afbf";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ElementMappingSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public IList<ElementMappingSettingsModel> ElementMappings => _element.ChildElements
            .GetElementsOfType(ElementMappingSettingsModel.SpecializationTypeId)
            .Select(x => new ElementMappingSettingsModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ElementMappingSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementMappingSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Ignore)]
        public MappingElementSettingPersistable ToPersistable()
        {
            return new MappingElementSettingPersistable()
            {
                Id = Id,
                Name = Name,
                SpecializationType = TargetType.Element.Name,
                SpecializationTypeId = TargetType.Element.Id,
                Represents = Enum.TryParse<ElementMappingRepresentation>(this.GetMappingSettings().Represents().Value, out var represents) ? represents : ElementMappingRepresentation.Unknown,
                FilterFunction = this.GetMappingSettings().FilterFunction(),
                IsMappableFunction = this.GetMappingSettings().IsMappableFunction(),
                AllowMultipleMappings = this.GetMappingSettings().AllowMultipleMappings(),
                IsRequiredFunction = this.GetMappingSettings().IsRequiredFunction(),
                TraversableTypes = this.GetMappingSettings().TraversableTypes().Select(x => x.Id).ToList(),
                IsTraversableFunction = this.GetMappingSettings().IsTraversableFunction(),
                MapsTo = this.GetMappingSettings().MapsTo().Select(x => x.Id).ToList(),
                UseChildSettingsFrom = this.GetMappingSettings().UseChildMappingsFrom()?.Id,
                ChildSettings = ElementMappings.Select(x => x.ToPersistable()).ToList(),
                CanBeModified = this.GetMappingSettings().CanBeModified(),
                CreateNameFunction = this.GetMappingSettings().CreateNameFunction(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ElementMappingSettingsModelExtensions
    {

        public static bool IsElementMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ElementMappingSettingsModel.SpecializationTypeId;
        }

        public static ElementMappingSettingsModel AsElementMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsElementMappingSettingsModel() ? new ElementMappingSettingsModel((IElement)type) : null;
        }
    }
}