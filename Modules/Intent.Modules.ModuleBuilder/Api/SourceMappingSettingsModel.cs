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
    public class SourceMappingSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Source Mapping Settings";
        public const string SpecializationTypeId = "1536425c-35f4-48e1-abe4-1f8f56533545";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public SourceMappingSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public IList<ElementMappingSettingsModel> ElementMappings => _element.ChildElements
            .GetElementsOfType(ElementMappingSettingsModel.SpecializationTypeId)
            .Select(x => new ElementMappingSettingsModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(SourceMappingSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SourceMappingSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SourceMappingSettingsModelExtensions
    {

        public static bool IsSourceMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == SourceMappingSettingsModel.SpecializationTypeId;
        }

        public static SourceMappingSettingsModel AsSourceMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsSourceMappingSettingsModel() ? new SourceMappingSettingsModel((IElement)type) : null;
        }
    }
}