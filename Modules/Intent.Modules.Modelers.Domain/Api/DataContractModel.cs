using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DataContractModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference, IHasFolder
    {
        public const string SpecializationType = "Data Contract";
        public const string SpecializationTypeId = "4464fabe-c59e-4d90-81fc-c9245bdd1afd";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DataContractModel(IElement element, string requiredType = SpecializationType)
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

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference BaseType => TypeReference?.Element != null ? TypeReference : null;

        public IElement InternalElement => _element;

        public IList<AttributeModel> Attributes => _element.ChildElements
            .GetElementsOfType(AttributeModel.SpecializationTypeId)
            .Select(x => new AttributeModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DataContractModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataContractModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DataContractModelExtensions
    {

        public static bool IsDataContractModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DataContractModel.SpecializationTypeId;
        }

        public static DataContractModel AsDataContractModel(this ICanBeReferencedType type)
        {
            return type.IsDataContractModel() ? new DataContractModel((IElement)type) : null;
        }
    }
}