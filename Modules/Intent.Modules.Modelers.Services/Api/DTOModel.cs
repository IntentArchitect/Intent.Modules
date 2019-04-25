using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public class DTOModel : IDTOModel
    {
        private readonly IClass _class;
        public DTOModel(IClass @class)
        {
            _class = @class;
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder => _class.Folder;
        public string Name => _class.Name;
        public IEnumerable<string> GenericTypes => _class.GenericTypes;
        public bool IsMapped => _class.IsMapped;
        public IClassMapping MappedClass => _class.MappedClass;
        public IApplication Application => _class.Application;
        public IEnumerable<IAttribute> Fields => _class.Attributes;
        public string Comment => _class.Id;
    }
}