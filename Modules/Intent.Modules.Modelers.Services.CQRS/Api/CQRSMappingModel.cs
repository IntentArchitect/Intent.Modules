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
    public class CQRSMappingModel : IMetadataModel
    {
        public const string SpecializationType = "CQRS Mapping";
        public const string SpecializationTypeId = "1f0777dc-4647-408c-b313-ab1bb0a659cf";
        protected readonly IAssociation _association;
        protected CQRSMappingSourceEndModel _sourceEnd;
        protected CQRSMappingTargetEndModel _targetEnd;

        public CQRSMappingModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CQRSMappingModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CQRSMappingModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CQRSMappingSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CQRSMappingSourceEndModel(_association.SourceEnd, this));

        public CQRSMappingTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CQRSMappingTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CQRSMappingModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CQRSMappingModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CQRSMappingSourceEndModel : CQRSMappingEndModel
    {
        public const string SpecializationTypeId = "8561dd59-d363-49f6-a9e1-333d7c1c7ae0";

        public CQRSMappingSourceEndModel(IAssociationEnd associationEnd, CQRSMappingModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CQRSMappingTargetEndModel : CQRSMappingEndModel
    {
        public const string SpecializationTypeId = "b4782cd3-7532-43d3-a759-4efa1920aa65";

        public CQRSMappingTargetEndModel(IAssociationEnd associationEnd, CQRSMappingModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CQRSMappingEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CQRSMappingModel _association;

        public CQRSMappingEndModel(IAssociationEnd associationEnd, CQRSMappingModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CQRSMappingEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CQRSMappingModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CQRSMappingEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CQRSMappingModel Association => _association;
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

        public CQRSMappingEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CQRSMappingEndModel)_association.TargetEnd : (CQRSMappingEndModel)_association.SourceEnd;
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

        public bool Equals(CQRSMappingEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CQRSMappingEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CQRSMappingEndModelExtensions
    {
        public static bool IsCQRSMappingEndModel(this ICanBeReferencedType type)
        {
            return IsCQRSMappingTargetEndModel(type) || IsCQRSMappingSourceEndModel(type);
        }

        public static CQRSMappingEndModel AsCQRSMappingEndModel(this ICanBeReferencedType type)
        {
            return (CQRSMappingEndModel)type.AsCQRSMappingTargetEndModel() ?? (CQRSMappingEndModel)type.AsCQRSMappingSourceEndModel();
        }

        public static bool IsCQRSMappingTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CQRSMappingTargetEndModel.SpecializationTypeId;
        }

        public static CQRSMappingTargetEndModel AsCQRSMappingTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCQRSMappingTargetEndModel() ? new CQRSMappingModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCQRSMappingSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CQRSMappingSourceEndModel.SpecializationTypeId;
        }

        public static CQRSMappingSourceEndModel AsCQRSMappingSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCQRSMappingSourceEndModel() ? new CQRSMappingModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}