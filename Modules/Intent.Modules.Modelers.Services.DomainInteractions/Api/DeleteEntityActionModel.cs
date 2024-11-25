using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DeleteEntityActionModel : IMetadataModel
    {
        public const string SpecializationType = "Delete Entity Action";
        public const string SpecializationTypeId = "bfc823fb-60ab-451d-ba62-12671fe7e28e";
        protected readonly IAssociation _association;
        protected DeleteEntityActionSourceEndModel _sourceEnd;
        protected DeleteEntityActionTargetEndModel _targetEnd;

        public DeleteEntityActionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static DeleteEntityActionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new DeleteEntityActionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public DeleteEntityActionSourceEndModel SourceEnd => _sourceEnd ??= new DeleteEntityActionSourceEndModel(_association.SourceEnd, this);

        public DeleteEntityActionTargetEndModel TargetEnd => _targetEnd ??= new DeleteEntityActionTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(DeleteEntityActionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeleteEntityActionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DeleteEntityActionSourceEndModel : DeleteEntityActionEndModel
    {
        public const string SpecializationTypeId = "8c2d9fed-bd14-44b2-9f98-8a801aaf157e";
        public const string SpecializationType = "Delete Entity Action Source End";

        public DeleteEntityActionSourceEndModel(IAssociationEnd associationEnd, DeleteEntityActionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DeleteEntityActionTargetEndModel : DeleteEntityActionEndModel
    {
        public const string SpecializationTypeId = "4a04cfc2-5841-438c-9c16-fb58b784b365";
        public const string SpecializationType = "Delete Entity Action Target End";

        public DeleteEntityActionTargetEndModel(IAssociationEnd associationEnd, DeleteEntityActionModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class DeleteEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly DeleteEntityActionModel _association;

        public DeleteEntityActionEndModel(IAssociationEnd associationEnd, DeleteEntityActionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static DeleteEntityActionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new DeleteEntityActionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (DeleteEntityActionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public DeleteEntityActionModel Association => _association;
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

        public DeleteEntityActionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (DeleteEntityActionEndModel)_association.TargetEnd : (DeleteEntityActionEndModel)_association.SourceEnd;
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

        public bool Equals(DeleteEntityActionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeleteEntityActionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DeleteEntityActionEndModelExtensions
    {
        public static bool IsDeleteEntityActionEndModel(this ICanBeReferencedType type)
        {
            return IsDeleteEntityActionTargetEndModel(type) || IsDeleteEntityActionSourceEndModel(type);
        }

        public static DeleteEntityActionEndModel AsDeleteEntityActionEndModel(this ICanBeReferencedType type)
        {
            return (DeleteEntityActionEndModel)type.AsDeleteEntityActionTargetEndModel() ?? (DeleteEntityActionEndModel)type.AsDeleteEntityActionSourceEndModel();
        }

        public static bool IsDeleteEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DeleteEntityActionTargetEndModel.SpecializationTypeId;
        }

        public static DeleteEntityActionTargetEndModel AsDeleteEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsDeleteEntityActionTargetEndModel() ? new DeleteEntityActionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsDeleteEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DeleteEntityActionSourceEndModel.SpecializationTypeId;
        }

        public static DeleteEntityActionSourceEndModel AsDeleteEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsDeleteEntityActionSourceEndModel() ? new DeleteEntityActionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}