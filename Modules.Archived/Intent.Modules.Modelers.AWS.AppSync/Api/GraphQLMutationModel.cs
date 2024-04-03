using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.AppSync.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class GraphQLMutationModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "GraphQL Mutation";
        public const string SpecializationTypeId = "66cca984-b1dc-445c-9685-e3abb4e5795a";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public GraphQLMutationModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(GraphQLMutationModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GraphQLMutationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GraphQLMutationModelExtensions
    {

        public static bool IsGraphQLMutationModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == GraphQLMutationModel.SpecializationTypeId;
        }

        public static GraphQLMutationModel AsGraphQLMutationModel(this ICanBeReferencedType type)
        {
            return type.IsGraphQLMutationModel() ? new GraphQLMutationModel((IElement)type) : null;
        }

        public static bool HasMapToLambdaMapping(this GraphQLMutationModel type)
        {
            return type.Mapping?.MappingSettingsId == "803e7511-8622-4d3c-93d3-92c840014e79";
        }

        public static IElementMapping GetMapToLambdaMapping(this GraphQLMutationModel type)
        {
            return type.HasMapToLambdaMapping() ? type.Mapping : null;
        }

        public static bool HasMapToDynamoDBEntityMapping(this GraphQLMutationModel type)
        {
            return type.Mapping?.MappingSettingsId == "91a283eb-8cf2-4c5a-9b3f-8ba8724c13cb";
        }

        public static IElementMapping GetMapToDynamoDBEntityMapping(this GraphQLMutationModel type)
        {
            return type.HasMapToDynamoDBEntityMapping() ? type.Mapping : null;
        }
    }
}