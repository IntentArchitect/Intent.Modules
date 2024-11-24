using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DomainEventHandlerAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Domain Event Handler Association";
        public const string SpecializationTypeId = "90831494-f069-44eb-b488-ab2dba7518ea";
        protected readonly IAssociation _association;
        protected DomainEventHandlerAssociationSourceEndModel _sourceEnd;
        protected DomainEventHandlerAssociationTargetEndModel _targetEnd;

        public DomainEventHandlerAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static DomainEventHandlerAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new DomainEventHandlerAssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public DomainEventHandlerAssociationSourceEndModel SourceEnd => _sourceEnd ??= new DomainEventHandlerAssociationSourceEndModel(_association.SourceEnd, this);

        public DomainEventHandlerAssociationTargetEndModel TargetEnd => _targetEnd ??= new DomainEventHandlerAssociationTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(DomainEventHandlerAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventHandlerAssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventHandlerAssociationSourceEndModel : DomainEventHandlerAssociationEndModel
    {
        public const string SpecializationTypeId = "79f048d4-4c09-4405-be8f-95473a981556";
        public const string SpecializationType = "Domain Event Handler Association Source End";

        public DomainEventHandlerAssociationSourceEndModel(IAssociationEnd associationEnd, DomainEventHandlerAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DomainEventHandlerAssociationTargetEndModel : DomainEventHandlerAssociationEndModel
    {
        public const string SpecializationTypeId = "f45dfee9-f62b-45ac-bfce-a3878e04b73f";
        public const string SpecializationType = "Domain Event Handler Association Target End";

        public DomainEventHandlerAssociationTargetEndModel(IAssociationEnd associationEnd, DomainEventHandlerAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public class DomainEventHandlerAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper, IProcessingHandlerModel
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly DomainEventHandlerAssociationModel _association;

        public DomainEventHandlerAssociationEndModel(IAssociationEnd associationEnd, DomainEventHandlerAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static DomainEventHandlerAssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new DomainEventHandlerAssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (DomainEventHandlerAssociationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public DomainEventHandlerAssociationModel Association => _association;
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

        public DomainEventHandlerAssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (DomainEventHandlerAssociationEndModel)_association.TargetEnd : (DomainEventHandlerAssociationEndModel)_association.SourceEnd;
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

        public bool Equals(DomainEventHandlerAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventHandlerAssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DomainEventHandlerAssociationEndModelExtensions
    {
        public static bool IsDomainEventHandlerAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsDomainEventHandlerAssociationTargetEndModel(type) || IsDomainEventHandlerAssociationSourceEndModel(type);
        }

        public static DomainEventHandlerAssociationEndModel AsDomainEventHandlerAssociationEndModel(this ICanBeReferencedType type)
        {
            return (DomainEventHandlerAssociationEndModel)type.AsDomainEventHandlerAssociationTargetEndModel() ?? (DomainEventHandlerAssociationEndModel)type.AsDomainEventHandlerAssociationSourceEndModel();
        }

        public static bool IsDomainEventHandlerAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventHandlerAssociationTargetEndModel.SpecializationTypeId;
        }

        public static DomainEventHandlerAssociationTargetEndModel AsDomainEventHandlerAssociationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventHandlerAssociationTargetEndModel() ? new DomainEventHandlerAssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsDomainEventHandlerAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DomainEventHandlerAssociationSourceEndModel.SpecializationTypeId;
        }

        public static DomainEventHandlerAssociationSourceEndModel AsDomainEventHandlerAssociationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventHandlerAssociationSourceEndModel() ? new DomainEventHandlerAssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}