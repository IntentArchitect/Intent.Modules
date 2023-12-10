using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CallServiceOperationModel : IMetadataModel
    {
        public const string SpecializationType = "Call Service Operation";
        public const string SpecializationTypeId = "9510ff76-fba3-4eca-a5dd-0cfefc8f5bb6";
        protected readonly IAssociation _association;
        protected CallServiceOperationSourceEndModel _sourceEnd;
        protected CallServiceOperationTargetEndModel _targetEnd;

        public CallServiceOperationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CallServiceOperationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CallServiceOperationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CallServiceOperationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new CallServiceOperationSourceEndModel(_association.SourceEnd, this));

        public CallServiceOperationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new CallServiceOperationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CallServiceOperationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallServiceOperationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallServiceOperationSourceEndModel : CallServiceOperationEndModel
    {
        public const string SpecializationTypeId = "01693f59-b89f-4e03-934a-dfcb4cbd90df";
        public const string SpecializationType = "Call Service Operation Source End";

        public CallServiceOperationSourceEndModel(IAssociationEnd associationEnd, CallServiceOperationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CallServiceOperationTargetEndModel : CallServiceOperationEndModel
    {
        public const string SpecializationTypeId = "752c289a-5961-4172-9017-0eca6fa09fd9";
        public const string SpecializationType = "Call Service Operation Target End";

        public CallServiceOperationTargetEndModel(IAssociationEnd associationEnd, CallServiceOperationModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class CallServiceOperationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CallServiceOperationModel _association;

        public CallServiceOperationEndModel(IAssociationEnd associationEnd, CallServiceOperationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CallServiceOperationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CallServiceOperationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CallServiceOperationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CallServiceOperationModel Association => _association;
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

        public CallServiceOperationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CallServiceOperationEndModel)_association.TargetEnd : (CallServiceOperationEndModel)_association.SourceEnd;
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

        public bool Equals(CallServiceOperationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CallServiceOperationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CallServiceOperationEndModelExtensions
    {
        public static bool IsCallServiceOperationEndModel(this ICanBeReferencedType type)
        {
            return IsCallServiceOperationTargetEndModel(type) || IsCallServiceOperationSourceEndModel(type);
        }

        public static CallServiceOperationEndModel AsCallServiceOperationEndModel(this ICanBeReferencedType type)
        {
            return (CallServiceOperationEndModel)type.AsCallServiceOperationTargetEndModel() ?? (CallServiceOperationEndModel)type.AsCallServiceOperationSourceEndModel();
        }

        public static bool IsCallServiceOperationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallServiceOperationTargetEndModel.SpecializationTypeId;
        }

        public static CallServiceOperationTargetEndModel AsCallServiceOperationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallServiceOperationTargetEndModel() ? new CallServiceOperationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCallServiceOperationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CallServiceOperationSourceEndModel.SpecializationTypeId;
        }

        public static CallServiceOperationSourceEndModel AsCallServiceOperationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCallServiceOperationSourceEndModel() ? new CallServiceOperationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}