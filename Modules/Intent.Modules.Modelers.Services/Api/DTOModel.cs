using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    internal class DTOModel : IDTOModel
    {
        private readonly IElement _class;
        public DTOModel(IElement @class)
        {
            _class = @class;
            Folder = _class.ParentElement?.SpecializationType == Api.Folder.SpecializationType ? new Folder(_class.ParentElement) : null;
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _class.Name;
        public IEnumerable<string> GenericTypes => _class.GenericTypes.Select(x => x.Name);
        public bool IsMapped => _class.IsMapped;
        public IElementMapping MappedClass => _class.MappedElement;
        public IElementApplication Application => _class.Application;
        public IEnumerable<IAttribute> Fields => _class.Attributes;
        public string Comment => _class.Id;
    }
}