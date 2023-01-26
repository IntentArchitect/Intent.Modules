using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ChoiceTransitionModel : IMetadataModel
    {
        public const string SpecializationType = "Choice Transition";
        public const string SpecializationTypeId = "40ba8d81-0ea4-4934-a36e-fbb2daab8e2e";
        protected readonly IAssociation _association;
        protected ChoiceTransitionSourceEndModel _sourceEnd;
        protected ChoiceTransitionTargetEndModel _targetEnd;

        public ChoiceTransitionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static ChoiceTransitionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new ChoiceTransitionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public ChoiceTransitionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new ChoiceTransitionSourceEndModel(_association.SourceEnd, this));

        public ChoiceTransitionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new ChoiceTransitionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(ChoiceTransitionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChoiceTransitionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ChoiceTransitionSourceEndModel : ChoiceTransitionEndModel
    {
        public const string SpecializationTypeId = "04abdefd-f368-4587-a008-371d725edfcc";

        public ChoiceTransitionSourceEndModel(IAssociationEnd associationEnd, ChoiceTransitionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ChoiceTransitionTargetEndModel : ChoiceTransitionEndModel
    {
        public const string SpecializationTypeId = "c0ca5899-7d7a-4399-a4ca-e1e2e48b04a6";

        public ChoiceTransitionTargetEndModel(IAssociationEnd associationEnd, ChoiceTransitionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ChoiceTransitionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly ChoiceTransitionModel _association;

        public ChoiceTransitionEndModel(IAssociationEnd associationEnd, ChoiceTransitionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static ChoiceTransitionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new ChoiceTransitionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (ChoiceTransitionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public ChoiceTransitionModel Association => _association;
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

        public ChoiceTransitionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (ChoiceTransitionEndModel)_association.TargetEnd : (ChoiceTransitionEndModel)_association.SourceEnd;
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

        public bool Equals(ChoiceTransitionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChoiceTransitionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ChoiceTransitionEndModelExtensions
    {
        public static bool IsChoiceTransitionEndModel(this ICanBeReferencedType type)
        {
            return IsChoiceTransitionTargetEndModel(type) || IsChoiceTransitionSourceEndModel(type);
        }

        public static ChoiceTransitionEndModel AsChoiceTransitionEndModel(this ICanBeReferencedType type)
        {
            return (ChoiceTransitionEndModel)type.AsChoiceTransitionTargetEndModel() ?? (ChoiceTransitionEndModel)type.AsChoiceTransitionSourceEndModel();
        }

        public static bool IsChoiceTransitionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ChoiceTransitionTargetEndModel.SpecializationTypeId;
        }

        public static ChoiceTransitionTargetEndModel AsChoiceTransitionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsChoiceTransitionTargetEndModel() ? new ChoiceTransitionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsChoiceTransitionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ChoiceTransitionSourceEndModel.SpecializationTypeId;
        }

        public static ChoiceTransitionSourceEndModel AsChoiceTransitionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsChoiceTransitionSourceEndModel() ? new ChoiceTransitionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}