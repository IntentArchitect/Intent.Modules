using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ServiceInvocationModel : IMetadataModel
    {
        public const string SpecializationType = "Service Invocation";
        public const string SpecializationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";
        protected readonly IAssociation _association;
        protected ServiceInvocationSourceEndModel _sourceEnd;
        protected ServiceInvocationTargetEndModel _targetEnd;

        public ServiceInvocationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static ServiceInvocationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new ServiceInvocationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public ServiceInvocationSourceEndModel SourceEnd => _sourceEnd ??= new ServiceInvocationSourceEndModel(_association.SourceEnd, this);

        public ServiceInvocationTargetEndModel TargetEnd => _targetEnd ??= new ServiceInvocationTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(ServiceInvocationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceInvocationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ServiceInvocationSourceEndModel : ServiceInvocationEndModel
    {
        public const string SpecializationTypeId = "ee56bd48-8eff-4fff-8d3a-87731d002335";
        public const string SpecializationType = "Service Invocation Source End";

        public ServiceInvocationSourceEndModel(IAssociationEnd associationEnd, ServiceInvocationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ServiceInvocationTargetEndModel : ServiceInvocationEndModel, IProcessingActionModel
    {
        public const string SpecializationTypeId = "093e5909-ffe4-4510-b3ea-532f30212f3c";
        public const string SpecializationType = "Service Invocation Target End";

        public ServiceInvocationTargetEndModel(IAssociationEnd associationEnd, ServiceInvocationModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class ServiceInvocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly ServiceInvocationModel _association;

        public ServiceInvocationEndModel(IAssociationEnd associationEnd, ServiceInvocationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static ServiceInvocationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new ServiceInvocationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (ServiceInvocationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public ServiceInvocationModel Association => _association;
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

        public ServiceInvocationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (ServiceInvocationEndModel)_association.TargetEnd : (ServiceInvocationEndModel)_association.SourceEnd;
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

        public bool Equals(ServiceInvocationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceInvocationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ServiceInvocationEndModelExtensions
    {
        public static bool IsServiceInvocationEndModel(this ICanBeReferencedType type)
        {
            return IsServiceInvocationTargetEndModel(type) || IsServiceInvocationSourceEndModel(type);
        }

        public static ServiceInvocationEndModel AsServiceInvocationEndModel(this ICanBeReferencedType type)
        {
            return (ServiceInvocationEndModel)type.AsServiceInvocationTargetEndModel() ?? (ServiceInvocationEndModel)type.AsServiceInvocationSourceEndModel();
        }

        public static bool IsServiceInvocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ServiceInvocationTargetEndModel.SpecializationTypeId;
        }

        public static ServiceInvocationTargetEndModel AsServiceInvocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsServiceInvocationTargetEndModel() ? new ServiceInvocationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsServiceInvocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ServiceInvocationSourceEndModel.SpecializationTypeId;
        }

        public static ServiceInvocationSourceEndModel AsServiceInvocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsServiceInvocationSourceEndModel() ? new ServiceInvocationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}