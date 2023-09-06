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
    public class SendCommandModel : IMetadataModel
    {
        public const string SpecializationType = "Send Command";
        public const string SpecializationTypeId = "38a3de5a-ca88-4f6e-88b9-88e5953936b2";
        protected readonly IAssociation _association;
        protected SendCommandSourceEndModel _sourceEnd;
        protected SendCommandTargetEndModel _targetEnd;

        public SendCommandModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static SendCommandModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new SendCommandModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public SendCommandSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new SendCommandSourceEndModel(_association.SourceEnd, this));

        public SendCommandTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new SendCommandTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(SendCommandModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SendCommandModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SendCommandSourceEndModel : SendCommandEndModel
    {
        public const string SpecializationTypeId = "6b9e34dd-b50f-4998-a3cf-93dde7b2d51e";

        public SendCommandSourceEndModel(IAssociationEnd associationEnd, SendCommandModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class SendCommandTargetEndModel : SendCommandEndModel
    {
        public const string SpecializationTypeId = "d3096261-1268-440f-8db3-0a6b8b4786cc";

        public SendCommandTargetEndModel(IAssociationEnd associationEnd, SendCommandModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class SendCommandEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly SendCommandModel _association;

        public SendCommandEndModel(IAssociationEnd associationEnd, SendCommandModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static SendCommandEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new SendCommandModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (SendCommandEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public SendCommandModel Association => _association;
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

        public SendCommandEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (SendCommandEndModel)_association.TargetEnd : (SendCommandEndModel)_association.SourceEnd;
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

        public bool Equals(SendCommandEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SendCommandEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SendCommandEndModelExtensions
    {
        public static bool IsSendCommandEndModel(this ICanBeReferencedType type)
        {
            return IsSendCommandTargetEndModel(type) || IsSendCommandSourceEndModel(type);
        }

        public static SendCommandEndModel AsSendCommandEndModel(this ICanBeReferencedType type)
        {
            return (SendCommandEndModel)type.AsSendCommandTargetEndModel() ?? (SendCommandEndModel)type.AsSendCommandSourceEndModel();
        }

        public static bool IsSendCommandTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SendCommandTargetEndModel.SpecializationTypeId;
        }

        public static SendCommandTargetEndModel AsSendCommandTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsSendCommandTargetEndModel() ? new SendCommandModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsSendCommandSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == SendCommandSourceEndModel.SpecializationTypeId;
        }

        public static SendCommandSourceEndModel AsSendCommandSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsSendCommandSourceEndModel() ? new SendCommandModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}