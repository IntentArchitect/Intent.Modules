using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.StoredProcedures.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class StoredProcedureInvocationModel : IMetadataModel
    {
        public const string SpecializationType = "Stored Procedure Invocation";
        public const string SpecializationTypeId = "adf062ed-c0a4-421f-9940-318a91e9a52c";
        protected readonly IAssociation _association;
        protected StoredProcedureInvocationSourceEndModel _sourceEnd;
        protected StoredProcedureInvocationTargetEndModel _targetEnd;

        public StoredProcedureInvocationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static StoredProcedureInvocationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new StoredProcedureInvocationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public StoredProcedureInvocationSourceEndModel SourceEnd => _sourceEnd ??= new StoredProcedureInvocationSourceEndModel(_association.SourceEnd, this);

        public StoredProcedureInvocationTargetEndModel TargetEnd => _targetEnd ??= new StoredProcedureInvocationTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(StoredProcedureInvocationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StoredProcedureInvocationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StoredProcedureInvocationSourceEndModel : StoredProcedureInvocationEndModel
    {
        public const string SpecializationTypeId = "7b7d3fd8-5e32-4f8c-b4cc-7b92f45a8577";
        public const string SpecializationType = "Stored Procedure Invocation Source End";

        public StoredProcedureInvocationSourceEndModel(IAssociationEnd associationEnd, StoredProcedureInvocationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class StoredProcedureInvocationTargetEndModel : StoredProcedureInvocationEndModel
    {
        public const string SpecializationTypeId = "d0b0b24a-db0f-4aff-873a-a0e9c2dce12d";
        public const string SpecializationType = "Stored Procedure Invocation Target End";

        public StoredProcedureInvocationTargetEndModel(IAssociationEnd associationEnd, StoredProcedureInvocationModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class StoredProcedureInvocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly StoredProcedureInvocationModel _association;

        public StoredProcedureInvocationEndModel(IAssociationEnd associationEnd, StoredProcedureInvocationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static StoredProcedureInvocationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new StoredProcedureInvocationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (StoredProcedureInvocationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public StoredProcedureInvocationModel Association => _association;
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

        public StoredProcedureInvocationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (StoredProcedureInvocationEndModel)_association.TargetEnd : (StoredProcedureInvocationEndModel)_association.SourceEnd;
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

        public bool Equals(StoredProcedureInvocationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StoredProcedureInvocationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StoredProcedureInvocationEndModelExtensions
    {
        public static bool IsStoredProcedureInvocationEndModel(this ICanBeReferencedType type)
        {
            return IsStoredProcedureInvocationTargetEndModel(type) || IsStoredProcedureInvocationSourceEndModel(type);
        }

        public static StoredProcedureInvocationEndModel AsStoredProcedureInvocationEndModel(this ICanBeReferencedType type)
        {
            return (StoredProcedureInvocationEndModel)type.AsStoredProcedureInvocationTargetEndModel() ?? (StoredProcedureInvocationEndModel)type.AsStoredProcedureInvocationSourceEndModel();
        }

        public static bool IsStoredProcedureInvocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == StoredProcedureInvocationTargetEndModel.SpecializationTypeId;
        }

        public static StoredProcedureInvocationTargetEndModel AsStoredProcedureInvocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsStoredProcedureInvocationTargetEndModel() ? new StoredProcedureInvocationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsStoredProcedureInvocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == StoredProcedureInvocationSourceEndModel.SpecializationTypeId;
        }

        public static StoredProcedureInvocationSourceEndModel AsStoredProcedureInvocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsStoredProcedureInvocationSourceEndModel() ? new StoredProcedureInvocationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}