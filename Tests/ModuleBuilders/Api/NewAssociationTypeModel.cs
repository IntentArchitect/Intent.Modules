using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace ModuleBuilders.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class NewAssociationTypeModel : IMetadataModel
    {
        public const string SpecializationType = "NewAssociationType";
        public const string SpecializationTypeId = "6ba7c27e-608b-4e97-8b85-e108719e2a88";
        protected readonly IAssociation _association;
        protected NewAssociationTypeSourceEndModel _sourceEnd;
        protected NewAssociationTypeTargetEndModel _targetEnd;

        public NewAssociationTypeModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static NewAssociationTypeModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new NewAssociationTypeModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public NewAssociationTypeSourceEndModel SourceEnd => _sourceEnd ??= new NewAssociationTypeSourceEndModel(_association.SourceEnd, this);

        public NewAssociationTypeTargetEndModel TargetEnd => _targetEnd ??= new NewAssociationTypeTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(NewAssociationTypeModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NewAssociationTypeModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class NewAssociationTypeSourceEndModel : NewAssociationTypeEndModel
    {
        public const string SpecializationTypeId = "65ec2a3f-c95d-4e7a-8a38-a657a5bc16d5";
        public const string SpecializationType = "NewAssociationType Source End";

        public NewAssociationTypeSourceEndModel(IAssociationEnd associationEnd, NewAssociationTypeModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class NewAssociationTypeTargetEndModel : NewAssociationTypeEndModel
    {
        public const string SpecializationTypeId = "43fb6968-f23c-436b-8ced-e340d76f2586";
        public const string SpecializationType = "NewAssociationType Target End";

        public NewAssociationTypeTargetEndModel(IAssociationEnd associationEnd, NewAssociationTypeModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class NewAssociationTypeEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly NewAssociationTypeModel _association;

        public NewAssociationTypeEndModel(IAssociationEnd associationEnd, NewAssociationTypeModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static NewAssociationTypeEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new NewAssociationTypeModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (NewAssociationTypeEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public NewAssociationTypeModel Association => _association;
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

        public NewAssociationTypeEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (NewAssociationTypeEndModel)_association.TargetEnd : (NewAssociationTypeEndModel)_association.SourceEnd;
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

        public bool Equals(NewAssociationTypeEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NewAssociationTypeEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class NewAssociationTypeEndModelExtensions
    {
        public static bool IsNewAssociationTypeEndModel(this ICanBeReferencedType type)
        {
            return IsNewAssociationTypeTargetEndModel(type) || IsNewAssociationTypeSourceEndModel(type);
        }

        public static NewAssociationTypeEndModel AsNewAssociationTypeEndModel(this ICanBeReferencedType type)
        {
            return (NewAssociationTypeEndModel)type.AsNewAssociationTypeTargetEndModel() ?? (NewAssociationTypeEndModel)type.AsNewAssociationTypeSourceEndModel();
        }

        public static bool IsNewAssociationTypeTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == NewAssociationTypeTargetEndModel.SpecializationTypeId;
        }

        public static NewAssociationTypeTargetEndModel AsNewAssociationTypeTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsNewAssociationTypeTargetEndModel() ? new NewAssociationTypeModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsNewAssociationTypeSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == NewAssociationTypeSourceEndModel.SpecializationTypeId;
        }

        public static NewAssociationTypeSourceEndModel AsNewAssociationTypeSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsNewAssociationTypeSourceEndModel() ? new NewAssociationTypeModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}