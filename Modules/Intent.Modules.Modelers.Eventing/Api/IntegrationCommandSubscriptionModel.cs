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
    public class IntegrationCommandSubscriptionModel : IMetadataModel
    {
        public const string SpecializationType = "Integration Command Subscription";
        public const string SpecializationTypeId = "9073c828-0127-448e-be24-f2ebfc4d3238";
        protected readonly IAssociation _association;
        protected IntegrationCommandSubscriptionSourceEndModel _sourceEnd;
        protected IntegrationCommandSubscriptionTargetEndModel _targetEnd;

        public IntegrationCommandSubscriptionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static IntegrationCommandSubscriptionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new IntegrationCommandSubscriptionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public IntegrationCommandSubscriptionSourceEndModel SourceEnd => _sourceEnd ??= new IntegrationCommandSubscriptionSourceEndModel(_association.SourceEnd, this);

        public IntegrationCommandSubscriptionTargetEndModel TargetEnd => _targetEnd ??= new IntegrationCommandSubscriptionTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(IntegrationCommandSubscriptionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegrationCommandSubscriptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class IntegrationCommandSubscriptionSourceEndModel : IntegrationCommandSubscriptionEndModel
    {
        public const string SpecializationTypeId = "ed77d7e5-6000-4653-a278-992c7627a459";
        public const string SpecializationType = "Integration Command Subscription Source End";

        public IntegrationCommandSubscriptionSourceEndModel(IAssociationEnd associationEnd, IntegrationCommandSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class IntegrationCommandSubscriptionTargetEndModel : IntegrationCommandSubscriptionEndModel
    {
        public const string SpecializationTypeId = "855b4897-bf70-4b03-87f4-ada35969c410";
        public const string SpecializationType = "Integration Command Subscription Target End";

        public IntegrationCommandSubscriptionTargetEndModel(IAssociationEnd associationEnd, IntegrationCommandSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class IntegrationCommandSubscriptionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly IntegrationCommandSubscriptionModel _association;

        public IntegrationCommandSubscriptionEndModel(IAssociationEnd associationEnd, IntegrationCommandSubscriptionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static IntegrationCommandSubscriptionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new IntegrationCommandSubscriptionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (IntegrationCommandSubscriptionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public IntegrationCommandSubscriptionModel Association => _association;
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

        public IntegrationCommandSubscriptionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (IntegrationCommandSubscriptionEndModel)_association.TargetEnd : (IntegrationCommandSubscriptionEndModel)_association.SourceEnd;
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

        public bool Equals(IntegrationCommandSubscriptionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegrationCommandSubscriptionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IntegrationCommandSubscriptionEndModelExtensions
    {
        public static bool IsIntegrationCommandSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return IsIntegrationCommandSubscriptionTargetEndModel(type) || IsIntegrationCommandSubscriptionSourceEndModel(type);
        }

        public static IntegrationCommandSubscriptionEndModel AsIntegrationCommandSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return (IntegrationCommandSubscriptionEndModel)type.AsIntegrationCommandSubscriptionTargetEndModel() ?? (IntegrationCommandSubscriptionEndModel)type.AsIntegrationCommandSubscriptionSourceEndModel();
        }

        public static bool IsIntegrationCommandSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == IntegrationCommandSubscriptionTargetEndModel.SpecializationTypeId;
        }

        public static IntegrationCommandSubscriptionTargetEndModel AsIntegrationCommandSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsIntegrationCommandSubscriptionTargetEndModel() ? new IntegrationCommandSubscriptionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsIntegrationCommandSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == IntegrationCommandSubscriptionSourceEndModel.SpecializationTypeId;
        }

        public static IntegrationCommandSubscriptionSourceEndModel AsIntegrationCommandSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsIntegrationCommandSubscriptionSourceEndModel() ? new IntegrationCommandSubscriptionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}