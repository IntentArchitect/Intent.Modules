using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Services.GraphQL.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class GraphQLSchemaFieldModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "GraphQL Schema Field";
        public const string SpecializationTypeId = "150aa241-479b-442e-9962-21b79de85648";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public GraphQLSchemaFieldModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;


        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

        public IList<GraphQLParameterModel> Parameters => _element.ChildElements
            .GetElementsOfType(GraphQLParameterModel.SpecializationTypeId)
            .Select(x => new GraphQLParameterModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(GraphQLSchemaFieldModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GraphQLSchemaFieldModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GraphQLSchemaFieldModelExtensions
    {

        public static bool IsGraphQLSchemaFieldModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == GraphQLSchemaFieldModel.SpecializationTypeId;
        }

        public static GraphQLSchemaFieldModel AsGraphQLSchemaFieldModel(this ICanBeReferencedType type)
        {
            return type.IsGraphQLSchemaFieldModel() ? new GraphQLSchemaFieldModel((IElement)type) : null;
        }

        public static bool HasMapToDomainMapping(this GraphQLSchemaFieldModel type)
        {
            return type.Mapping?.MappingSettingsId == "79d0e613-7688-4b73-8f84-79eba94b8b46";
        }

        public static IElementMapping GetMapToDomainMapping(this GraphQLSchemaFieldModel type)
        {
            return type.HasMapToDomainMapping() ? type.Mapping : null;
        }

        public static bool HasMapToServiceMapping(this GraphQLSchemaFieldModel type)
        {
            return type.Mapping?.MappingSettingsId == "cf512bcf-7b14-47ee-9c62-cf1b40fd2b7f";
        }

        public static IElementMapping GetMapToServiceMapping(this GraphQLSchemaFieldModel type)
        {
            return type.HasMapToServiceMapping() ? type.Mapping : null;
        }
    }
}