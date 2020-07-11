using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class DTOModel : IHasStereotypes, IMetadataModel, IHasFolder
    {
        protected readonly IElement _element;
        public const string SpecializationType = "DTO";

        public DTOModel(IElement element, string requiredType = SpecializationType)
        {
            _element = element;
            Folder = _element.ParentElement?.SpecializationType == FolderModel.SpecializationType ? new FolderModel(_element.ParentElement) : null;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public FolderModel Folder { get; }

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public IElementApplication Application => _element.Application;

        [IntentManaged(Mode.Fully)]
        public bool IsMapped => _element.IsMapped;

        [IntentManaged(Mode.Fully)]
        public IElementMapping Mapping => _element.MappedElement;

        [IntentManaged(Mode.Fully)]
        public IList<DTOFieldModel> Fields => _element.ChildElements
            .Where(x => x.SpecializationType == DTOFieldModel.SpecializationType)
            .Select(x => new DTOFieldModel(x))
            .ToList();

        public string Comment => _element.Id;

        [IntentManaged(Mode.Fully)]
        public bool Equals(DTOModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DTOModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }
        public const string SpecializationTypeId = "fee0edca-4aa0-4f77-a524-6bbd84e78734";
    }
}