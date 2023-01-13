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
    public class EventStreamModel : IMetadataModel
    {
        public const string SpecializationType = "Event Stream";
        public const string SpecializationTypeId = "055db259-36fc-495f-935c-89c847c34c32";
        protected readonly IAssociation _association;
        protected EventStreamSourceEndModel _sourceEnd;
        protected EventStreamTargetEndModel _targetEnd;

        public EventStreamModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static EventStreamModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new EventStreamModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public EventStreamSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new EventStreamSourceEndModel(_association.SourceEnd, this));

        public EventStreamTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new EventStreamTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(EventStreamModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventStreamModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class EventStreamSourceEndModel : EventStreamEndModel
    {
        public const string SpecializationTypeId = "611be10d-2870-4c6b-b05f-e2ef5b62c03b";

        public EventStreamSourceEndModel(IAssociationEnd associationEnd, EventStreamModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class EventStreamTargetEndModel : EventStreamEndModel
    {
        public const string SpecializationTypeId = "c6d6f8dc-2b4c-42d5-a34c-383c0b207d0d";

        public EventStreamTargetEndModel(IAssociationEnd associationEnd, EventStreamModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class EventStreamEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly EventStreamModel _association;

        public EventStreamEndModel(IAssociationEnd associationEnd, EventStreamModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static EventStreamEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new EventStreamModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (EventStreamEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public EventStreamModel Association => _association;
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

        public EventStreamEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (EventStreamEndModel)_association.TargetEnd : (EventStreamEndModel)_association.SourceEnd;
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

        public bool Equals(EventStreamEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventStreamEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class EventStreamEndModelExtensions
    {
        public static bool IsEventStreamEndModel(this ICanBeReferencedType type)
        {
            return IsEventStreamTargetEndModel(type) || IsEventStreamSourceEndModel(type);
        }

        public static EventStreamEndModel AsEventStreamEndModel(this ICanBeReferencedType type)
        {
            return (EventStreamEndModel)type.AsEventStreamTargetEndModel() ?? (EventStreamEndModel)type.AsEventStreamSourceEndModel();
        }

        public static bool IsEventStreamTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == EventStreamTargetEndModel.SpecializationTypeId;
        }

        public static EventStreamTargetEndModel AsEventStreamTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsEventStreamTargetEndModel() ? new EventStreamModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsEventStreamSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == EventStreamSourceEndModel.SpecializationTypeId;
        }

        public static EventStreamSourceEndModel AsEventStreamSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsEventStreamSourceEndModel() ? new EventStreamModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}