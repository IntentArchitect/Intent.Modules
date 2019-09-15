using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    internal class ServiceModel : IServiceModel
    {
        private readonly IElement _class;
        public ServiceModel(IElement @class)
        {
            _class = @class;
            Folder = _class.ParentElement?.SpecializationType == Api.Folder.SpecializationType ? new Folder(_class.ParentElement) : null;
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _class.Name;
        public string ApplicationName => _class.Application.Name;
        public IElementApplication Application => _class.Application;
        public IEnumerable<IOperation> Operations => _class.Operations;
        public string Comment => _class.Id;
    }
}