using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    [IntentManaged(Mode.Merge)]
    public class UniqueConstraintModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Unique Constraint";
        public const string SpecializationTypeId = "c760d0f5-d8a1-4c16-9e2b-e654c2860154";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public UniqueConstraintModel(IElement element, string requiredType = SpecializationType)
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

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

        public IList<UniqueConstraintColumnModel> Columns => _element.ChildElements
            .GetElementsOfType(UniqueConstraintColumnModel.SpecializationTypeId)
            .Select(x => new UniqueConstraintColumnModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(UniqueConstraintModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UniqueConstraintModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class UniqueConstraintModelExtensions
    {

        public static bool IsUniqueConstraintModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == UniqueConstraintModel.SpecializationTypeId;
        }

        public static UniqueConstraintModel AsUniqueConstraintModel(this ICanBeReferencedType type)
        {
            return type.IsUniqueConstraintModel() ? new UniqueConstraintModel((IElement)type) : null;
        }

        public static bool HasNewMappingSettingsMapping(this UniqueConstraintModel type)
        {
            return type.Mapping?.MappingSettingsId == "43488ce4-4542-4d85-9939-8b0db04cf744";
        }

        public static IElementMapping GetNewMappingSettingsMapping(this UniqueConstraintModel type)
        {
            return type.HasNewMappingSettingsMapping() ? type.Mapping : null;
        }
    }
}