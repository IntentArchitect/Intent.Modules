using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class StateTransitionModel : IMetadataModel
    {
        public const string SpecializationType = "State Transition";
        public const string SpecializationTypeId = "995e8812-4510-45b4-9a24-8ad57f01dea9";
        protected readonly IAssociation _association;
        protected StateTransitionSourceEndModel _sourceEnd;
        protected StateTransitionTargetEndModel _targetEnd;

        public StateTransitionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static StateTransitionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new StateTransitionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public StateTransitionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new StateTransitionSourceEndModel(_association.SourceEnd, this));

        public StateTransitionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new StateTransitionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(StateTransitionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StateTransitionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StateTransitionSourceEndModel : StateTransitionEndModel
    {
        public const string SpecializationTypeId = "f636a8d9-bd1a-4689-aaf7-456d2523c2c6";

        public StateTransitionSourceEndModel(IAssociationEnd associationEnd, StateTransitionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StateTransitionTargetEndModel : StateTransitionEndModel
    {
        public const string SpecializationTypeId = "e25951f6-5052-408f-9fa7-a3f9024cb387";

        public StateTransitionTargetEndModel(IAssociationEnd associationEnd, StateTransitionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StateTransitionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly StateTransitionModel _association;

        public StateTransitionEndModel(IAssociationEnd associationEnd, StateTransitionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static StateTransitionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new StateTransitionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (StateTransitionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public StateTransitionModel Association => _association;
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

        public StateTransitionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (StateTransitionEndModel)_association.TargetEnd : (StateTransitionEndModel)_association.SourceEnd;
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

        public bool Equals(StateTransitionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StateTransitionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StateTransitionEndModelExtensions
    {
        public static bool IsStateTransitionEndModel(this ICanBeReferencedType type)
        {
            return IsStateTransitionTargetEndModel(type) || IsStateTransitionSourceEndModel(type);
        }

        public static StateTransitionEndModel AsStateTransitionEndModel(this ICanBeReferencedType type)
        {
            return (StateTransitionEndModel)type.AsStateTransitionTargetEndModel() ?? (StateTransitionEndModel)type.AsStateTransitionSourceEndModel();
        }

        public static bool IsStateTransitionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == StateTransitionTargetEndModel.SpecializationTypeId;
        }

        public static StateTransitionTargetEndModel AsStateTransitionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsStateTransitionTargetEndModel() ? new StateTransitionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsStateTransitionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == StateTransitionSourceEndModel.SpecializationTypeId;
        }

        public static StateTransitionSourceEndModel AsStateTransitionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsStateTransitionSourceEndModel() ? new StateTransitionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}