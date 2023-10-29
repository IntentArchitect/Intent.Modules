using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Services.DomainInteractions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class QueryEntityActionModel : IMetadataModel
    {
        public const string SpecializationType = "Query Entity Action";
        public const string SpecializationTypeId = "47ab5888-a258-4bec-a9fc-a83de69eb79d";
        protected readonly IAssociation _association;
        protected QueryEntityActionSourceEndModel _sourceEnd;
        protected QueryEntityActionTargetEndModel _targetEnd;

        public QueryEntityActionModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static QueryEntityActionModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new QueryEntityActionModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public QueryEntityActionSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new QueryEntityActionSourceEndModel(_association.SourceEnd, this));

        public QueryEntityActionTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new QueryEntityActionTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(QueryEntityActionModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryEntityActionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class QueryEntityActionSourceEndModel : QueryEntityActionEndModel
    {
        public const string SpecializationTypeId = "32a65f26-2555-4616-8a2c-6a90805600bb";
        public const string SpecializationType = "Query Entity Action Source End";

        public QueryEntityActionSourceEndModel(IAssociationEnd associationEnd, QueryEntityActionModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class QueryEntityActionTargetEndModel : QueryEntityActionEndModel
    {
        public const string SpecializationTypeId = "93ef6675-cba4-4998-adff-cb22d5343ed4";
        public const string SpecializationType = "Query Entity Action Target End";

        public QueryEntityActionTargetEndModel(IAssociationEnd associationEnd, QueryEntityActionModel association) : base(associationEnd, association)
        {
        }
        public IList<ProcessingActionModel> ProcessingActions => InternalElement.ChildElements
            .GetElementsOfType(ProcessingActionModel.SpecializationTypeId)
            .Select(x => new ProcessingActionModel(x))
            .ToList();


        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class QueryEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly QueryEntityActionModel _association;

        public QueryEntityActionEndModel(IAssociationEnd associationEnd, QueryEntityActionModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static QueryEntityActionEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new QueryEntityActionModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (QueryEntityActionEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public QueryEntityActionModel Association => _association;
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

        public QueryEntityActionEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (QueryEntityActionEndModel)_association.TargetEnd : (QueryEntityActionEndModel)_association.SourceEnd;
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

        public bool Equals(QueryEntityActionEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryEntityActionEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class QueryEntityActionEndModelExtensions
    {
        public static bool IsQueryEntityActionEndModel(this ICanBeReferencedType type)
        {
            return IsQueryEntityActionTargetEndModel(type) || IsQueryEntityActionSourceEndModel(type);
        }

        public static QueryEntityActionEndModel AsQueryEntityActionEndModel(this ICanBeReferencedType type)
        {
            return (QueryEntityActionEndModel)type.AsQueryEntityActionTargetEndModel() ?? (QueryEntityActionEndModel)type.AsQueryEntityActionSourceEndModel();
        }

        public static bool IsQueryEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == QueryEntityActionTargetEndModel.SpecializationTypeId;
        }

        public static QueryEntityActionTargetEndModel AsQueryEntityActionTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsQueryEntityActionTargetEndModel() ? new QueryEntityActionModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsQueryEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == QueryEntityActionSourceEndModel.SpecializationTypeId;
        }

        public static QueryEntityActionSourceEndModel AsQueryEntityActionSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsQueryEntityActionSourceEndModel() ? new QueryEntityActionModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}