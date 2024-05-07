using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DTOModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        protected readonly IElement _element;
        public const string SpecializationType = "DTO";
        public const string SpecializationTypeId = "fee0edca-4aa0-4f77-a524-6bbd84e78734";

        [IntentManaged(Mode.Ignore)]
        public DTOModel(IElement element, string requiredType = SpecializationType)
        {
            _element = element;
            Folder = _element.ParentElement?.SpecializationType == FolderModel.SpecializationType ? new FolderModel(_element.ParentElement) : null;

            // Intent.Modelers.Service.3.1.2: as part of 
            if (IsMapped && Mapping?.MappingSettingsId == null)
            {
                Logging.Log.Warning($@"DTO [{Name}] is mapped, but has not specified mapping settings. This may cause unexpected changes in the software factory execution. 
To fix this, open and re-save your Services designer. If this warning persists then please reach out to Intent Architect support.");
            }
        }

        public string Id => _element.Id;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public FolderModel Folder { get; }

        public string Name => _element.Name;

        public bool IsAbstract => _element.IsAbstract;

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public DTOModel ParentDto => this.Generalizations().Select(x => new DTOModel((IElement)x.Element)).SingleOrDefault();

        public ITypeReference ParentDtoTypeReference => this.Generalizations().SingleOrDefault()?.TypeReference;

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

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

        public static DTOModel AsDTOModel(this ICanBeReferencedType type)
        {
            return type.IsDTOModel() ? new DTOModel((IElement)type) : null;
        }

        public static bool HasMapFromDomainMapping(this DTOModel type)
        {
            return type.Mapping?.MappingSettingsId == "1f747d14-681c-4a20-8c68-34223f41b825";
        }

        public static IElementMapping GetMapFromDomainMapping(this DTOModel type)
        {
            return type.HasMapFromDomainMapping() ? type.Mapping : null;
        }

        public static bool HasProjectToDomainMapping(this DTOModel type)
        {
            return type.Mapping?.MappingSettingsId == "942eae46-49f1-450e-9274-a92d40ac35fa";
        }

        public static IElementMapping GetProjectToDomainMapping(this DTOModel type)
        {
            return type.HasProjectToDomainMapping() ? type.Mapping : null;
        }

        public static bool HasMapToDomainOperationMapping(this DTOModel type)
        {
            return type.Mapping?.MappingSettingsId == "8d1f6a8a-77c8-43a2-8e60-421559725419";
        }

        public static IElementMapping GetMapToDomainOperationMapping(this DTOModel type)
        {
            return type.HasMapToDomainOperationMapping() ? type.Mapping : null;
        }
    }
}