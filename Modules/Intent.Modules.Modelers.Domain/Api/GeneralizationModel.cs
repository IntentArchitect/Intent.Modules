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
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GeneralizationModel : IMetadataModel
    {
        public const string SpecializationType = "Generalization";
        protected readonly IAssociation _association;
        protected GeneralizationSourceEndModel _sourceEnd;
        protected GeneralizationTargetEndModel _targetEnd;

        public GeneralizationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static GeneralizationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new GeneralizationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public GeneralizationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new GeneralizationSourceEndModel(_association.SourceEnd, this));

        public GeneralizationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new GeneralizationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(GeneralizationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneralizationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
        public const string SpecializationTypeId = "5de35973-3ac7-4e65-b48c-385605aec561";
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;

        public GeneralizationEndModel(IAssociationEnd associationEnd, GeneralizationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public string Id => _associationEnd.Id;
        public string Name => _associationEnd.Name;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public bool Equals(GeneralizationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneralizationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }

        private readonly GeneralizationModel _association;
        public GeneralizationModel Association => _association;
        public IElement InternalElement => _associationEnd;
        public string Comment => _associationEnd.Comment;
        public ICanBeReferencedType Element => _associationEnd.TypeReference.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.TypeReference.GenericTypeParameters;
        public bool IsCollection => _associationEnd.TypeReference.IsCollection;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.TypeReference.IsNullable;

        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        public GeneralizationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (GeneralizationEndModel)_association.TargetEnd : (GeneralizationEndModel)_association.SourceEnd;
        }

        public override string ToString()
        {
            return _associationEnd.ToString();
        }

        public IAssociation InternalAssociation => _association.InternalAssociation;

        public IAssociationEnd InternalAssociationEnd => _associationEnd;

        public IPackage Package => Element?.Package;

        public string SpecializationType => _associationEnd.SpecializationType;

        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;

        public ITypeReference TypeReference => this;

        public static GeneralizationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new GeneralizationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (GeneralizationEndModel)association.TargetEnd : association.SourceEnd;
        }
    }

    [IntentManaged(Mode.Fully)]
    public class GeneralizationSourceEndModel : GeneralizationEndModel
    {
        public const string SpecializationTypeId = "8190bf43-222c-4b53-8a44-14626efe3574";

        public GeneralizationSourceEndModel(IAssociationEnd associationEnd, GeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class GeneralizationTargetEndModel : GeneralizationEndModel
    {
        public const string SpecializationTypeId = "4686cc1d-b4d8-4b99-b45b-f77bd5496946";

        public GeneralizationTargetEndModel(IAssociationEnd associationEnd, GeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GeneralizationEndModelExtensions
    {
        public static bool IsGeneralizationEndModel(this ICanBeReferencedType type)
        {
            return IsGeneralizationTargetEndModel(type) || IsGeneralizationSourceEndModel(type);
        }

        public static GeneralizationEndModel AsGeneralizationEndModel(this ICanBeReferencedType type)
        {
            return (GeneralizationEndModel)type.AsGeneralizationTargetEndModel() ?? (GeneralizationEndModel)type.AsGeneralizationSourceEndModel();
        }

        public static bool IsGeneralizationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == GeneralizationTargetEndModel.SpecializationTypeId;
        }

        public static GeneralizationTargetEndModel AsGeneralizationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsGeneralizationTargetEndModel() ? new GeneralizationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsGeneralizationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == GeneralizationSourceEndModel.SpecializationTypeId;
        }

        public static GeneralizationSourceEndModel AsGeneralizationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsGeneralizationSourceEndModel() ? new GeneralizationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}