using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class DTOModel : IHasStereotypes, IMetadataModel, IHasFolder
    {
        private readonly IElement _class;
        public DTOModel(IElement @class)
        {
            _class = @class;
            Folder = _class.ParentElement?.SpecializationType == Api.FolderModel.SpecializationType ? new FolderModel(_class.ParentElement) : null;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public FolderModel Folder { get; }

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;
        public IEnumerable<string> GenericTypes => _class.GenericTypes.Select(x => x.Name);
        public bool IsMapped => _class.IsMapped;
        public IElementMapping MappedClass => _class.MappedElement;
        public IElementApplication Application => _class.Application;

        [IntentManaged(Mode.Fully)]
        public IList<DTOFieldModel> Fields => _element.ChildElements
            .Where(x => x.SpecializationType == DTOFieldModel.SpecializationType)
            .Select(x => new DTOFieldModel(x))
            .ToList();
        public string Comment => _class.Id;

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
        protected readonly IElement _element;
        public const string SpecializationType = "DTO";

        [IntentManaged(Mode.Fully)]
        public IList<DTOModel> DTOs => _element.ChildElements
            .Where(x => x.SpecializationType == DTOModel.SpecializationType)
            .Select(x => new DTOModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }
    }
}