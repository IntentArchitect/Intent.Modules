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
    public class TargetMappingSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Target Mapping Settings";
        public const string SpecializationTypeId = "a812bff7-017a-4692-8ec7-1caad0a1dd88";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public TargetMappingSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(TargetMappingSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TargetMappingSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class TargetMappingSettingsModelExtensions
    {

        public static bool IsTargetMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == TargetMappingSettingsModel.SpecializationTypeId;
        }

        public static TargetMappingSettingsModel AsTargetMappingSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsTargetMappingSettingsModel() ? new TargetMappingSettingsModel((IElement)type) : null;
        }
    }
}