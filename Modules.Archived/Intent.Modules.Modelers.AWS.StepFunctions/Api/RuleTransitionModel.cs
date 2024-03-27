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
    public class RuleTransitionModel : IMetadataModel
    {
        public const string SpecializationType = "Rule Transition";
        public const string SpecializationTypeId = "40ba8d81-0ea4-4934-a36e-fbb2daab8e2e";
        protected readonly IAssociation _association;
        protected RuleTransitionSourceEndModel _sourceEnd;
        protected RuleTransitionTargetEndModel _targetEnd;

        public RuleTransitionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static RuleTransitionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new RuleTransitionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public RuleTransitionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new RuleTransitionSourceEndModel(_association.SourceEnd, this));

        public RuleTransitionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new RuleTransitionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(RuleTransitionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RuleTransitionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class RuleTransitionSourceEndModel : RuleTransitionEndModel
    {
        public const string SpecializationTypeId = "04abdefd-f368-4587-a008-371d725edfcc";

        public RuleTransitionSourceEndModel(IAssociationEnd associationEnd, RuleTransitionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class RuleTransitionTargetEndModel : RuleTransitionEndModel
    {
        public const string SpecializationTypeId = "c0ca5899-7d7a-4399-a4ca-e1e2e48b04a6";

        public RuleTransitionTargetEndModel(IAssociationEnd associationEnd, RuleTransitionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class RuleTransitionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly RuleTransitionModel _association;

        public RuleTransitionEndModel(IAssociationEnd associationEnd, RuleTransitionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static RuleTransitionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new RuleTransitionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (RuleTransitionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public RuleTransitionModel Association => _association;
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

        public RuleTransitionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (RuleTransitionEndModel)_association.TargetEnd : (RuleTransitionEndModel)_association.SourceEnd;
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

        public bool Equals(RuleTransitionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RuleTransitionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class RuleTransitionEndModelExtensions
    {
        public static bool IsRuleTransitionEndModel(this ICanBeReferencedType type)
        {
            return IsRuleTransitionTargetEndModel(type) || IsRuleTransitionSourceEndModel(type);
        }

        public static RuleTransitionEndModel AsRuleTransitionEndModel(this ICanBeReferencedType type)
        {
            return (RuleTransitionEndModel)type.AsRuleTransitionTargetEndModel() ?? (RuleTransitionEndModel)type.AsRuleTransitionSourceEndModel();
        }

        public static bool IsRuleTransitionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == RuleTransitionTargetEndModel.SpecializationTypeId;
        }

        public static RuleTransitionTargetEndModel AsRuleTransitionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsRuleTransitionTargetEndModel() ? new RuleTransitionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsRuleTransitionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == RuleTransitionSourceEndModel.SpecializationTypeId;
        }

        public static RuleTransitionSourceEndModel AsRuleTransitionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsRuleTransitionSourceEndModel() ? new RuleTransitionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}