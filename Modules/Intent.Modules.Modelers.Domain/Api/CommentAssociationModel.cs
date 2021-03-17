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
    public class CommentAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Comment Association";
        protected readonly IAssociation _association;
        protected CommentSourceEndModel _sourceEnd;
        protected CommentTargetEndModel _targetEnd;

        public CommentAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CommentAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CommentAssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CommentSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CommentSourceEndModel(_association.SourceEnd, this));

        public CommentTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CommentTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CommentAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommentAssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
        public const string SpecializationTypeId = "5264c135-e856-468d-8bd7-154b75842256";
    }

    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CommentAssociationEndModel : ITypeReference, ICanBeReferencedType, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CommentAssociationModel _association;

        public CommentAssociationEndModel(IAssociationEnd associationEnd, CommentAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CommentAssociationModel Association => _association;
        IAssociationEnd InternalAssociationEnd => _associationEnd;
        IAssociation InternalAssociation => _association.InternalAssociation;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.IsNullable;
        public bool IsCollection => _associationEnd.IsCollection;
        public ICanBeReferencedType Element => _associationEnd.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;
        public ITypeReference TypeReference => this;
        public IPackage Package => Element?.Package;
        public string Comment => _associationEnd.Comment;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public CommentAssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CommentAssociationEndModel)_association.TargetEnd : (CommentAssociationEndModel)_association.SourceEnd;
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

        public bool Equals(CommentAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommentAssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CommentSourceEndModel : CommentAssociationEndModel
    {
        public const string SpecializationTypeId = "5264c135-e856-468d-8bd7-154b75842256";

        public CommentSourceEndModel(IAssociationEnd associationEnd, CommentAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CommentTargetEndModel : CommentAssociationEndModel
    {
        public const string SpecializationTypeId = "5264c135-e856-468d-8bd7-154b75842256";

        public CommentTargetEndModel(IAssociationEnd associationEnd, CommentAssociationModel association) : base(associationEnd, association)
        {
        }
    }
}
