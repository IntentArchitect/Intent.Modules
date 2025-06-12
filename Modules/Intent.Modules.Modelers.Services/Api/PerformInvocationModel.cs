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
    public class PerformInvocationModel : IMetadataModel
    {
        public const string SpecializationType = "Perform Invocation";
        public const string SpecializationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";
        protected readonly IAssociation _association;
        protected PerformInvocationSourceEndModel _sourceEnd;
        protected PerformInvocationTargetEndModel _targetEnd;

        public PerformInvocationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static PerformInvocationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new PerformInvocationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public PerformInvocationSourceEndModel SourceEnd => _sourceEnd ??= new PerformInvocationSourceEndModel(_association.SourceEnd, this);

        public PerformInvocationTargetEndModel TargetEnd => _targetEnd ??= new PerformInvocationTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(PerformInvocationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PerformInvocationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class PerformInvocationSourceEndModel : PerformInvocationEndModel
    {
        public const string SpecializationTypeId = "ee56bd48-8eff-4fff-8d3a-87731d002335";
        public const string SpecializationType = "Perform Invocation Source End";

        public PerformInvocationSourceEndModel(IAssociationEnd associationEnd, PerformInvocationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class PerformInvocationTargetEndModel : PerformInvocationEndModel, IProcessingActionModel
    {
        public const string SpecializationTypeId = "093e5909-ffe4-4510-b3ea-532f30212f3c";
        public const string SpecializationType = "Perform Invocation Target End";

        public PerformInvocationTargetEndModel(IAssociationEnd associationEnd, PerformInvocationModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class PerformInvocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly PerformInvocationModel _association;

        public PerformInvocationEndModel(IAssociationEnd associationEnd, PerformInvocationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static PerformInvocationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new PerformInvocationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (PerformInvocationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public PerformInvocationModel Association => _association;
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

        public PerformInvocationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (PerformInvocationEndModel)_association.TargetEnd : (PerformInvocationEndModel)_association.SourceEnd;
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

        public bool Equals(PerformInvocationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PerformInvocationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class PerformInvocationEndModelExtensions
    {
        public static bool IsPerformInvocationEndModel(this ICanBeReferencedType type)
        {
            return IsPerformInvocationTargetEndModel(type) || IsPerformInvocationSourceEndModel(type);
        }

        public static PerformInvocationEndModel AsPerformInvocationEndModel(this ICanBeReferencedType type)
        {
            return (PerformInvocationEndModel)type.AsPerformInvocationTargetEndModel() ?? (PerformInvocationEndModel)type.AsPerformInvocationSourceEndModel();
        }

        public static bool IsPerformInvocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == PerformInvocationTargetEndModel.SpecializationTypeId;
        }

        public static PerformInvocationTargetEndModel AsPerformInvocationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsPerformInvocationTargetEndModel() ? new PerformInvocationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsPerformInvocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == PerformInvocationSourceEndModel.SpecializationTypeId;
        }

        public static PerformInvocationSourceEndModel AsPerformInvocationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsPerformInvocationSourceEndModel() ? new PerformInvocationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}