using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class UpdateActionMappingModel : IMetadataModel
    {
        public const string SpecializationType = "Update Action Mapping";
        public const string SpecializationTypeId = "efbe6193-5d1e-40a9-b98d-fb15ca9e547b";
        protected readonly IAssociation _association;
        protected UpdateActionMappingSourceEndModel _sourceEnd;
        protected UpdateActionMappingTargetEndModel _targetEnd;

        public UpdateActionMappingModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static UpdateActionMappingModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new UpdateActionMappingModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public UpdateActionMappingSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new UpdateActionMappingSourceEndModel(_association.SourceEnd, this));

        public UpdateActionMappingTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new UpdateActionMappingTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(UpdateActionMappingModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateActionMappingModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UpdateActionMappingSourceEndModel : UpdateActionMappingEndModel
    {
        public const string SpecializationTypeId = "a2d0757b-0afb-4b3b-9c0f-4ba25c659f62";

        public UpdateActionMappingSourceEndModel(IAssociationEnd associationEnd, UpdateActionMappingModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UpdateActionMappingTargetEndModel : UpdateActionMappingEndModel
    {
        public const string SpecializationTypeId = "5f5b2984-1ee1-47c1-a4a1-1744e0273856";

        public UpdateActionMappingTargetEndModel(IAssociationEnd associationEnd, UpdateActionMappingModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class UpdateActionMappingEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly UpdateActionMappingModel _association;

        public UpdateActionMappingEndModel(IAssociationEnd associationEnd, UpdateActionMappingModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static UpdateActionMappingEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new UpdateActionMappingModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (UpdateActionMappingEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public UpdateActionMappingModel Association => _association;
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

        public UpdateActionMappingEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (UpdateActionMappingEndModel)_association.TargetEnd : (UpdateActionMappingEndModel)_association.SourceEnd;
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

        public bool Equals(UpdateActionMappingEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateActionMappingEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class UpdateActionMappingEndModelExtensions
    {
        public static bool IsUpdateActionMappingEndModel(this ICanBeReferencedType type)
        {
            return IsUpdateActionMappingTargetEndModel(type) || IsUpdateActionMappingSourceEndModel(type);
        }

        public static UpdateActionMappingEndModel AsUpdateActionMappingEndModel(this ICanBeReferencedType type)
        {
            return (UpdateActionMappingEndModel)type.AsUpdateActionMappingTargetEndModel() ?? (UpdateActionMappingEndModel)type.AsUpdateActionMappingSourceEndModel();
        }

        public static bool IsUpdateActionMappingTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == UpdateActionMappingTargetEndModel.SpecializationTypeId;
        }

        public static UpdateActionMappingTargetEndModel AsUpdateActionMappingTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsUpdateActionMappingTargetEndModel() ? new UpdateActionMappingModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsUpdateActionMappingSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == UpdateActionMappingSourceEndModel.SpecializationTypeId;
        }

        public static UpdateActionMappingSourceEndModel AsUpdateActionMappingSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsUpdateActionMappingSourceEndModel() ? new UpdateActionMappingModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}