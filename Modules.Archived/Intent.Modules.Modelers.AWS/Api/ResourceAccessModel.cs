using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ResourceAccessModel : IMetadataModel
    {
        public const string SpecializationType = "Resource Access";
        public const string SpecializationTypeId = "d3fe616a-e00b-4553-aaa2-41b626e58faa";
        protected readonly IAssociation _association;
        protected ResourceAccessSourceEndModel _sourceEnd;
        protected ResourceAccessTargetEndModel _targetEnd;

        public ResourceAccessModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static ResourceAccessModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new ResourceAccessModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public ResourceAccessSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new ResourceAccessSourceEndModel(_association.SourceEnd, this));

        public ResourceAccessTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new ResourceAccessTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(ResourceAccessModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResourceAccessModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ResourceAccessSourceEndModel : ResourceAccessEndModel
    {
        public const string SpecializationTypeId = "03ee1a03-b5f1-405c-96b5-4e3369b86ca4";

        public ResourceAccessSourceEndModel(IAssociationEnd associationEnd, ResourceAccessModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ResourceAccessTargetEndModel : ResourceAccessEndModel
    {
        public const string SpecializationTypeId = "afeea9e0-5fe1-4f7f-a302-db9b6d61ec44";

        public ResourceAccessTargetEndModel(IAssociationEnd associationEnd, ResourceAccessModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ResourceAccessEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly ResourceAccessModel _association;

        public ResourceAccessEndModel(IAssociationEnd associationEnd, ResourceAccessModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static ResourceAccessEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new ResourceAccessModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (ResourceAccessEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public ResourceAccessModel Association => _association;
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

        public ResourceAccessEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (ResourceAccessEndModel)_association.TargetEnd : (ResourceAccessEndModel)_association.SourceEnd;
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

        public bool Equals(ResourceAccessEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResourceAccessEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ResourceAccessEndModelExtensions
    {
        public static bool IsResourceAccessEndModel(this ICanBeReferencedType type)
        {
            return IsResourceAccessTargetEndModel(type) || IsResourceAccessSourceEndModel(type);
        }

        public static ResourceAccessEndModel AsResourceAccessEndModel(this ICanBeReferencedType type)
        {
            return (ResourceAccessEndModel)type.AsResourceAccessTargetEndModel() ?? (ResourceAccessEndModel)type.AsResourceAccessSourceEndModel();
        }

        public static bool IsResourceAccessTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ResourceAccessTargetEndModel.SpecializationTypeId;
        }

        public static ResourceAccessTargetEndModel AsResourceAccessTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsResourceAccessTargetEndModel() ? new ResourceAccessModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsResourceAccessSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ResourceAccessSourceEndModel.SpecializationTypeId;
        }

        public static ResourceAccessSourceEndModel AsResourceAccessSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsResourceAccessSourceEndModel() ? new ResourceAccessModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}