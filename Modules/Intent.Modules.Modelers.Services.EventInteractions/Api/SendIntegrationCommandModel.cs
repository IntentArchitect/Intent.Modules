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
    public class SendIntegrationCommandModel : IMetadataModel
    {
        public const string SpecializationType = "Send Integration Command";
        public const string SpecializationTypeId = "389a7478-a8f1-4acc-adff-a73ce4aa7e6d";
        protected readonly IAssociation _association;
        protected SendIntegrationCommandSourceEndModel _sourceEnd;
        protected SendIntegrationCommandTargetEndModel _targetEnd;

        public SendIntegrationCommandModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static SendIntegrationCommandModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new SendIntegrationCommandModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public SendIntegrationCommandSourceEndModel SourceEnd => _sourceEnd ??= new SendIntegrationCommandSourceEndModel(_association.SourceEnd, this);

        public SendIntegrationCommandTargetEndModel TargetEnd => _targetEnd ??= new SendIntegrationCommandTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(SendIntegrationCommandModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SendIntegrationCommandModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SendIntegrationCommandSourceEndModel : SendIntegrationCommandEndModel
    {
        public const string SpecializationTypeId = "c5f4f98f-e464-48de-b202-c0724bacebb7";
        public const string SpecializationType = "Send Integration Command Source End";

        public SendIntegrationCommandSourceEndModel(IAssociationEnd associationEnd, SendIntegrationCommandModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SendIntegrationCommandTargetEndModel : SendIntegrationCommandEndModel, IProcessingActionModel
    {
        public const string SpecializationTypeId = "35a14f76-71e0-45f2-a17f-f8d1483510f7";
        public const string SpecializationType = "Send Integration Command Target End";

        public SendIntegrationCommandTargetEndModel(IAssociationEnd associationEnd, SendIntegrationCommandModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class SendIntegrationCommandEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly SendIntegrationCommandModel _association;

        public SendIntegrationCommandEndModel(IAssociationEnd associationEnd, SendIntegrationCommandModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static SendIntegrationCommandEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new SendIntegrationCommandModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (SendIntegrationCommandEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public SendIntegrationCommandModel Association => _association;
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

        public SendIntegrationCommandEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (SendIntegrationCommandEndModel)_association.TargetEnd : (SendIntegrationCommandEndModel)_association.SourceEnd;
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

        public bool Equals(SendIntegrationCommandEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SendIntegrationCommandEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SendIntegrationCommandEndModelExtensions
    {
        public static bool IsSendIntegrationCommandEndModel(this ICanBeReferencedType type)
        {
            return IsSendIntegrationCommandTargetEndModel(type) || IsSendIntegrationCommandSourceEndModel(type);
        }

        public static SendIntegrationCommandEndModel AsSendIntegrationCommandEndModel(this ICanBeReferencedType type)
        {
            return (SendIntegrationCommandEndModel)type.AsSendIntegrationCommandTargetEndModel() ?? (SendIntegrationCommandEndModel)type.AsSendIntegrationCommandSourceEndModel();
        }

        public static bool IsSendIntegrationCommandTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SendIntegrationCommandTargetEndModel.SpecializationTypeId;
        }

        public static SendIntegrationCommandTargetEndModel AsSendIntegrationCommandTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsSendIntegrationCommandTargetEndModel() ? new SendIntegrationCommandModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsSendIntegrationCommandSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SendIntegrationCommandSourceEndModel.SpecializationTypeId;
        }

        public static SendIntegrationCommandSourceEndModel AsSendIntegrationCommandSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsSendIntegrationCommandSourceEndModel() ? new SendIntegrationCommandModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}