using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
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

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public FolderModel Folder { get; }
        public string Name => _class.Name;
        public IEnumerable<string> GenericTypes => _class.GenericTypes.Select(x => x.Name);
        public bool IsMapped => _class.IsMapped;
        public IElementMapping MappedClass => _class.MappedElement;
        public IElementApplication Application => _class.Application;

        [IntentManaged(Mode.Fully)]
        public IList<DTOFieldModel> Fields => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DTOFieldModel.SpecializationType)
            .Select(x => new DTOFieldModel(x))
            .ToList<DTOFieldModel>();
        public string Comment => _class.Id;

        protected bool Equals(DTOModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
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
            return (Id != null ? Id.GetHashCode() : 0);
        }
        protected readonly IElement _element;
        public const string SpecializationType = "DTO";

        [IntentManaged(Mode.Fully)]
        public IList<DTOModel> DTOs => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DTOModel.SpecializationType)
            .Select(x => new DTOModel(x))
            .ToList<DTOModel>();
    }
}