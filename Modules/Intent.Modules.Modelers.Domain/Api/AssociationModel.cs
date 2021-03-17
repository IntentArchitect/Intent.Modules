using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

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

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AssociationEndModel : ITypeReference, ICanBeReferencedType, IHasStereotypes
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
        public bool IsNullable => _associationEnd.IsNullable;
        public bool IsCollection => _associationEnd.IsCollection;
        public ICanBeReferencedType Element => _associationEnd.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;
        public string Comment => _associationEnd.Comment;
        public ITypeReference TypeReference => this;
        public IPackage Package => Element?.Package;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        [IntentManaged(Mode.Ignore)]
        public ClassModel Class => new ClassModel((IElement)_associationEnd.Element);

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

        //IAssociationEnd IAssociationEnd.OtherEnd()
        //{
        //    return this.Equals(_association.SourceEnd) ? (IAssociationEnd)_association.TargetEnd : (IAssociationEnd)_association.SourceEnd;
        //}

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

        IAssociation InternalAssociation => _association.InternalAssociation;

        IAssociationEnd InternalAssociationEnd => _associationEnd;
    }

    [IntentManaged(Mode.Fully)]
    public class AssociationSourceEndModel : AssociationEndModel
    {
        public const string SpecializationTypeId = "eaf9ed4e-0b61-4ac1-ba88-09f912c12087";

        public AssociationSourceEndModel(IAssociationEnd associationEnd, AssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class AssociationTargetEndModel : AssociationEndModel
    {
        public const string SpecializationTypeId = "eaf9ed4e-0b61-4ac1-ba88-09f912c12087";

        public AssociationTargetEndModel(IAssociationEnd associationEnd, AssociationModel association) : base(associationEnd, association)
        {
        }
    }
}