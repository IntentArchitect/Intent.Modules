using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class PublishIntegrationEventModel : IMetadataModel
    {
        public const string SpecializationType = "Publish Integration Event";
        public const string SpecializationTypeId = "580b6b26-eab5-4602-a408-e76e2d292d2c";
        protected readonly IAssociation _association;
        protected PublishIntegrationEventSourceEndModel _sourceEnd;
        protected PublishIntegrationEventTargetEndModel _targetEnd;

        public PublishIntegrationEventModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static PublishIntegrationEventModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new PublishIntegrationEventModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public PublishIntegrationEventSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new PublishIntegrationEventSourceEndModel(_association.SourceEnd, this));

        public PublishIntegrationEventTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new PublishIntegrationEventTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(PublishIntegrationEventModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PublishIntegrationEventModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class PublishIntegrationEventSourceEndModel : PublishIntegrationEventEndModel
    {
        public const string SpecializationTypeId = "eab91b3a-9903-40a2-90e8-ddb714883eab";

        public PublishIntegrationEventSourceEndModel(IAssociationEnd associationEnd, PublishIntegrationEventModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class PublishIntegrationEventTargetEndModel : PublishIntegrationEventEndModel
    {
        public const string SpecializationTypeId = "6feb1511-849a-4aa3-85eb-d0c736ac1fec";

        public PublishIntegrationEventTargetEndModel(IAssociationEnd associationEnd, PublishIntegrationEventModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class PublishIntegrationEventEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly PublishIntegrationEventModel _association;

        public PublishIntegrationEventEndModel(IAssociationEnd associationEnd, PublishIntegrationEventModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static PublishIntegrationEventEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new PublishIntegrationEventModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (PublishIntegrationEventEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public PublishIntegrationEventModel Association => _association;
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

        public PublishIntegrationEventEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (PublishIntegrationEventEndModel)_association.TargetEnd : (PublishIntegrationEventEndModel)_association.SourceEnd;
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

        public bool Equals(PublishIntegrationEventEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PublishIntegrationEventEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class PublishIntegrationEventEndModelExtensions
    {
        public static bool IsPublishIntegrationEventEndModel(this ICanBeReferencedType type)
        {
            return IsPublishIntegrationEventTargetEndModel(type) || IsPublishIntegrationEventSourceEndModel(type);
        }

        public static PublishIntegrationEventEndModel AsPublishIntegrationEventEndModel(this ICanBeReferencedType type)
        {
            return (PublishIntegrationEventEndModel)type.AsPublishIntegrationEventTargetEndModel() ?? (PublishIntegrationEventEndModel)type.AsPublishIntegrationEventSourceEndModel();
        }

        public static bool IsPublishIntegrationEventTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == PublishIntegrationEventTargetEndModel.SpecializationTypeId;
        }

        public static PublishIntegrationEventTargetEndModel AsPublishIntegrationEventTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsPublishIntegrationEventTargetEndModel() ? new PublishIntegrationEventModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsPublishIntegrationEventSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == PublishIntegrationEventSourceEndModel.SpecializationTypeId;
        }

        public static PublishIntegrationEventSourceEndModel AsPublishIntegrationEventSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsPublishIntegrationEventSourceEndModel() ? new PublishIntegrationEventModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}