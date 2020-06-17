using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommentAssociationModel : IMetadataModel
    {
        [IntentManaged(Mode.Fully)]
        public const string SpecializationType = "Comment Association";
        [IntentManaged(Mode.Fully)]
        protected readonly IAssociation _association;

        public CommentAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _association.Id;

        [IntentManaged(Mode.Fully)]
        public IAssociation InternalAssociation => _association;

        [IntentManaged(Mode.Fully)]
        public CommentSourceEndModel SourceEnd => new CommentSourceEndModel(_association.SourceEnd, this);

        [IntentManaged(Mode.Fully)]
        public CommentTargetEndModel TargetEnd => new CommentTargetEndModel(_association.TargetEnd, this);

        [IntentManaged(Mode.Fully)]
        public static CommentAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CommentAssociationModel(associationEnd.Association);
            return association;
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(CommentAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommentAssociationModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _association.ToString();
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommentAssociationEndModel : IAssociationEnd
    {
        [IntentManaged(Mode.Fully)]
        protected readonly IAssociationEnd _associationEnd;
        [IntentManaged(Mode.Fully)]
        private readonly CommentAssociationModel _association;

        [IntentManaged(Mode.Fully)]
        public CommentAssociationEndModel(IAssociationEnd associationEnd, CommentAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _associationEnd.Id;
        [IntentManaged(Mode.Fully)]
        public string Name => _associationEnd.Name;
        [IntentManaged(Mode.Fully)]
        public CommentAssociationModel Association => _association;
        [IntentManaged(Mode.Fully)]
        IAssociation IAssociationEnd.Association => _association.InternalAssociation;
        [IntentManaged(Mode.Fully)]
        public bool IsNavigable => _associationEnd.IsNavigable;
        [IntentManaged(Mode.Fully)]
        public bool IsNullable => _associationEnd.IsNullable;
        [IntentManaged(Mode.Fully)]
        public bool IsCollection => _associationEnd.IsCollection;
        [IntentManaged(Mode.Fully)]
        public IElement Element => _associationEnd.Element;
        [IntentManaged(Mode.Fully)]
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;
        [IntentManaged(Mode.Fully)]
        public string Comment => _associationEnd.Comment;
        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        [IntentManaged(Mode.Fully)]
        IAssociationEnd IAssociationEnd.OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (IAssociationEnd)_association.TargetEnd : (IAssociationEnd)_association.SourceEnd;
        }

        [IntentManaged(Mode.Fully)]
        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        [IntentManaged(Mode.Fully)]
        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _associationEnd.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(CommentAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommentAssociationEndModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CommentSourceEndModel : CommentAssociationEndModel
    {
        public CommentSourceEndModel(IAssociationEnd associationEnd, CommentAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CommentTargetEndModel : CommentAssociationEndModel
    {
        public CommentTargetEndModel(IAssociationEnd associationEnd, CommentAssociationModel association) : base(associationEnd, association)
        {
        }
    }
}
