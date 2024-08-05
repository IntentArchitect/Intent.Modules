using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CommentAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Comment Association";
        public const string SpecializationTypeId = "eea9b48a-e5e9-48c2-b3ae-cd51d3f7b7bf";
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
    }

    [IntentManaged(Mode.Fully)]
    public class CommentSourceEndModel : CommentAssociationEndModel
    {
        public const string SpecializationTypeId = "0952566b-9884-4ed0-92f3-10bc137b8f41";
        public const string SpecializationType = "Comment Source End";

        public CommentSourceEndModel(IAssociationEnd associationEnd, CommentAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CommentTargetEndModel : CommentAssociationEndModel
    {
        public const string SpecializationTypeId = "2c33691a-e912-4d4f-99af-c82ff0fd9756";
        public const string SpecializationType = "Comment Target End";

        public CommentTargetEndModel(IAssociationEnd associationEnd, CommentAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CommentAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CommentAssociationModel _association;

        public CommentAssociationEndModel(IAssociationEnd associationEnd, CommentAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CommentAssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CommentAssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CommentAssociationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CommentAssociationModel Association => _association;
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
    public static class CommentAssociationEndModelExtensions
    {
        public static bool IsCommentAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsCommentTargetEndModel(type) || IsCommentSourceEndModel(type);
        }

        public static CommentAssociationEndModel AsCommentAssociationEndModel(this ICanBeReferencedType type)
        {
            return (CommentAssociationEndModel)type.AsCommentTargetEndModel() ?? (CommentAssociationEndModel)type.AsCommentSourceEndModel();
        }

        public static bool IsCommentTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CommentTargetEndModel.SpecializationTypeId;
        }

        public static CommentTargetEndModel AsCommentTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCommentTargetEndModel() ? new CommentAssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCommentSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CommentSourceEndModel.SpecializationTypeId;
        }

        public static CommentSourceEndModel AsCommentSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCommentSourceEndModel() ? new CommentAssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}