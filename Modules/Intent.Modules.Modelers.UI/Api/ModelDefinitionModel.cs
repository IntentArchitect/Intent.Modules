using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ModelDefinitionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public const string SpecializationType = "Model Definition";
        public const string SpecializationTypeId = "bd3941b5-e3b3-4a40-96e6-b9c87cea0101";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ModelDefinitionModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
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

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

        public IList<ConstructorModel> Constructors => _element.ChildElements
            .GetElementsOfType(ConstructorModel.SpecializationTypeId)
            .Select(x => new ConstructorModel(x))
            .ToList();

        public IList<PropertyModel> Properties => _element.ChildElements
            .GetElementsOfType(PropertyModel.SpecializationTypeId)
            .Select(x => new PropertyModel(x))
            .ToList();

        public IList<OperationModel> Operations => _element.ChildElements
            .GetElementsOfType(OperationModel.SpecializationTypeId)
            .Select(x => new OperationModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ModelDefinitionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelDefinitionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ModelDefinitionModelExtensions
    {

        public static bool IsModelDefinitionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ModelDefinitionModel.SpecializationTypeId;
        }

        public static ModelDefinitionModel AsModelDefinitionModel(this ICanBeReferencedType type)
        {
            return type.IsModelDefinitionModel() ? new ModelDefinitionModel((IElement)type) : null;
        }

        public static bool HasMapFromDTOMapping(this ModelDefinitionModel type)
        {
            return type.Mapping?.MappingSettingsId == "31b3d3a7-bf3c-4bb4-8b1d-9e18b6a8bcdd";
        }

        public static IElementMapping GetMapFromDTOMapping(this ModelDefinitionModel type)
        {
            return type.HasMapFromDTOMapping() ? type.Mapping : null;
        }
    }
}