using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DynamoDBTableModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public const string SpecializationType = "DynamoDB Table";
        public const string SpecializationTypeId = "b45fcd71-d5a3-48c7-9dd5-e6a138080bcf";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DynamoDBTableModel(IElement element, string requiredType = SpecializationType)
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

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        public DynamoDBTableHashKeyModel HashKey => _element.ChildElements
            .GetElementsOfType(DynamoDBTableHashKeyModel.SpecializationTypeId)
            .Select(x => new DynamoDBTableHashKeyModel(x))
            .SingleOrDefault();

        public DynamoDBTableRangeKeyModel RangeKey => _element.ChildElements
            .GetElementsOfType(DynamoDBTableRangeKeyModel.SpecializationTypeId)
            .Select(x => new DynamoDBTableRangeKeyModel(x))
            .SingleOrDefault();

        public IList<DynamoDBItemModel> Items => _element.ChildElements
            .GetElementsOfType(DynamoDBItemModel.SpecializationTypeId)
            .Select(x => new DynamoDBItemModel(x))
            .ToList();

        public IList<DynamoDBMapValueAttributeModel> ItemMapValueAttributes => _element.ChildElements
            .GetElementsOfType(DynamoDBMapValueAttributeModel.SpecializationTypeId)
            .Select(x => new DynamoDBMapValueAttributeModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DynamoDBTableModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DynamoDBTableModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DynamoDBTableModelExtensions
    {

        public static bool IsDynamoDBTableModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DynamoDBTableModel.SpecializationTypeId;
        }

        public static DynamoDBTableModel AsDynamoDBTableModel(this ICanBeReferencedType type)
        {
            return type.IsDynamoDBTableModel() ? new DynamoDBTableModel((IElement)type) : null;
        }
    }
}