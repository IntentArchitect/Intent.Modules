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
    public class UsesTableModel : IMetadataModel
    {
        public const string SpecializationType = "Uses Table";
        public const string SpecializationTypeId = "e1a3b27b-6364-433e-96a8-24864841f688";
        protected readonly IAssociation _association;
        protected UsesTableSourceEndModel _sourceEnd;
        protected UsesTableTargetEndModel _targetEnd;

        public UsesTableModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static UsesTableModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new UsesTableModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public UsesTableSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new UsesTableSourceEndModel(_association.SourceEnd, this));

        public UsesTableTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new UsesTableTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(UsesTableModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UsesTableModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UsesTableSourceEndModel : UsesTableEndModel
    {
        public const string SpecializationTypeId = "e5ec0217-b28f-4ec6-ac4e-e4b5c2897350";

        public UsesTableSourceEndModel(IAssociationEnd associationEnd, UsesTableModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UsesTableTargetEndModel : UsesTableEndModel
    {
        public const string SpecializationTypeId = "51d80704-b51a-403d-83d1-fb121f6d909f";

        public UsesTableTargetEndModel(IAssociationEnd associationEnd, UsesTableModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UsesTableEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly UsesTableModel _association;

        public UsesTableEndModel(IAssociationEnd associationEnd, UsesTableModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static UsesTableEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new UsesTableModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (UsesTableEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public UsesTableModel Association => _association;
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

        public UsesTableEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (UsesTableEndModel)_association.TargetEnd : (UsesTableEndModel)_association.SourceEnd;
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

        public bool Equals(UsesTableEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UsesTableEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class UsesTableEndModelExtensions
    {
        public static bool IsUsesTableEndModel(this ICanBeReferencedType type)
        {
            return IsUsesTableTargetEndModel(type) || IsUsesTableSourceEndModel(type);
        }

        public static UsesTableEndModel AsUsesTableEndModel(this ICanBeReferencedType type)
        {
            return (UsesTableEndModel)type.AsUsesTableTargetEndModel() ?? (UsesTableEndModel)type.AsUsesTableSourceEndModel();
        }

        public static bool IsUsesTableTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == UsesTableTargetEndModel.SpecializationTypeId;
        }

        public static UsesTableTargetEndModel AsUsesTableTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsUsesTableTargetEndModel() ? new UsesTableModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsUsesTableSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == UsesTableSourceEndModel.SpecializationTypeId;
        }

        public static UsesTableSourceEndModel AsUsesTableSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsUsesTableSourceEndModel() ? new UsesTableModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}