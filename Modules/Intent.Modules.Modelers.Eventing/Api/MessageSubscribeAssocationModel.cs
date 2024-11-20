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
    public class MessageSubscribeAssocationModel : IMetadataModel
    {
        public const string SpecializationType = "Message Subscribe Assocation";
        public const string SpecializationTypeId = "50e0bed1-1387-4d67-8f66-1194763296b1";
        protected readonly IAssociation _association;
        protected MessageSubscribeAssocationSourceEndModel _sourceEnd;
        protected MessageSubscribeAssocationTargetEndModel _targetEnd;

        public MessageSubscribeAssocationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static MessageSubscribeAssocationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new MessageSubscribeAssocationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public MessageSubscribeAssocationSourceEndModel SourceEnd => _sourceEnd ??= new MessageSubscribeAssocationSourceEndModel(_association.SourceEnd, this);

        public MessageSubscribeAssocationTargetEndModel TargetEnd => _targetEnd ??= new MessageSubscribeAssocationTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(MessageSubscribeAssocationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessageSubscribeAssocationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class MessageSubscribeAssocationSourceEndModel : MessageSubscribeAssocationEndModel
    {
        public const string SpecializationTypeId = "18780798-1ea8-462b-b4f9-b8605ad11636";
        public const string SpecializationType = "Message Subscribe Assocation Source End";

        public MessageSubscribeAssocationSourceEndModel(IAssociationEnd associationEnd, MessageSubscribeAssocationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class MessageSubscribeAssocationTargetEndModel : MessageSubscribeAssocationEndModel
    {
        public const string SpecializationTypeId = "21c3df1c-5140-4f65-9814-a68e1a577768";
        public const string SpecializationType = "Message Subscribe Assocation Target End";

        public MessageSubscribeAssocationTargetEndModel(IAssociationEnd associationEnd, MessageSubscribeAssocationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class MessageSubscribeAssocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly MessageSubscribeAssocationModel _association;

        public MessageSubscribeAssocationEndModel(IAssociationEnd associationEnd, MessageSubscribeAssocationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static MessageSubscribeAssocationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new MessageSubscribeAssocationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (MessageSubscribeAssocationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public MessageSubscribeAssocationModel Association => _association;
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

        public MessageSubscribeAssocationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (MessageSubscribeAssocationEndModel)_association.TargetEnd : (MessageSubscribeAssocationEndModel)_association.SourceEnd;
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

        public bool Equals(MessageSubscribeAssocationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessageSubscribeAssocationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MessageSubscribeAssocationEndModelExtensions
    {
        public static bool IsMessageSubscribeAssocationEndModel(this ICanBeReferencedType type)
        {
            return IsMessageSubscribeAssocationTargetEndModel(type) || IsMessageSubscribeAssocationSourceEndModel(type);
        }

        public static MessageSubscribeAssocationEndModel AsMessageSubscribeAssocationEndModel(this ICanBeReferencedType type)
        {
            return (MessageSubscribeAssocationEndModel)type.AsMessageSubscribeAssocationTargetEndModel() ?? (MessageSubscribeAssocationEndModel)type.AsMessageSubscribeAssocationSourceEndModel();
        }

        public static bool IsMessageSubscribeAssocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == MessageSubscribeAssocationTargetEndModel.SpecializationTypeId;
        }

        public static MessageSubscribeAssocationTargetEndModel AsMessageSubscribeAssocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsMessageSubscribeAssocationTargetEndModel() ? new MessageSubscribeAssocationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsMessageSubscribeAssocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == MessageSubscribeAssocationSourceEndModel.SpecializationTypeId;
        }

        public static MessageSubscribeAssocationSourceEndModel AsMessageSubscribeAssocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsMessageSubscribeAssocationSourceEndModel() ? new MessageSubscribeAssocationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}