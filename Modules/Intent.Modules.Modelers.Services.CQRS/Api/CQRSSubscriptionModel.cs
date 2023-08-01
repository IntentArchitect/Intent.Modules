using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CQRSSubscriptionModel : IMetadataModel
    {
        public const string SpecializationType = "CQRS Subscription";
        public const string SpecializationTypeId = "70bf1bcd-bdcd-4f85-a975-4b266a1bdc0c";
        protected readonly IAssociation _association;
        protected CQRSSubscriptionSourceEndModel _sourceEnd;
        protected CQRSSubscriptionTargetEndModel _targetEnd;

        public CQRSSubscriptionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CQRSSubscriptionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CQRSSubscriptionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CQRSSubscriptionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CQRSSubscriptionSourceEndModel(_association.SourceEnd, this));

        public CQRSSubscriptionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CQRSSubscriptionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CQRSSubscriptionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CQRSSubscriptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CQRSSubscriptionSourceEndModel : CQRSSubscriptionEndModel
    {
        public const string SpecializationTypeId = "bfb23910-6ffb-436a-870e-3aa7914f4697";

        public CQRSSubscriptionSourceEndModel(IAssociationEnd associationEnd, CQRSSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CQRSSubscriptionTargetEndModel : CQRSSubscriptionEndModel
    {
        public const string SpecializationTypeId = "5ae54b6a-3c15-4e33-9e24-3c9dfd177ae2";

        public CQRSSubscriptionTargetEndModel(IAssociationEnd associationEnd, CQRSSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CQRSSubscriptionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CQRSSubscriptionModel _association;

        public CQRSSubscriptionEndModel(IAssociationEnd associationEnd, CQRSSubscriptionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CQRSSubscriptionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CQRSSubscriptionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CQRSSubscriptionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CQRSSubscriptionModel Association => _association;
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

        public CQRSSubscriptionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CQRSSubscriptionEndModel)_association.TargetEnd : (CQRSSubscriptionEndModel)_association.SourceEnd;
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

        public bool Equals(CQRSSubscriptionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CQRSSubscriptionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CQRSSubscriptionEndModelExtensions
    {
        public static bool IsCQRSSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return IsCQRSSubscriptionTargetEndModel(type) || IsCQRSSubscriptionSourceEndModel(type);
        }

        public static CQRSSubscriptionEndModel AsCQRSSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return (CQRSSubscriptionEndModel)type.AsCQRSSubscriptionTargetEndModel() ?? (CQRSSubscriptionEndModel)type.AsCQRSSubscriptionSourceEndModel();
        }

        public static bool IsCQRSSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CQRSSubscriptionTargetEndModel.SpecializationTypeId;
        }

        public static CQRSSubscriptionTargetEndModel AsCQRSSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCQRSSubscriptionTargetEndModel() ? new CQRSSubscriptionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCQRSSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CQRSSubscriptionSourceEndModel.SpecializationTypeId;
        }

        public static CQRSSubscriptionSourceEndModel AsCQRSSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCQRSSubscriptionSourceEndModel() ? new CQRSSubscriptionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}