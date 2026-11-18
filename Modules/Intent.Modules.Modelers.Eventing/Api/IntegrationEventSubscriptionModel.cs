using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class IntegrationEventSubscriptionModel : IMetadataModel
    {
        public const string SpecializationType = "Integration Event Subscription";
        public const string SpecializationTypeId = "75e953ed-9a87-47d5-9d45-cdca4232be54";
        protected readonly IAssociation _association;
        protected IntegrationEventSubscriptionSourceEndModel _sourceEnd;
        protected IntegrationEventSubscriptionTargetEndModel _targetEnd;

        public IntegrationEventSubscriptionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static IntegrationEventSubscriptionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new IntegrationEventSubscriptionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public IntegrationEventSubscriptionSourceEndModel SourceEnd => _sourceEnd ??= new IntegrationEventSubscriptionSourceEndModel(_association.SourceEnd, this);

        public IntegrationEventSubscriptionTargetEndModel TargetEnd => _targetEnd ??= new IntegrationEventSubscriptionTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(IntegrationEventSubscriptionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegrationEventSubscriptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class IntegrationEventSubscriptionSourceEndModel : IntegrationEventSubscriptionEndModel
    {
        public const string SpecializationTypeId = "0d828a77-f460-4d03-a6fd-63d69ede80b1";
        public const string SpecializationType = "Integration Event Subscription Source End";

        public IntegrationEventSubscriptionSourceEndModel(IAssociationEnd associationEnd, IntegrationEventSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class IntegrationEventSubscriptionTargetEndModel : IntegrationEventSubscriptionEndModel
    {
        public const string SpecializationTypeId = "cb3c248d-989c-4bc1-9f89-a772ee5f4746";
        public const string SpecializationType = "Integration Event Subscription Target End";

        public IntegrationEventSubscriptionTargetEndModel(IAssociationEnd associationEnd, IntegrationEventSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class IntegrationEventSubscriptionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly IntegrationEventSubscriptionModel _association;

        public IntegrationEventSubscriptionEndModel(IAssociationEnd associationEnd, IntegrationEventSubscriptionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static IntegrationEventSubscriptionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new IntegrationEventSubscriptionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (IntegrationEventSubscriptionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public IntegrationEventSubscriptionModel Association => _association;
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

        public IntegrationEventSubscriptionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (IntegrationEventSubscriptionEndModel)_association.TargetEnd : (IntegrationEventSubscriptionEndModel)_association.SourceEnd;
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

        public bool Equals(IntegrationEventSubscriptionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegrationEventSubscriptionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IntegrationEventSubscriptionEndModelExtensions
    {
        public static bool IsIntegrationEventSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return IsIntegrationEventSubscriptionTargetEndModel(type) || IsIntegrationEventSubscriptionSourceEndModel(type);
        }

        public static IntegrationEventSubscriptionEndModel AsIntegrationEventSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return (IntegrationEventSubscriptionEndModel)type.AsIntegrationEventSubscriptionTargetEndModel() ?? (IntegrationEventSubscriptionEndModel)type.AsIntegrationEventSubscriptionSourceEndModel();
        }

        public static bool IsIntegrationEventSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == IntegrationEventSubscriptionTargetEndModel.SpecializationTypeId;
        }

        public static IntegrationEventSubscriptionTargetEndModel AsIntegrationEventSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsIntegrationEventSubscriptionTargetEndModel() ? new IntegrationEventSubscriptionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsIntegrationEventSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == IntegrationEventSubscriptionSourceEndModel.SpecializationTypeId;
        }

        public static IntegrationEventSubscriptionSourceEndModel AsIntegrationEventSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsIntegrationEventSubscriptionSourceEndModel() ? new IntegrationEventSubscriptionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}