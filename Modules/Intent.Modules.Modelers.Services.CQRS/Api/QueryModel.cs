using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Merge)]
    public class QueryModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder
    {
        public const string SpecializationType = "Query";
        public const string SpecializationTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public QueryModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        [IntentManaged(Mode.Ignore)]
        public string GetConceptName()
        {
            return Name.RemoveSuffix("Query");
        }

        public IList<DTOFieldModel> Properties => _element.ChildElements
            .GetElementsOfType(DTOFieldModel.SpecializationTypeId)
            .Select(x => new DTOFieldModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(QueryModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;

        public FolderModel Folder { get; }
    }

    [IntentManaged(Mode.Fully)]
    public static class QueryModelExtensions
    {

        public static bool IsQueryModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == QueryModel.SpecializationTypeId;
        }

        public static QueryModel AsQueryModel(this ICanBeReferencedType type)
        {
            return type.IsQueryModel() ? new QueryModel((IElement)type) : null;
        }
    }
}