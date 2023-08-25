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
    public class CreateActionMappingModel : IMetadataModel
    {
        public const string SpecializationType = "Create Action Mapping";
        public const string SpecializationTypeId = "1f0777dc-4647-408c-b313-ab1bb0a659cf";
        protected readonly IAssociation _association;
        protected CreateActionMappingSourceEndModel _sourceEnd;
        protected CreateActionMappingTargetEndModel _targetEnd;

        public CreateActionMappingModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CreateActionMappingModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CreateActionMappingModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CreateActionMappingSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CreateActionMappingSourceEndModel(_association.SourceEnd, this));

        public CreateActionMappingTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CreateActionMappingTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CreateActionMappingModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CreateActionMappingModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CreateActionMappingSourceEndModel : CreateActionMappingEndModel
    {
        public const string SpecializationTypeId = "8561dd59-d363-49f6-a9e1-333d7c1c7ae0";

        public CreateActionMappingSourceEndModel(IAssociationEnd associationEnd, CreateActionMappingModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CreateActionMappingTargetEndModel : CreateActionMappingEndModel
    {
        public const string SpecializationTypeId = "b4782cd3-7532-43d3-a759-4efa1920aa65";

        public CreateActionMappingTargetEndModel(IAssociationEnd associationEnd, CreateActionMappingModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class CreateActionMappingEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CreateActionMappingModel _association;

        public CreateActionMappingEndModel(IAssociationEnd associationEnd, CreateActionMappingModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CreateActionMappingEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CreateActionMappingModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CreateActionMappingEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CreateActionMappingModel Association => _association;
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

        public CreateActionMappingEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CreateActionMappingEndModel)_association.TargetEnd : (CreateActionMappingEndModel)_association.SourceEnd;
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

        public bool Equals(CreateActionMappingEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CreateActionMappingEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CreateActionMappingEndModelExtensions
    {
        public static bool IsCreateActionMappingEndModel(this ICanBeReferencedType type)
        {
            return IsCreateActionMappingTargetEndModel(type) || IsCreateActionMappingSourceEndModel(type);
        }

        public static CreateActionMappingEndModel AsCreateActionMappingEndModel(this ICanBeReferencedType type)
        {
            return (CreateActionMappingEndModel)type.AsCreateActionMappingTargetEndModel() ?? (CreateActionMappingEndModel)type.AsCreateActionMappingSourceEndModel();
        }

        public static bool IsCreateActionMappingTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CreateActionMappingTargetEndModel.SpecializationTypeId;
        }

        public static CreateActionMappingTargetEndModel AsCreateActionMappingTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCreateActionMappingTargetEndModel() ? new CreateActionMappingModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCreateActionMappingSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CreateActionMappingSourceEndModel.SpecializationTypeId;
        }

        public static CreateActionMappingSourceEndModel AsCreateActionMappingSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCreateActionMappingSourceEndModel() ? new CreateActionMappingModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}