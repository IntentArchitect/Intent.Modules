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
    public class UniqueConstraintColumnModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Unique Constraint Column";
        public const string SpecializationTypeId = "d32bc804-8f5e-463f-9cc5-d3b8f3536402";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public UniqueConstraintColumnModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(UniqueConstraintColumnModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UniqueConstraintColumnModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class UniqueConstraintColumnModelExtensions
    {

        public static bool IsUniqueConstraintColumnModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == UniqueConstraintColumnModel.SpecializationTypeId;
        }

        public static UniqueConstraintColumnModel AsUniqueConstraintColumnModel(this ICanBeReferencedType type)
        {
            return type.IsUniqueConstraintColumnModel() ? new UniqueConstraintColumnModel((IElement)type) : null;
        }
    }
}