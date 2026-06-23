using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Modelers.UI.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CompositionModel : IMetadataModel
    {
        public const string SpecializationType = "Composition";
        public const string SpecializationTypeId = "503b9ea9-4e8b-41d7-bbc6-92c97666c476";
        protected readonly IAssociation _association;
        protected CompositionSourceEndModel _sourceEnd;
        protected CompositionTargetEndModel _targetEnd;

        public CompositionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static CompositionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new CompositionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public CompositionSourceEndModel SourceEnd => _sourceEnd ??= new CompositionSourceEndModel(_association.SourceEnd, this);

        public CompositionTargetEndModel TargetEnd => _targetEnd ??= new CompositionTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(CompositionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CompositionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CompositionSourceEndModel : CompositionEndModel
    {
        public const string SpecializationTypeId = "e5e22007-149b-499c-b2e9-5c17064b606a";
        public const string SpecializationType = "Composition Source End";

        public CompositionSourceEndModel(IAssociationEnd associationEnd, CompositionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CompositionTargetEndModel : CompositionEndModel, IInvokableModel
    {
        public const string SpecializationTypeId = "15131ce2-31a6-461a-9889-42ec0f8f980b";
        public const string SpecializationType = "Composition Target End";

        public CompositionTargetEndModel(IAssociationEnd associationEnd, CompositionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class CompositionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly CompositionModel _association;

        public CompositionEndModel(IAssociationEnd associationEnd, CompositionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static CompositionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new CompositionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (CompositionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public CompositionModel Association => _association;
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

        public CompositionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (CompositionEndModel)_association.TargetEnd : (CompositionEndModel)_association.SourceEnd;
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

        public bool Equals(CompositionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CompositionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CompositionEndModelExtensions
    {
        public static bool IsCompositionEndModel(this ICanBeReferencedType type)
        {
            return IsCompositionTargetEndModel(type) || IsCompositionSourceEndModel(type);
        }

        public static CompositionEndModel AsCompositionEndModel(this ICanBeReferencedType type)
        {
            return (CompositionEndModel)type.AsCompositionTargetEndModel() ?? (CompositionEndModel)type.AsCompositionSourceEndModel();
        }

        public static bool IsCompositionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CompositionTargetEndModel.SpecializationTypeId;
        }

        public static CompositionTargetEndModel AsCompositionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsCompositionTargetEndModel() ? new CompositionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsCompositionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == CompositionSourceEndModel.SpecializationTypeId;
        }

        public static CompositionSourceEndModel AsCompositionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsCompositionSourceEndModel() ? new CompositionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}