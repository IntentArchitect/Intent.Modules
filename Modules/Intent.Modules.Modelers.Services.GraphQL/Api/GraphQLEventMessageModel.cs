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
    public class GraphQLEventMessageModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "GraphQL Event Message";
        public const string SpecializationTypeId = "e68932de-3dcf-4b2f-8796-59ff28a81efa";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public GraphQLEventMessageModel(IElement element, string requiredType = SpecializationType)
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


        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(GraphQLEventMessageModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GraphQLEventMessageModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GraphQLEventMessageModelExtensions
    {

        public static bool IsGraphQLEventMessageModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == GraphQLEventMessageModel.SpecializationTypeId;
        }

        public static GraphQLEventMessageModel AsGraphQLEventMessageModel(this ICanBeReferencedType type)
        {
            return type.IsGraphQLEventMessageModel() ? new GraphQLEventMessageModel((IElement)type) : null;
        }
    }
}