using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Types.ServiceProxies.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CallExternalEndpointModel : IMetadataModel
    {
        public const string SpecializationType = "Call External Endpoint";
        public const string SpecializationTypeId = "6412d45e-71db-47e9-8832-b3b9b9b5fe00";
        protected readonly IAssociation _association;
        protected CallExternalEndpointSourceEndModel _sourceEnd;
        protected CallExternalEndpointTargetEndModel _targetEnd;

        public CallExternalEndpointModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CallExternalEndpointModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CallExternalEndpointModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CallExternalEndpointSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CallExternalEndpointSourceEndModel(_association.SourceEnd, this));

        public CallExternalEndpointTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CallExternalEndpointTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CallExternalEndpointModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallExternalEndpointModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallExternalEndpointSourceEndModel : CallExternalEndpointEndModel
    {
        public const string SpecializationTypeId = "9461da60-870b-4fa3-a41a-6ca79c3fe1fe";
        public const string SpecializationType = "Call External Endpoint Source End";

        public CallExternalEndpointSourceEndModel(IAssociationEnd associationEnd, CallExternalEndpointModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallExternalEndpointTargetEndModel : CallExternalEndpointEndModel
    {
        public const string SpecializationTypeId = "90fa6c63-5d1c-4fae-94b3-f93865373cab";
        public const string SpecializationType = "Call External Endpoint Target End";

        public CallExternalEndpointTargetEndModel(IAssociationEnd associationEnd, CallExternalEndpointModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class CallExternalEndpointEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CallExternalEndpointModel _association;

        public CallExternalEndpointEndModel(IAssociationEnd associationEnd, CallExternalEndpointModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CallExternalEndpointEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CallExternalEndpointModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CallExternalEndpointEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CallExternalEndpointModel Association => _association;
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

        public CallExternalEndpointEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CallExternalEndpointEndModel)_association.TargetEnd : (CallExternalEndpointEndModel)_association.SourceEnd;
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

        public bool Equals(CallExternalEndpointEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallExternalEndpointEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CallExternalEndpointEndModelExtensions
    {
        public static bool IsCallExternalEndpointEndModel(this ICanBeReferencedType type)
        {
            return IsCallExternalEndpointTargetEndModel(type) || IsCallExternalEndpointSourceEndModel(type);
        }

        public static CallExternalEndpointEndModel AsCallExternalEndpointEndModel(this ICanBeReferencedType type)
        {
            return (CallExternalEndpointEndModel)type.AsCallExternalEndpointTargetEndModel() ?? (CallExternalEndpointEndModel)type.AsCallExternalEndpointSourceEndModel();
        }

        public static bool IsCallExternalEndpointTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallExternalEndpointTargetEndModel.SpecializationTypeId;
        }

        public static CallExternalEndpointTargetEndModel AsCallExternalEndpointTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallExternalEndpointTargetEndModel() ? new CallExternalEndpointModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCallExternalEndpointSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallExternalEndpointSourceEndModel.SpecializationTypeId;
        }

        public static CallExternalEndpointSourceEndModel AsCallExternalEndpointSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallExternalEndpointSourceEndModel() ? new CallExternalEndpointModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}