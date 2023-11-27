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
    public class CreateEntityActionModel : IMetadataModel
    {
        public const string SpecializationType = "Create Entity Action";
        public const string SpecializationTypeId = "7a3f0474-3cf8-4249-baac-8c07c49465e0";
        protected readonly IAssociation _association;
        protected CreateEntityActionSourceEndModel _sourceEnd;
        protected CreateEntityActionTargetEndModel _targetEnd;

        public CreateEntityActionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CreateEntityActionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CreateEntityActionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CreateEntityActionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CreateEntityActionSourceEndModel(_association.SourceEnd, this));

        public CreateEntityActionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CreateEntityActionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CreateEntityActionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CreateEntityActionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CreateEntityActionSourceEndModel : CreateEntityActionEndModel
    {
        public const string SpecializationTypeId = "a3e7c59e-b0a1-47e1-ba29-66f2c7047b0a";
        public const string SpecializationType = "Create Entity Action Source End";

        public CreateEntityActionSourceEndModel(IAssociationEnd associationEnd, CreateEntityActionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CreateEntityActionTargetEndModel : CreateEntityActionEndModel
    {
        public const string SpecializationTypeId = "328f54e5-7bad-4b5f-90ca-03ce3105d016";
        public const string SpecializationType = "Create Entity Action Target End";

        public CreateEntityActionTargetEndModel(IAssociationEnd associationEnd, CreateEntityActionModel association) : base(associationEnd, association)
        {
        }
        public IList<ProcessingActionModel> ProcessingActions => InternalElement.ChildElements
            .GetElementsOfType(ProcessingActionModel.SpecializationTypeId)
            .Select(x => new ProcessingActionModel(x))
            .ToList();

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class CreateEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CreateEntityActionModel _association;

        public CreateEntityActionEndModel(IAssociationEnd associationEnd, CreateEntityActionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CreateEntityActionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CreateEntityActionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CreateEntityActionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CreateEntityActionModel Association => _association;
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

        public CreateEntityActionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CreateEntityActionEndModel)_association.TargetEnd : (CreateEntityActionEndModel)_association.SourceEnd;
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

        public bool Equals(CreateEntityActionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CreateEntityActionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CreateEntityActionEndModelExtensions
    {
        public static bool IsCreateEntityActionEndModel(this ICanBeReferencedType type)
        {
            return IsCreateEntityActionTargetEndModel(type) || IsCreateEntityActionSourceEndModel(type);
        }

        public static CreateEntityActionEndModel AsCreateEntityActionEndModel(this ICanBeReferencedType type)
        {
            return (CreateEntityActionEndModel)type.AsCreateEntityActionTargetEndModel() ?? (CreateEntityActionEndModel)type.AsCreateEntityActionSourceEndModel();
        }

        public static bool IsCreateEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CreateEntityActionTargetEndModel.SpecializationTypeId;
        }

        public static CreateEntityActionTargetEndModel AsCreateEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCreateEntityActionTargetEndModel() ? new CreateEntityActionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCreateEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CreateEntityActionSourceEndModel.SpecializationTypeId;
        }

        public static CreateEntityActionSourceEndModel AsCreateEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCreateEntityActionSourceEndModel() ? new CreateEntityActionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}