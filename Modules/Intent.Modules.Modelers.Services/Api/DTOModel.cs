using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class DTOModel : IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IHasTypeReference
    {
        protected readonly IElement _element;
        public const string SpecializationType = "DTO";
        public const string SpecializationTypeId = "fee0edca-4aa0-4f77-a524-6bbd84e78734";

        [IntentManaged(Mode.Ignore)]
        public DTOModel(IElement element, string requiredType = SpecializationType)
        {
            _element = element;
            Folder = _element.ParentElement?.SpecializationType == FolderModel.SpecializationType ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public FolderModel Folder { get; }

        public string Name => _element.Name;

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public ITypeReference TypeReference => _element.TypeReference;

        public IList<DTOFieldModel> Fields => _element.ChildElements
            .GetElementsOfType(DTOFieldModel.SpecializationTypeId)
            .Select(x => new DTOFieldModel(x))
            .ToList();

        public string Comment => _element.Comment;

        public bool Equals(DTOModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DTOModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DTOModelExtensions
    {

        public static bool IsDTOModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DTOModel.SpecializationTypeId;
        }

        public static DTOModel ToDTOModel(this ICanBeReferencedType type)
        {
            return type.IsDTOModel() ? new DTOModel((IElement)type) : null;
        }
    }
}