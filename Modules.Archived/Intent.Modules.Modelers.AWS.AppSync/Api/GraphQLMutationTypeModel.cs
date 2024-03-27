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
    public class GraphQLMutationTypeModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "GraphQL Mutation Type";
        public const string SpecializationTypeId = "02864de4-afe0-42a1-a655-1eeb68c8a098";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public GraphQLMutationTypeModel(IElement element, string requiredType = SpecializationType)
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

        public IList<GraphQLMutationModel> Mutations => _element.ChildElements
            .GetElementsOfType(GraphQLMutationModel.SpecializationTypeId)
            .Select(x => new GraphQLMutationModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(GraphQLMutationTypeModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GraphQLMutationTypeModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GraphQLMutationTypeModelExtensions
    {

        public static bool IsGraphQLMutationTypeModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == GraphQLMutationTypeModel.SpecializationTypeId;
        }

        public static GraphQLMutationTypeModel AsGraphQLMutationTypeModel(this ICanBeReferencedType type)
        {
            return type.IsGraphQLMutationTypeModel() ? new GraphQLMutationTypeModel((IElement)type) : null;
        }
    }
}