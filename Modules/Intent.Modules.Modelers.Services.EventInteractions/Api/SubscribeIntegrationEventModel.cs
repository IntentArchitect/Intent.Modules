using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class SubscribeIntegrationEventModel : IMetadataModel
    {
        public const string SpecializationType = "Subscribe Integration Event";
        public const string SpecializationTypeId = "80aa7f6d-64e5-4d24-a81e-6bc212925ca7";
        protected readonly IAssociation _association;
        protected SubscribeIntegrationEventSourceEndModel _sourceEnd;
        protected SubscribeIntegrationEventTargetEndModel _targetEnd;

        public SubscribeIntegrationEventModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static SubscribeIntegrationEventModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new SubscribeIntegrationEventModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public SubscribeIntegrationEventSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new SubscribeIntegrationEventSourceEndModel(_association.SourceEnd, this));

        public SubscribeIntegrationEventTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new SubscribeIntegrationEventTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(SubscribeIntegrationEventModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscribeIntegrationEventModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SubscribeIntegrationEventSourceEndModel : SubscribeIntegrationEventEndModel
    {
        public const string SpecializationTypeId = "5d8f5c33-f9c9-4629-b637-fad9b0096894";

        public SubscribeIntegrationEventSourceEndModel(IAssociationEnd associationEnd, SubscribeIntegrationEventModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SubscribeIntegrationEventTargetEndModel : SubscribeIntegrationEventEndModel
    {
        public const string SpecializationTypeId = "16fa2952-79e6-4150-b5ab-45aa4c106de4";

        public SubscribeIntegrationEventTargetEndModel(IAssociationEnd associationEnd, SubscribeIntegrationEventModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SubscribeIntegrationEventEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly SubscribeIntegrationEventModel _association;

        public SubscribeIntegrationEventEndModel(IAssociationEnd associationEnd, SubscribeIntegrationEventModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static SubscribeIntegrationEventEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new SubscribeIntegrationEventModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (SubscribeIntegrationEventEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public SubscribeIntegrationEventModel Association => _association;
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

        public SubscribeIntegrationEventEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (SubscribeIntegrationEventEndModel)_association.TargetEnd : (SubscribeIntegrationEventEndModel)_association.SourceEnd;
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

        public bool Equals(SubscribeIntegrationEventEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscribeIntegrationEventEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SubscribeIntegrationEventEndModelExtensions
    {
        public static bool IsSubscribeIntegrationEventEndModel(this ICanBeReferencedType type)
        {
            return IsSubscribeIntegrationEventTargetEndModel(type) || IsSubscribeIntegrationEventSourceEndModel(type);
        }

        public static SubscribeIntegrationEventEndModel AsSubscribeIntegrationEventEndModel(this ICanBeReferencedType type)
        {
            return (SubscribeIntegrationEventEndModel)type.AsSubscribeIntegrationEventTargetEndModel() ?? (SubscribeIntegrationEventEndModel)type.AsSubscribeIntegrationEventSourceEndModel();
        }

        public static bool IsSubscribeIntegrationEventTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SubscribeIntegrationEventTargetEndModel.SpecializationTypeId;
        }

        public static SubscribeIntegrationEventTargetEndModel AsSubscribeIntegrationEventTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsSubscribeIntegrationEventTargetEndModel() ? new SubscribeIntegrationEventModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsSubscribeIntegrationEventSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SubscribeIntegrationEventSourceEndModel.SpecializationTypeId;
        }

        public static SubscribeIntegrationEventSourceEndModel AsSubscribeIntegrationEventSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsSubscribeIntegrationEventSourceEndModel() ? new SubscribeIntegrationEventModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}