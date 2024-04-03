using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DataContractGeneralizationModel : IMetadataModel
    {
        public const string SpecializationType = "Data Contract Generalization";
        public const string SpecializationTypeId = "4199ae15-0ecc-4086-82f3-bfa885c9d3e8";
        protected readonly IAssociation _association;
        protected DataContractGeneralizationSourceEndModel _sourceEnd;
        protected DataContractGeneralizationTargetEndModel _targetEnd;

        public DataContractGeneralizationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static DataContractGeneralizationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new DataContractGeneralizationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public DataContractGeneralizationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new DataContractGeneralizationSourceEndModel(_association.SourceEnd, this));

        public DataContractGeneralizationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new DataContractGeneralizationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(DataContractGeneralizationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataContractGeneralizationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DataContractGeneralizationSourceEndModel : DataContractGeneralizationEndModel
    {
        public const string SpecializationTypeId = "12c2ffdc-9a54-4e99-9e09-a441fa260bef";
        public const string SpecializationType = "Data Contract Generalization Source End";

        public DataContractGeneralizationSourceEndModel(IAssociationEnd associationEnd, DataContractGeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DataContractGeneralizationTargetEndModel : DataContractGeneralizationEndModel
    {
        public const string SpecializationTypeId = "4ea029c6-e963-46c7-8d2f-e4ea73e05a07";
        public const string SpecializationType = "Data Contract Generalization Target End";

        public DataContractGeneralizationTargetEndModel(IAssociationEnd associationEnd, DataContractGeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DataContractGeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly DataContractGeneralizationModel _association;

        public DataContractGeneralizationEndModel(IAssociationEnd associationEnd, DataContractGeneralizationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static DataContractGeneralizationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new DataContractGeneralizationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (DataContractGeneralizationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public DataContractGeneralizationModel Association => _association;
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

        public DataContractGeneralizationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (DataContractGeneralizationEndModel)_association.TargetEnd : (DataContractGeneralizationEndModel)_association.SourceEnd;
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

        public bool Equals(DataContractGeneralizationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataContractGeneralizationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DataContractGeneralizationEndModelExtensions
    {
        public static bool IsDataContractGeneralizationEndModel(this ICanBeReferencedType type)
        {
            return IsDataContractGeneralizationTargetEndModel(type) || IsDataContractGeneralizationSourceEndModel(type);
        }

        public static DataContractGeneralizationEndModel AsDataContractGeneralizationEndModel(this ICanBeReferencedType type)
        {
            return (DataContractGeneralizationEndModel)type.AsDataContractGeneralizationTargetEndModel() ?? (DataContractGeneralizationEndModel)type.AsDataContractGeneralizationSourceEndModel();
        }

        public static bool IsDataContractGeneralizationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DataContractGeneralizationTargetEndModel.SpecializationTypeId;
        }

        public static DataContractGeneralizationTargetEndModel AsDataContractGeneralizationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsDataContractGeneralizationTargetEndModel() ? new DataContractGeneralizationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsDataContractGeneralizationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DataContractGeneralizationSourceEndModel.SpecializationTypeId;
        }

        public static DataContractGeneralizationSourceEndModel AsDataContractGeneralizationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsDataContractGeneralizationSourceEndModel() ? new DataContractGeneralizationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}