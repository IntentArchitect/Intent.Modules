using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class SQSSubscriptionModel : IMetadataModel
    {
        public const string SpecializationType = "SQS Subscription";
        public const string SpecializationTypeId = "23bb08e4-8783-46d5-8d3d-ef756d19b664";
        protected readonly IAssociation _association;
        protected SQSSubscriptionSourceEndModel _sourceEnd;
        protected SQSSubscriptionTargetEndModel _targetEnd;

        public SQSSubscriptionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static SQSSubscriptionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new SQSSubscriptionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public SQSSubscriptionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new SQSSubscriptionSourceEndModel(_association.SourceEnd, this));

        public SQSSubscriptionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new SQSSubscriptionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(SQSSubscriptionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SQSSubscriptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SQSSubscriptionSourceEndModel : SQSSubscriptionEndModel
    {
        public const string SpecializationTypeId = "d7efefb5-2bff-4aa8-8f01-dc8f0573e944";

        public SQSSubscriptionSourceEndModel(IAssociationEnd associationEnd, SQSSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SQSSubscriptionTargetEndModel : SQSSubscriptionEndModel
    {
        public const string SpecializationTypeId = "9b736ebc-c539-482d-b5ac-98d1783526bf";

        public SQSSubscriptionTargetEndModel(IAssociationEnd associationEnd, SQSSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SQSSubscriptionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly SQSSubscriptionModel _association;

        public SQSSubscriptionEndModel(IAssociationEnd associationEnd, SQSSubscriptionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static SQSSubscriptionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new SQSSubscriptionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (SQSSubscriptionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public SQSSubscriptionModel Association => _association;
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

        public SQSSubscriptionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (SQSSubscriptionEndModel)_association.TargetEnd : (SQSSubscriptionEndModel)_association.SourceEnd;
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

        public bool Equals(SQSSubscriptionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SQSSubscriptionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SQSSubscriptionEndModelExtensions
    {
        public static bool IsSQSSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return IsSQSSubscriptionTargetEndModel(type) || IsSQSSubscriptionSourceEndModel(type);
        }

        public static SQSSubscriptionEndModel AsSQSSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return (SQSSubscriptionEndModel)type.AsSQSSubscriptionTargetEndModel() ?? (SQSSubscriptionEndModel)type.AsSQSSubscriptionSourceEndModel();
        }

        public static bool IsSQSSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SQSSubscriptionTargetEndModel.SpecializationTypeId;
        }

        public static SQSSubscriptionTargetEndModel AsSQSSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsSQSSubscriptionTargetEndModel() ? new SQSSubscriptionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsSQSSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SQSSubscriptionSourceEndModel.SpecializationTypeId;
        }

        public static SQSSubscriptionSourceEndModel AsSQSSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsSQSSubscriptionSourceEndModel() ? new SQSSubscriptionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}