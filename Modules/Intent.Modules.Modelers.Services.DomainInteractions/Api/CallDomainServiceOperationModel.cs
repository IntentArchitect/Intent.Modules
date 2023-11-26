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
    public class CallDomainServiceOperationModel : IMetadataModel
    {
        public const string SpecializationType = "Call Domain Service Operation";
        public const string SpecializationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";
        protected readonly IAssociation _association;
        protected CallDomainServiceOperationSourceEndModel _sourceEnd;
        protected CallDomainServiceOperationTargetEndModel _targetEnd;

        public CallDomainServiceOperationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CallDomainServiceOperationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CallDomainServiceOperationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CallDomainServiceOperationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CallDomainServiceOperationSourceEndModel(_association.SourceEnd, this));

        public CallDomainServiceOperationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CallDomainServiceOperationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CallDomainServiceOperationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallDomainServiceOperationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallDomainServiceOperationSourceEndModel : CallDomainServiceOperationEndModel
    {
        public const string SpecializationTypeId = "ee56bd48-8eff-4fff-8d3a-87731d002335";
        public const string SpecializationType = "Call Domain Service Operation Source End";

        public CallDomainServiceOperationSourceEndModel(IAssociationEnd associationEnd, CallDomainServiceOperationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallDomainServiceOperationTargetEndModel : CallDomainServiceOperationEndModel
    {
        public const string SpecializationTypeId = "093e5909-ffe4-4510-b3ea-532f30212f3c";
        public const string SpecializationType = "Call Domain Service Operation Target End";

        public CallDomainServiceOperationTargetEndModel(IAssociationEnd associationEnd, CallDomainServiceOperationModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class CallDomainServiceOperationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CallDomainServiceOperationModel _association;

        public CallDomainServiceOperationEndModel(IAssociationEnd associationEnd, CallDomainServiceOperationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CallDomainServiceOperationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CallDomainServiceOperationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CallDomainServiceOperationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CallDomainServiceOperationModel Association => _association;
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

        public CallDomainServiceOperationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CallDomainServiceOperationEndModel)_association.TargetEnd : (CallDomainServiceOperationEndModel)_association.SourceEnd;
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

        public bool Equals(CallDomainServiceOperationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallDomainServiceOperationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CallDomainServiceOperationEndModelExtensions
    {
        public static bool IsCallDomainServiceOperationEndModel(this ICanBeReferencedType type)
        {
            return IsCallDomainServiceOperationTargetEndModel(type) || IsCallDomainServiceOperationSourceEndModel(type);
        }

        public static CallDomainServiceOperationEndModel AsCallDomainServiceOperationEndModel(this ICanBeReferencedType type)
        {
            return (CallDomainServiceOperationEndModel)type.AsCallDomainServiceOperationTargetEndModel() ?? (CallDomainServiceOperationEndModel)type.AsCallDomainServiceOperationSourceEndModel();
        }

        public static bool IsCallDomainServiceOperationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallDomainServiceOperationTargetEndModel.SpecializationTypeId;
        }

        public static CallDomainServiceOperationTargetEndModel AsCallDomainServiceOperationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallDomainServiceOperationTargetEndModel() ? new CallDomainServiceOperationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCallDomainServiceOperationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallDomainServiceOperationSourceEndModel.SpecializationTypeId;
        }

        public static CallDomainServiceOperationSourceEndModel AsCallDomainServiceOperationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallDomainServiceOperationSourceEndModel() ? new CallDomainServiceOperationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}