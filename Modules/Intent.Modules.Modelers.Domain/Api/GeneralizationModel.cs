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
    public class GeneralizationModel : IMetadataModel
    {
        public const string SpecializationType = "Generalization";
        protected readonly IAssociation _association;
        protected GeneralizationSourceEndModel _sourceEnd;
        protected GeneralizationTargetEndModel _targetEnd;

        public GeneralizationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static GeneralizationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new GeneralizationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public GeneralizationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new GeneralizationSourceEndModel(_association.SourceEnd, this));

        public GeneralizationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new GeneralizationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(GeneralizationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneralizationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GeneralizationEndModel : ITypeReference, ICanBeReferencedType, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;

        public GeneralizationEndModel(IAssociationEnd associationEnd, GeneralizationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public string Id => _associationEnd.Id;
        public string Name => _associationEnd.Name;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public bool Equals(GeneralizationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneralizationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }

        private readonly GeneralizationModel _association;
        public GeneralizationModel Association => _association;
        public string Comment => _associationEnd.Comment;
        public ICanBeReferencedType Element => _associationEnd.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;
        public bool IsCollection => _associationEnd.IsCollection;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.IsNullable;

        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        public GeneralizationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (GeneralizationEndModel)_association.TargetEnd : (GeneralizationEndModel)_association.SourceEnd;
        }

        public override string ToString()
        {
            return _associationEnd.ToString();
        }

        IAssociation InternalAssociation => _association.InternalAssociation;

        IAssociationEnd InternalAssociationEnd => _associationEnd;

        public IPackage Package => Element?.Package;

        public string SpecializationType => _associationEnd.SpecializationType;

        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;

        public ITypeReference TypeReference => this;
    }

    [IntentManaged(Mode.Fully)]
    public class GeneralizationSourceEndModel : GeneralizationEndModel
    {
        public GeneralizationSourceEndModel(IAssociationEnd associationEnd, GeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class GeneralizationTargetEndModel : GeneralizationEndModel
    {
        public GeneralizationTargetEndModel(IAssociationEnd associationEnd, GeneralizationModel association) : base(associationEnd, association)
        {
        }
    }
}