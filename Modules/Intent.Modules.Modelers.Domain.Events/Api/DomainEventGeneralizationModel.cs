using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DomainEventGeneralizationModel : IMetadataModel
    {
        public const string SpecializationType = "Domain Event Generalization";
        public const string SpecializationTypeId = "fa57ec52-536d-46a8-8aa0-4589812665c1";
        protected readonly IAssociation _association;
        protected DomainEventGeneralizationSourceEndModel _sourceEnd;
        protected DomainEventGeneralizationTargetEndModel _targetEnd;

        public DomainEventGeneralizationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static DomainEventGeneralizationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new DomainEventGeneralizationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public DomainEventGeneralizationSourceEndModel SourceEnd => _sourceEnd ??= new DomainEventGeneralizationSourceEndModel(_association.SourceEnd, this);

        public DomainEventGeneralizationTargetEndModel TargetEnd => _targetEnd ??= new DomainEventGeneralizationTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(DomainEventGeneralizationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventGeneralizationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventGeneralizationSourceEndModel : DomainEventGeneralizationEndModel
    {
        public const string SpecializationTypeId = "9d77f404-ca00-46ea-9ec8-8734274cb3f7";
        public const string SpecializationType = "Domain Event Generalization Source End";

        public DomainEventGeneralizationSourceEndModel(IAssociationEnd associationEnd, DomainEventGeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventGeneralizationTargetEndModel : DomainEventGeneralizationEndModel
    {
        public const string SpecializationTypeId = "1540252f-51ba-48e4-a7ad-f52ac32389e6";
        public const string SpecializationType = "Domain Event Generalization Target End";

        public DomainEventGeneralizationTargetEndModel(IAssociationEnd associationEnd, DomainEventGeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventGeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly DomainEventGeneralizationModel _association;

        public DomainEventGeneralizationEndModel(IAssociationEnd associationEnd, DomainEventGeneralizationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static DomainEventGeneralizationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new DomainEventGeneralizationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (DomainEventGeneralizationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public DomainEventGeneralizationModel Association => _association;
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

        public DomainEventGeneralizationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (DomainEventGeneralizationEndModel)_association.TargetEnd : (DomainEventGeneralizationEndModel)_association.SourceEnd;
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

        public bool Equals(DomainEventGeneralizationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventGeneralizationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DomainEventGeneralizationEndModelExtensions
    {
        public static bool IsDomainEventGeneralizationEndModel(this ICanBeReferencedType type)
        {
            return IsDomainEventGeneralizationTargetEndModel(type) || IsDomainEventGeneralizationSourceEndModel(type);
        }

        public static DomainEventGeneralizationEndModel AsDomainEventGeneralizationEndModel(this ICanBeReferencedType type)
        {
            return (DomainEventGeneralizationEndModel)type.AsDomainEventGeneralizationTargetEndModel() ?? (DomainEventGeneralizationEndModel)type.AsDomainEventGeneralizationSourceEndModel();
        }

        public static bool IsDomainEventGeneralizationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventGeneralizationTargetEndModel.SpecializationTypeId;
        }

        public static DomainEventGeneralizationTargetEndModel AsDomainEventGeneralizationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventGeneralizationTargetEndModel() ? new DomainEventGeneralizationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsDomainEventGeneralizationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventGeneralizationSourceEndModel.SpecializationTypeId;
        }

        public static DomainEventGeneralizationSourceEndModel AsDomainEventGeneralizationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventGeneralizationSourceEndModel() ? new DomainEventGeneralizationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}