using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MapAttributeModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Map Attribute";
        public const string SpecializationTypeId = "4b5f63fc-2019-4ec0-8f1a-4f47f397a60c";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MapAttributeModel(IElement element, string requiredType = SpecializationType)
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

        public IList<ScalarAttributeModel> ScalarAttributes => _element.ChildElements
            .GetElementsOfType(ScalarAttributeModel.SpecializationTypeId)
            .Select(x => new ScalarAttributeModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MapAttributeModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MapAttributeModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MapAttributeModelExtensions
    {

        public static bool IsMapAttributeModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MapAttributeModel.SpecializationTypeId;
        }

        public static MapAttributeModel AsMapAttributeModel(this ICanBeReferencedType type)
        {
            return type.IsMapAttributeModel() ? new MapAttributeModel((IElement)type) : null;
        }
    }
}