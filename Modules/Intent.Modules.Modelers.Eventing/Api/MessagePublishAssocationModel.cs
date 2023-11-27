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
    public class MessagePublishAssocationModel : IMetadataModel
    {
        public const string SpecializationType = "Message Publish Assocation";
        public const string SpecializationTypeId = "022d4c90-e1b6-4747-a15f-640c19503a8f";
        protected readonly IAssociation _association;
        protected MessagePublishAssocationSourceEndModel _sourceEnd;
        protected MessagePublishAssocationTargetEndModel _targetEnd;

        public MessagePublishAssocationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static MessagePublishAssocationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new MessagePublishAssocationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public MessagePublishAssocationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new MessagePublishAssocationSourceEndModel(_association.SourceEnd, this));

        public MessagePublishAssocationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new MessagePublishAssocationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(MessagePublishAssocationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessagePublishAssocationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class MessagePublishAssocationSourceEndModel : MessagePublishAssocationEndModel
    {
        public const string SpecializationTypeId = "282eea12-2b98-4f0f-a28c-50c05b953c8e";

        public MessagePublishAssocationSourceEndModel(IAssociationEnd associationEnd, MessagePublishAssocationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class MessagePublishAssocationTargetEndModel : MessagePublishAssocationEndModel
    {
        public const string SpecializationTypeId = "6be8d569-70b9-4451-bd1b-7b654499503e";

        public MessagePublishAssocationTargetEndModel(IAssociationEnd associationEnd, MessagePublishAssocationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class MessagePublishAssocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly MessagePublishAssocationModel _association;

        public MessagePublishAssocationEndModel(IAssociationEnd associationEnd, MessagePublishAssocationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static MessagePublishAssocationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new MessagePublishAssocationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (MessagePublishAssocationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public MessagePublishAssocationModel Association => _association;
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

        public MessagePublishAssocationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (MessagePublishAssocationEndModel)_association.TargetEnd : (MessagePublishAssocationEndModel)_association.SourceEnd;
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

        public bool Equals(MessagePublishAssocationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessagePublishAssocationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MessagePublishAssocationEndModelExtensions
    {
        public static bool IsMessagePublishAssocationEndModel(this ICanBeReferencedType type)
        {
            return IsMessagePublishAssocationTargetEndModel(type) || IsMessagePublishAssocationSourceEndModel(type);
        }

        public static MessagePublishAssocationEndModel AsMessagePublishAssocationEndModel(this ICanBeReferencedType type)
        {
            return (MessagePublishAssocationEndModel)type.AsMessagePublishAssocationTargetEndModel() ?? (MessagePublishAssocationEndModel)type.AsMessagePublishAssocationSourceEndModel();
        }

        public static bool IsMessagePublishAssocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == MessagePublishAssocationTargetEndModel.SpecializationTypeId;
        }

        public static MessagePublishAssocationTargetEndModel AsMessagePublishAssocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsMessagePublishAssocationTargetEndModel() ? new MessagePublishAssocationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsMessagePublishAssocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == MessagePublishAssocationSourceEndModel.SpecializationTypeId;
        }

        public static MessagePublishAssocationSourceEndModel AsMessagePublishAssocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsMessagePublishAssocationSourceEndModel() ? new MessagePublishAssocationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}