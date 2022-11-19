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
    public class DynamoDBMapAttributeModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Dynamo DB Map Attribute";
        public const string SpecializationTypeId = "4b5f63fc-2019-4ec0-8f1a-4f47f397a60c";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DynamoDBMapAttributeModel(IElement element, string requiredType = SpecializationType)
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

        public IList<DynamoDBScalarValueAttributeModel> ValueAttributes => _element.ChildElements
            .GetElementsOfType(DynamoDBScalarValueAttributeModel.SpecializationTypeId)
            .Select(x => new DynamoDBScalarValueAttributeModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DynamoDBMapAttributeModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DynamoDBMapAttributeModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DynamoDBMapAttributeModelExtensions
    {

        public static bool IsDynamoDBMapAttributeModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DynamoDBMapAttributeModel.SpecializationTypeId;
        }

        public static DynamoDBMapAttributeModel AsDynamoDBMapAttributeModel(this ICanBeReferencedType type)
        {
            return type.IsDynamoDBMapAttributeModel() ? new DynamoDBMapAttributeModel((IElement)type) : null;
        }
    }
}