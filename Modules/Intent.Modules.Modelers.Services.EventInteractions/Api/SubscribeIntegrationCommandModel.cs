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
    public class SubscribeIntegrationCommandModel : IMetadataModel
    {
        public const string SpecializationType = "Subscribe Integration Command";
        public const string SpecializationTypeId = "f485e4aa-a032-4b9a-aa85-5d2c62d75799";
        protected readonly IAssociation _association;
        protected SubscribeIntegrationCommandSourceEndModel _sourceEnd;
        protected SubscribeIntegrationCommandTargetEndModel _targetEnd;

        public SubscribeIntegrationCommandModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static SubscribeIntegrationCommandModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new SubscribeIntegrationCommandModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public SubscribeIntegrationCommandSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new SubscribeIntegrationCommandSourceEndModel(_association.SourceEnd, this));

        public SubscribeIntegrationCommandTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new SubscribeIntegrationCommandTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(SubscribeIntegrationCommandModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscribeIntegrationCommandModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SubscribeIntegrationCommandSourceEndModel : SubscribeIntegrationCommandEndModel
    {
        public const string SpecializationTypeId = "cdc0ae0a-1199-4450-8e21-2da80e03bc26";
        public const string SpecializationType = "Subscribe Integration Command Source End";

        public SubscribeIntegrationCommandSourceEndModel(IAssociationEnd associationEnd, SubscribeIntegrationCommandModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SubscribeIntegrationCommandTargetEndModel : SubscribeIntegrationCommandEndModel
    {
        public const string SpecializationTypeId = "efa73bdb-69b7-4f52-aa10-15c19874b394";
        public const string SpecializationType = "Subscribe Integration Command Target End";

        public SubscribeIntegrationCommandTargetEndModel(IAssociationEnd associationEnd, SubscribeIntegrationCommandModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SubscribeIntegrationCommandEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly SubscribeIntegrationCommandModel _association;

        public SubscribeIntegrationCommandEndModel(IAssociationEnd associationEnd, SubscribeIntegrationCommandModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static SubscribeIntegrationCommandEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new SubscribeIntegrationCommandModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (SubscribeIntegrationCommandEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public SubscribeIntegrationCommandModel Association => _association;
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

        public SubscribeIntegrationCommandEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (SubscribeIntegrationCommandEndModel)_association.TargetEnd : (SubscribeIntegrationCommandEndModel)_association.SourceEnd;
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

        public bool Equals(SubscribeIntegrationCommandEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscribeIntegrationCommandEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SubscribeIntegrationCommandEndModelExtensions
    {
        public static bool IsSubscribeIntegrationCommandEndModel(this ICanBeReferencedType type)
        {
            return IsSubscribeIntegrationCommandTargetEndModel(type) || IsSubscribeIntegrationCommandSourceEndModel(type);
        }

        public static SubscribeIntegrationCommandEndModel AsSubscribeIntegrationCommandEndModel(this ICanBeReferencedType type)
        {
            return (SubscribeIntegrationCommandEndModel)type.AsSubscribeIntegrationCommandTargetEndModel() ?? (SubscribeIntegrationCommandEndModel)type.AsSubscribeIntegrationCommandSourceEndModel();
        }

        public static bool IsSubscribeIntegrationCommandTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SubscribeIntegrationCommandTargetEndModel.SpecializationTypeId;
        }

        public static SubscribeIntegrationCommandTargetEndModel AsSubscribeIntegrationCommandTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsSubscribeIntegrationCommandTargetEndModel() ? new SubscribeIntegrationCommandModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsSubscribeIntegrationCommandSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SubscribeIntegrationCommandSourceEndModel.SpecializationTypeId;
        }

        public static SubscribeIntegrationCommandSourceEndModel AsSubscribeIntegrationCommandSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsSubscribeIntegrationCommandSourceEndModel() ? new SubscribeIntegrationCommandModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}