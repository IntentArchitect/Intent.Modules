using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.GraphQL.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class GraphQLEndpointModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "GraphQL Endpoint";
        public const string SpecializationTypeId = "c394de2e-9c3b-4633-b6df-702904ad9914";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public GraphQLEndpointModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public IList<GraphQLQueryTypeModel> QueryTypes => _element.ChildElements
            .GetElementsOfType(GraphQLQueryTypeModel.SpecializationTypeId)
            .Select(x => new GraphQLQueryTypeModel(x))
            .ToList();

        public IList<GraphQLMutationTypeModel> MutationTypes => _element.ChildElements
            .GetElementsOfType(GraphQLMutationTypeModel.SpecializationTypeId)
            .Select(x => new GraphQLMutationTypeModel(x))
            .ToList();

        public IList<GraphQLSchemaTypeModel> Types => _element.ChildElements
            .GetElementsOfType(GraphQLSchemaTypeModel.SpecializationTypeId)
            .Select(x => new GraphQLSchemaTypeModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(GraphQLEndpointModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GraphQLEndpointModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GraphQLEndpointModelExtensions
    {

        public static bool IsGraphQLEndpointModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == GraphQLEndpointModel.SpecializationTypeId;
        }

        public static GraphQLEndpointModel AsGraphQLEndpointModel(this ICanBeReferencedType type)
        {
            return type.IsGraphQLEndpointModel() ? new GraphQLEndpointModel((IElement)type) : null;
        }
    }
}