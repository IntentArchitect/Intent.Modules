using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public class ServiceModel : IServiceModel
    {
        private readonly IClass _class;
        public ServiceModel(IClass @class)
        {
            _class = @class;
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder => _class.Folder;
        public string Name => _class.Name;
        public string ApplicationName => _class.Application.Name;
        public IApplication Application => _class.Application;
        public IEnumerable<IOperation> Operations => _class.Operations;
        public string Comment => _class.Id;
    }
}