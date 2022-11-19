using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ResourceAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Resource Association";
        public const string SpecializationTypeId = "eb4520aa-5fe2-4f03-a502-0489d1ac4177";
        protected readonly IAssociation _association;
        protected ResourceAssociationSourceEndModel _sourceEnd;
        protected ResourceAssociationTargetEndModel _targetEnd;

        public ResourceAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static ResourceAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new ResourceAssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public ResourceAssociationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new ResourceAssociationSourceEndModel(_association.SourceEnd, this));

        public ResourceAssociationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new ResourceAssociationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(ResourceAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResourceAssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ResourceAssociationSourceEndModel : ResourceAssociationEndModel
    {
        public const string SpecializationTypeId = "cce2e9bf-bdfe-4d61-812f-fb9c5bf48196";

        public ResourceAssociationSourceEndModel(IAssociationEnd associationEnd, ResourceAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ResourceAssociationTargetEndModel : ResourceAssociationEndModel
    {
        public const string SpecializationTypeId = "640b90f9-26b9-44d5-85c6-6f977acc0de8";

        public ResourceAssociationTargetEndModel(IAssociationEnd associationEnd, ResourceAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ResourceAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly ResourceAssociationModel _association;

        public ResourceAssociationEndModel(IAssociationEnd associationEnd, ResourceAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static ResourceAssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new ResourceAssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (ResourceAssociationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public ResourceAssociationModel Association => _association;
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

        public ResourceAssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (ResourceAssociationEndModel)_association.TargetEnd : (ResourceAssociationEndModel)_association.SourceEnd;
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

        public bool Equals(ResourceAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResourceAssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ResourceAssociationEndModelExtensions
    {
        public static bool IsResourceAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsResourceAssociationTargetEndModel(type) || IsResourceAssociationSourceEndModel(type);
        }

        public static ResourceAssociationEndModel AsResourceAssociationEndModel(this ICanBeReferencedType type)
        {
            return (ResourceAssociationEndModel)type.AsResourceAssociationTargetEndModel() ?? (ResourceAssociationEndModel)type.AsResourceAssociationSourceEndModel();
        }

        public static bool IsResourceAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ResourceAssociationTargetEndModel.SpecializationTypeId;
        }

        public static ResourceAssociationTargetEndModel AsResourceAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsResourceAssociationTargetEndModel() ? new ResourceAssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsResourceAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ResourceAssociationSourceEndModel.SpecializationTypeId;
        }

        public static ResourceAssociationSourceEndModel AsResourceAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsResourceAssociationSourceEndModel() ? new ResourceAssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}