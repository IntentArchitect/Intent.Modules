using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DomainEventOriginAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Domain Event Origin Association";
        public const string SpecializationTypeId = "4c0cc50b-8a9d-43cd-b731-9f354f69f3c9";
        protected readonly IAssociation _association;
        protected DomainEventOriginAssociationSourceEndModel _sourceEnd;
        protected DomainEventOriginAssociationTargetEndModel _targetEnd;

        public DomainEventOriginAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static DomainEventOriginAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new DomainEventOriginAssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public DomainEventOriginAssociationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new DomainEventOriginAssociationSourceEndModel(_association.SourceEnd, this));

        public DomainEventOriginAssociationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new DomainEventOriginAssociationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(DomainEventOriginAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventOriginAssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventOriginAssociationSourceEndModel : DomainEventOriginAssociationEndModel
    {
        public const string SpecializationTypeId = "2495d3ef-f7a2-441d-b749-b51b2546b45e";

        public DomainEventOriginAssociationSourceEndModel(IAssociationEnd associationEnd, DomainEventOriginAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventOriginAssociationTargetEndModel : DomainEventOriginAssociationEndModel
    {
        public const string SpecializationTypeId = "17046427-14e2-4081-8463-ef16c0fda399";

        public DomainEventOriginAssociationTargetEndModel(IAssociationEnd associationEnd, DomainEventOriginAssociationModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventOriginAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly DomainEventOriginAssociationModel _association;

        public DomainEventOriginAssociationEndModel(IAssociationEnd associationEnd, DomainEventOriginAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static DomainEventOriginAssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new DomainEventOriginAssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (DomainEventOriginAssociationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public DomainEventOriginAssociationModel Association => _association;
        public IElement InternalElement => _associationEnd;
        public IAssociationEnd InternalAssociationEnd => _associationEnd;
        public IAssociation InternalAssociation => _association.InternalAssociation;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.TypeReference.IsNullable;
        public bool IsCollection => _associationEnd.TypeReference.IsCollection;
        public ICanBeReferencedType Element => _associationEnd.TypeReference.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.TypeReference.GenericTypeParameters;
        public ITypeReference TypeReference => this;
        public IPackage Package => Element?.Package;
        public string Comment => _associationEnd.Comment;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public DomainEventOriginAssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (DomainEventOriginAssociationEndModel)_association.TargetEnd : (DomainEventOriginAssociationEndModel)_association.SourceEnd;
        }

        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        public override string ToString()
        {
            return _associationEnd.ToString();
        }

        public bool Equals(DomainEventOriginAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventOriginAssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DomainEventOriginAssociationEndModelExtensions
    {
        public static bool IsDomainEventOriginAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsDomainEventOriginAssociationTargetEndModel(type) || IsDomainEventOriginAssociationSourceEndModel(type);
        }

        public static DomainEventOriginAssociationEndModel AsDomainEventOriginAssociationEndModel(this ICanBeReferencedType type)
        {
            return (DomainEventOriginAssociationEndModel)type.AsDomainEventOriginAssociationTargetEndModel() ?? (DomainEventOriginAssociationEndModel)type.AsDomainEventOriginAssociationSourceEndModel();
        }

        public static bool IsDomainEventOriginAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventOriginAssociationTargetEndModel.SpecializationTypeId;
        }

        public static DomainEventOriginAssociationTargetEndModel AsDomainEventOriginAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventOriginAssociationTargetEndModel() ? new DomainEventOriginAssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsDomainEventOriginAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventOriginAssociationSourceEndModel.SpecializationTypeId;
        }

        public static DomainEventOriginAssociationSourceEndModel AsDomainEventOriginAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventOriginAssociationSourceEndModel() ? new DomainEventOriginAssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}