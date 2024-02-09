using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CallServiceOperationActionModel : IMetadataModel
    {
        public const string SpecializationType = "Call Service Operation Action";
        public const string SpecializationTypeId = "fe5a5cd8-aabd-472f-8d42-f5c233e658dc";
        protected readonly IAssociation _association;
        protected CallServiceOperationActionSourceEndModel _sourceEnd;
        protected CallServiceOperationActionTargetEndModel _targetEnd;

        public CallServiceOperationActionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CallServiceOperationActionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CallServiceOperationActionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CallServiceOperationActionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CallServiceOperationActionSourceEndModel(_association.SourceEnd, this));

        public CallServiceOperationActionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CallServiceOperationActionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CallServiceOperationActionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallServiceOperationActionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallServiceOperationActionSourceEndModel : CallServiceOperationActionEndModel
    {
        public const string SpecializationTypeId = "936e090c-8408-429d-b2f6-2eb8deecc428";
        public const string SpecializationType = "Call Service Operation Action Source End";

        public CallServiceOperationActionSourceEndModel(IAssociationEnd associationEnd, CallServiceOperationActionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallServiceOperationActionTargetEndModel : CallServiceOperationActionEndModel
    {
        public const string SpecializationTypeId = "475f0810-2b4a-40da-8eb8-697cb62f7dbe";
        public const string SpecializationType = "Call Service Operation Action Target End";

        public CallServiceOperationActionTargetEndModel(IAssociationEnd associationEnd, CallServiceOperationActionModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class CallServiceOperationActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CallServiceOperationActionModel _association;

        public CallServiceOperationActionEndModel(IAssociationEnd associationEnd, CallServiceOperationActionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CallServiceOperationActionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CallServiceOperationActionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CallServiceOperationActionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CallServiceOperationActionModel Association => _association;
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

        public CallServiceOperationActionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CallServiceOperationActionEndModel)_association.TargetEnd : (CallServiceOperationActionEndModel)_association.SourceEnd;
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

        public bool Equals(CallServiceOperationActionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallServiceOperationActionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CallServiceOperationActionEndModelExtensions
    {
        public static bool IsCallServiceOperationActionEndModel(this ICanBeReferencedType type)
        {
            return IsCallServiceOperationActionTargetEndModel(type) || IsCallServiceOperationActionSourceEndModel(type);
        }

        public static CallServiceOperationActionEndModel AsCallServiceOperationActionEndModel(this ICanBeReferencedType type)
        {
            return (CallServiceOperationActionEndModel)type.AsCallServiceOperationActionTargetEndModel() ?? (CallServiceOperationActionEndModel)type.AsCallServiceOperationActionSourceEndModel();
        }

        public static bool IsCallServiceOperationActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallServiceOperationActionTargetEndModel.SpecializationTypeId;
        }

        public static CallServiceOperationActionTargetEndModel AsCallServiceOperationActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallServiceOperationActionTargetEndModel() ? new CallServiceOperationActionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCallServiceOperationActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallServiceOperationActionSourceEndModel.SpecializationTypeId;
        }

        public static CallServiceOperationActionSourceEndModel AsCallServiceOperationActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallServiceOperationActionSourceEndModel() ? new CallServiceOperationActionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}