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
    public class DynamoDBItemAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "DynamoDB Item Association";
        public const string SpecializationTypeId = "605bfe10-c345-4493-a2aa-f0605e45d2ed";
        protected readonly IAssociation _association;
        protected DynamoDBItemAssociationSourceEndModel _sourceEnd;
        protected DynamoDBItemAssociationTargetEndModel _targetEnd;

        public DynamoDBItemAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static DynamoDBItemAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new DynamoDBItemAssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public DynamoDBItemAssociationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new DynamoDBItemAssociationSourceEndModel(_association.SourceEnd, this));

        public DynamoDBItemAssociationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new DynamoDBItemAssociationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(DynamoDBItemAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DynamoDBItemAssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DynamoDBItemAssociationSourceEndModel : DynamoDBItemAssociationEndModel
    {
        public const string SpecializationTypeId = "d275271d-33c5-4e47-bf9c-ac495869d292";

        public DynamoDBItemAssociationSourceEndModel(IAssociationEnd associationEnd, DynamoDBItemAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DynamoDBItemAssociationTargetEndModel : DynamoDBItemAssociationEndModel
    {
        public const string SpecializationTypeId = "ad655f31-0176-4d45-84cd-1acde6aa7325";

        public DynamoDBItemAssociationTargetEndModel(IAssociationEnd associationEnd, DynamoDBItemAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DynamoDBItemAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly DynamoDBItemAssociationModel _association;

        public DynamoDBItemAssociationEndModel(IAssociationEnd associationEnd, DynamoDBItemAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static DynamoDBItemAssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new DynamoDBItemAssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (DynamoDBItemAssociationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public DynamoDBItemAssociationModel Association => _association;
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

        public DynamoDBItemAssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (DynamoDBItemAssociationEndModel)_association.TargetEnd : (DynamoDBItemAssociationEndModel)_association.SourceEnd;
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

        public bool Equals(DynamoDBItemAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DynamoDBItemAssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DynamoDBItemAssociationEndModelExtensions
    {
        public static bool IsDynamoDBItemAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsDynamoDBItemAssociationTargetEndModel(type) || IsDynamoDBItemAssociationSourceEndModel(type);
        }

        public static DynamoDBItemAssociationEndModel AsDynamoDBItemAssociationEndModel(this ICanBeReferencedType type)
        {
            return (DynamoDBItemAssociationEndModel)type.AsDynamoDBItemAssociationTargetEndModel() ?? (DynamoDBItemAssociationEndModel)type.AsDynamoDBItemAssociationSourceEndModel();
        }

        public static bool IsDynamoDBItemAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DynamoDBItemAssociationTargetEndModel.SpecializationTypeId;
        }

        public static DynamoDBItemAssociationTargetEndModel AsDynamoDBItemAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsDynamoDBItemAssociationTargetEndModel() ? new DynamoDBItemAssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsDynamoDBItemAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DynamoDBItemAssociationSourceEndModel.SpecializationTypeId;
        }

        public static DynamoDBItemAssociationSourceEndModel AsDynamoDBItemAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsDynamoDBItemAssociationSourceEndModel() ? new DynamoDBItemAssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}