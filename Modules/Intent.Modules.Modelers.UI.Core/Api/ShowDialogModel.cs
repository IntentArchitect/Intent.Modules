using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ShowDialogModel : IMetadataModel
    {
        public const string SpecializationType = "Show Dialog";
        public const string SpecializationTypeId = "2a309fb2-c487-4c00-9462-16dac9824731";
        protected readonly IAssociation _association;
        protected ShowDialogSourceEndModel _sourceEnd;
        protected ShowDialogTargetEndModel _targetEnd;

        public ShowDialogModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static ShowDialogModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new ShowDialogModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public ShowDialogSourceEndModel SourceEnd => _sourceEnd ??= new ShowDialogSourceEndModel(_association.SourceEnd, this);

        public ShowDialogTargetEndModel TargetEnd => _targetEnd ??= new ShowDialogTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(ShowDialogModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ShowDialogModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ShowDialogSourceEndModel : ShowDialogEndModel
    {
        public const string SpecializationTypeId = "ad3de626-cb72-4dde-966b-122a6b386fb7";
        public const string SpecializationType = "Show Dialog Source End";

        public ShowDialogSourceEndModel(IAssociationEnd associationEnd, ShowDialogModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ShowDialogTargetEndModel : ShowDialogEndModel
    {
        public const string SpecializationTypeId = "c44a7969-abfa-4073-ab2c-d2d0f1f6bd2f";
        public const string SpecializationType = "Show Dialog Target End";

        public ShowDialogTargetEndModel(IAssociationEnd associationEnd, ShowDialogModel association) : base(associationEnd, association)
        {
        }

        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class ShowDialogEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly ShowDialogModel _association;

        public ShowDialogEndModel(IAssociationEnd associationEnd, ShowDialogModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static ShowDialogEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new ShowDialogModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (ShowDialogEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public ShowDialogModel Association => _association;
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

        public ShowDialogEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (ShowDialogEndModel)_association.TargetEnd : (ShowDialogEndModel)_association.SourceEnd;
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

        public bool Equals(ShowDialogEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ShowDialogEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ShowDialogEndModelExtensions
    {
        public static bool IsShowDialogEndModel(this ICanBeReferencedType type)
        {
            return IsShowDialogTargetEndModel(type) || IsShowDialogSourceEndModel(type);
        }

        public static ShowDialogEndModel AsShowDialogEndModel(this ICanBeReferencedType type)
        {
            return (ShowDialogEndModel)type.AsShowDialogTargetEndModel() ?? (ShowDialogEndModel)type.AsShowDialogSourceEndModel();
        }

        public static bool IsShowDialogTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ShowDialogTargetEndModel.SpecializationTypeId;
        }

        public static ShowDialogTargetEndModel AsShowDialogTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsShowDialogTargetEndModel() ? new ShowDialogModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsShowDialogSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ShowDialogSourceEndModel.SpecializationTypeId;
        }

        public static ShowDialogSourceEndModel AsShowDialogSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsShowDialogSourceEndModel() ? new ShowDialogModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}