using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DomainEventAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Domain Event Association";
        public const string SpecializationTypeId = "8014b71d-627b-4349-b6d7-5011dfa1bb09";
        protected readonly IAssociation _association;
        protected DomainEventAssociationSourceEndModel _sourceEnd;
        protected DomainEventAssociationTargetEndModel _targetEnd;

        public DomainEventAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static DomainEventAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new DomainEventAssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public DomainEventAssociationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new DomainEventAssociationSourceEndModel(_association.SourceEnd, this));

        public DomainEventAssociationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new DomainEventAssociationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(DomainEventAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventAssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventAssociationSourceEndModel : DomainEventAssociationEndModel
    {
        public const string SpecializationTypeId = "ad8998ec-ee85-4544-9267-8ec150a2f257";

        public DomainEventAssociationSourceEndModel(IAssociationEnd associationEnd, DomainEventAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventAssociationTargetEndModel : DomainEventAssociationEndModel
    {
        public const string SpecializationTypeId = "fe0d0f78-fb7e-4685-ab2d-bae88054a78e";

        public DomainEventAssociationTargetEndModel(IAssociationEnd associationEnd, DomainEventAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly DomainEventAssociationModel _association;

        public DomainEventAssociationEndModel(IAssociationEnd associationEnd, DomainEventAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static DomainEventAssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new DomainEventAssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (DomainEventAssociationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public DomainEventAssociationModel Association => _association;
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

        public DomainEventAssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (DomainEventAssociationEndModel)_association.TargetEnd : (DomainEventAssociationEndModel)_association.SourceEnd;
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

        public bool Equals(DomainEventAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventAssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DomainEventAssociationEndModelExtensions
    {
        public static bool IsDomainEventAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsDomainEventAssociationTargetEndModel(type) || IsDomainEventAssociationSourceEndModel(type);
        }

        public static DomainEventAssociationEndModel AsDomainEventAssociationEndModel(this ICanBeReferencedType type)
        {
            return (DomainEventAssociationEndModel)type.AsDomainEventAssociationTargetEndModel() ?? (DomainEventAssociationEndModel)type.AsDomainEventAssociationSourceEndModel();
        }

        public static bool IsDomainEventAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventAssociationTargetEndModel.SpecializationTypeId;
        }

        public static DomainEventAssociationTargetEndModel AsDomainEventAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventAssociationTargetEndModel() ? new DomainEventAssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsDomainEventAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventAssociationSourceEndModel.SpecializationTypeId;
        }

        public static DomainEventAssociationSourceEndModel AsDomainEventAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventAssociationSourceEndModel() ? new DomainEventAssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}