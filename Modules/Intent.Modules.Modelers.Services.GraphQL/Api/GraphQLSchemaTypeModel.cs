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
    public class GraphQLSchemaTypeModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "GraphQL Schema Type";
        public const string SpecializationTypeId = "3f2d13dd-738f-453c-8c30-50953a9b3781";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public GraphQLSchemaTypeModel(IElement element, string requiredType = SpecializationType)
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

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

        public IList<GraphQLSchemaFieldModel> Fields => _element.ChildElements
            .GetElementsOfType(GraphQLSchemaFieldModel.SpecializationTypeId)
            .Select(x => new GraphQLSchemaFieldModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(GraphQLSchemaTypeModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GraphQLSchemaTypeModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class GraphQLSchemaTypeModelExtensions
    {

        public static bool IsGraphQLSchemaTypeModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == GraphQLSchemaTypeModel.SpecializationTypeId;
        }

        public static GraphQLSchemaTypeModel AsGraphQLSchemaTypeModel(this ICanBeReferencedType type)
        {
            return type.IsGraphQLSchemaTypeModel() ? new GraphQLSchemaTypeModel((IElement)type) : null;
        }

        public static bool HasMapFromDomainMapping(this GraphQLSchemaTypeModel type)
        {
            return type.Mapping?.MappingSettingsId == "637e11b2-b420-415a-af5f-5a63312c1ccc";
        }

        public static IElementMapping GetMapFromDomainMapping(this GraphQLSchemaTypeModel type)
        {
            return type.HasMapFromDomainMapping() ? type.Mapping : null;
        }

        public static bool HasMapFromDtoMapping(this GraphQLSchemaTypeModel type)
        {
            return type.Mapping?.MappingSettingsId == "67960843-0927-4127-ad4f-e19e3e60cc74";
        }

        public static IElementMapping GetMapFromDtoMapping(this GraphQLSchemaTypeModel type)
        {
            return type.HasMapFromDtoMapping() ? type.Mapping : null;
        }
    }
}