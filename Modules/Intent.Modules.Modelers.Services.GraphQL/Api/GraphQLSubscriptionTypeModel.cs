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
    public class GraphQLSubscriptionTypeModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "GraphQL Subscription Type";
        public const string SpecializationTypeId = "b09d3b7f-63ad-4518-9a6a-5c7401c57c1b";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public GraphQLSubscriptionTypeModel(IElement element, string requiredType = SpecializationType)
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

        public IList<GraphQLSubscriptionModel> Subscriptions => _element.ChildElements
            .GetElementsOfType(GraphQLSubscriptionModel.SpecializationTypeId)
            .Select(x => new GraphQLSubscriptionModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(GraphQLSubscriptionTypeModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GraphQLSubscriptionTypeModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GraphQLSubscriptionTypeModelExtensions
    {

        public static bool IsGraphQLSubscriptionTypeModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == GraphQLSubscriptionTypeModel.SpecializationTypeId;
        }

        public static GraphQLSubscriptionTypeModel AsGraphQLSubscriptionTypeModel(this ICanBeReferencedType type)
        {
            return type.IsGraphQLSubscriptionTypeModel() ? new GraphQLSubscriptionTypeModel((IElement)type) : null;
        }
    }
}