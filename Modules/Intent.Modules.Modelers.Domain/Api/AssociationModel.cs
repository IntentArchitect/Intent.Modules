using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.SdkEvolutionHelpers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Association";
        protected readonly IAssociation _association;
        protected AssociationSourceEndModel _sourceEnd;
        protected AssociationTargetEndModel _targetEnd;

        public AssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static AssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new AssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public AssociationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new AssociationSourceEndModel(_association.SourceEnd, this));

        public AssociationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new AssociationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        [IntentManaged(Mode.Ignore)]
        public AssociationType AssociationType => !SourceEnd.IsNullable && !SourceEnd.IsCollection ? AssociationType.Composition : AssociationType.Aggregation;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(AssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
        public const string SpecializationTypeId = "eaf9ed4e-0b61-4ac1-ba88-09f912c12087";
    }

    [FixFor_Version4("Should implement IHasTypeReference and NOT ITypeReference")]
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class AssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly AssociationModel _association;


        public AssociationEndModel(IAssociationEnd associationEnd, AssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public AssociationModel Association => _association;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.TypeReference.IsNullable;
        public bool IsCollection => _associationEnd.TypeReference.IsCollection;
        public ICanBeReferencedType Element => _associationEnd.TypeReference.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.TypeReference.GenericTypeParameters;
        public string Comment => _associationEnd.Comment;
        public ITypeReference TypeReference => this;
        public IPackage Package => Element?.Package;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;
        public IAssociation InternalAssociation => _association.InternalAssociation;
        public IAssociationEnd InternalAssociationEnd => _associationEnd;

        [IntentManaged(Mode.Ignore)]
        public ClassModel Class => _associationEnd.TypeReference.Element.AsClassModel();

        [IntentManaged(Mode.Ignore)]
        public Multiplicity Multiplicity
        {
            get
            {
                if (IsNullable && !IsCollection)
                {
                    return Multiplicity.ZeroToOne;
                }
                if (!IsNullable && !IsCollection)
                {
                    return Multiplicity.One;
                }
                return Multiplicity.Many;
            }
        }

        [IntentManaged(Mode.Ignore)]
        public AssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (AssociationEndModel)_association.TargetEnd : (AssociationEndModel)_association.SourceEnd;
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

        public bool Equals(AssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }

        public static AssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new AssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (AssociationEndModel)association.TargetEnd : association.SourceEnd;
        }
    }

    [IntentManaged(Mode.Fully)]
    public class AssociationSourceEndModel : AssociationEndModel
    {
        public const string SpecializationTypeId = "8d9d2e5b-bd55-4f36-9ae4-2b9e84fd4e58";

        public AssociationSourceEndModel(IAssociationEnd associationEnd, AssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class AssociationTargetEndModel : AssociationEndModel
    {
        public const string SpecializationTypeId = "0a66489f-30aa-417b-a75d-b945863366fd";

        public AssociationTargetEndModel(IAssociationEnd associationEnd, AssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AssociationEndModelExtensions
    {
        public static bool IsAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsAssociationTargetEndModel(type) || IsAssociationSourceEndModel(type);
        }

        public static AssociationEndModel AsAssociationEndModel(this ICanBeReferencedType type)
        {
            return (AssociationEndModel)type.AsAssociationTargetEndModel() ?? (AssociationEndModel)type.AsAssociationSourceEndModel();
        }

        public static bool IsAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == AssociationTargetEndModel.SpecializationTypeId;
        }

        public static AssociationTargetEndModel AsAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationTargetEndModel() ? new AssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == AssociationSourceEndModel.SpecializationTypeId;
        }

        public static AssociationSourceEndModel AsAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationSourceEndModel() ? new AssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}