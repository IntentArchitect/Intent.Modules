using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class StreamSubscriptionModel : IMetadataModel
    {
        public const string SpecializationType = "Stream Subscription";
        public const string SpecializationTypeId = "03b0ee2e-7696-464d-88d5-fcae17cd0c75";
        protected readonly IAssociation _association;
        protected StreamSubscriptionSourceEndModel _sourceEnd;
        protected StreamSubscriptionTargetEndModel _targetEnd;

        public StreamSubscriptionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static StreamSubscriptionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new StreamSubscriptionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public StreamSubscriptionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new StreamSubscriptionSourceEndModel(_association.SourceEnd, this));

        public StreamSubscriptionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new StreamSubscriptionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(StreamSubscriptionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StreamSubscriptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StreamSubscriptionSourceEndModel : StreamSubscriptionEndModel
    {
        public const string SpecializationTypeId = "1cb73347-6f31-4e94-9bd0-6c01fb284313";

        public StreamSubscriptionSourceEndModel(IAssociationEnd associationEnd, StreamSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StreamSubscriptionTargetEndModel : StreamSubscriptionEndModel
    {
        public const string SpecializationTypeId = "bd0b08c5-0c1b-49f6-aa5e-1b1445e7dc5d";

        public StreamSubscriptionTargetEndModel(IAssociationEnd associationEnd, StreamSubscriptionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StreamSubscriptionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly StreamSubscriptionModel _association;

        public StreamSubscriptionEndModel(IAssociationEnd associationEnd, StreamSubscriptionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static StreamSubscriptionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new StreamSubscriptionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (StreamSubscriptionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public StreamSubscriptionModel Association => _association;
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

        public StreamSubscriptionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (StreamSubscriptionEndModel)_association.TargetEnd : (StreamSubscriptionEndModel)_association.SourceEnd;
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

        public bool Equals(StreamSubscriptionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StreamSubscriptionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StreamSubscriptionEndModelExtensions
    {
        public static bool IsStreamSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return IsStreamSubscriptionTargetEndModel(type) || IsStreamSubscriptionSourceEndModel(type);
        }

        public static StreamSubscriptionEndModel AsStreamSubscriptionEndModel(this ICanBeReferencedType type)
        {
            return (StreamSubscriptionEndModel)type.AsStreamSubscriptionTargetEndModel() ?? (StreamSubscriptionEndModel)type.AsStreamSubscriptionSourceEndModel();
        }

        public static bool IsStreamSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == StreamSubscriptionTargetEndModel.SpecializationTypeId;
        }

        public static StreamSubscriptionTargetEndModel AsStreamSubscriptionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsStreamSubscriptionTargetEndModel() ? new StreamSubscriptionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsStreamSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == StreamSubscriptionSourceEndModel.SpecializationTypeId;
        }

        public static StreamSubscriptionSourceEndModel AsStreamSubscriptionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsStreamSubscriptionSourceEndModel() ? new StreamSubscriptionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}