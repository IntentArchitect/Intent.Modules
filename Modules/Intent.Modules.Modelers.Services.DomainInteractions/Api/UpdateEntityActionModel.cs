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
    public class UpdateEntityActionModel : IMetadataModel
    {
        public const string SpecializationType = "Update Entity Action";
        public const string SpecializationTypeId = "9ea0382a-4617-412a-a8c8-af987bbce226";
        protected readonly IAssociation _association;
        protected UpdateEntityActionSourceEndModel _sourceEnd;
        protected UpdateEntityActionTargetEndModel _targetEnd;

        public UpdateEntityActionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static UpdateEntityActionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new UpdateEntityActionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public UpdateEntityActionSourceEndModel SourceEnd => _sourceEnd ??= new UpdateEntityActionSourceEndModel(_association.SourceEnd, this);

        public UpdateEntityActionTargetEndModel TargetEnd => _targetEnd ??= new UpdateEntityActionTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(UpdateEntityActionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateEntityActionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UpdateEntityActionSourceEndModel : UpdateEntityActionEndModel
    {
        public const string SpecializationTypeId = "6bc95978-6def-4d0c-a4f5-25bdeda8a9f6";
        public const string SpecializationType = "Update Entity Action Source End";

        public UpdateEntityActionSourceEndModel(IAssociationEnd associationEnd, UpdateEntityActionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UpdateEntityActionTargetEndModel : UpdateEntityActionEndModel
    {
        public const string SpecializationTypeId = "516069f6-09cc-4de8-8e31-3c71ca823452";
        public const string SpecializationType = "Update Entity Action Target End";

        public UpdateEntityActionTargetEndModel(IAssociationEnd associationEnd, UpdateEntityActionModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class UpdateEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly UpdateEntityActionModel _association;

        public UpdateEntityActionEndModel(IAssociationEnd associationEnd, UpdateEntityActionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static UpdateEntityActionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new UpdateEntityActionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (UpdateEntityActionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public UpdateEntityActionModel Association => _association;
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

        public UpdateEntityActionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (UpdateEntityActionEndModel)_association.TargetEnd : (UpdateEntityActionEndModel)_association.SourceEnd;
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

        public bool Equals(UpdateEntityActionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateEntityActionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class UpdateEntityActionEndModelExtensions
    {
        public static bool IsUpdateEntityActionEndModel(this ICanBeReferencedType type)
        {
            return IsUpdateEntityActionTargetEndModel(type) || IsUpdateEntityActionSourceEndModel(type);
        }

        public static UpdateEntityActionEndModel AsUpdateEntityActionEndModel(this ICanBeReferencedType type)
        {
            return (UpdateEntityActionEndModel)type.AsUpdateEntityActionTargetEndModel() ?? (UpdateEntityActionEndModel)type.AsUpdateEntityActionSourceEndModel();
        }

        public static bool IsUpdateEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == UpdateEntityActionTargetEndModel.SpecializationTypeId;
        }

        public static UpdateEntityActionTargetEndModel AsUpdateEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsUpdateEntityActionTargetEndModel() ? new UpdateEntityActionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsUpdateEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == UpdateEntityActionSourceEndModel.SpecializationTypeId;
        }

        public static UpdateEntityActionSourceEndModel AsUpdateEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsUpdateEntityActionSourceEndModel() ? new UpdateEntityActionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}