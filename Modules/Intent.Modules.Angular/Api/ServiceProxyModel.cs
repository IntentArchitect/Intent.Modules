using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public class ServiceProxyModel : IServiceProxyModel
    {
        private readonly IClass _class;

        public ServiceProxyModel(IClass @class, IModuleModel module)
        {
            _class = @class;
            Module = module;
            MappedService = _class.MappedClass != null ? new ServiceModel(_class.MappedClass.Class) : null;
        }

        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public string Id => _class.Id;
        public string Name => _class.Name;
        public string Comment => _class.Comment;
        public IModuleModel Module { get; }
        public IServiceModel MappedService { get; }
        public IEnumerable<IOperation> Operations => _class.Operations;

        public bool Equals(IComponentModel other)
        {
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IComponentModel)obj);
        }

        public override int GetHashCode()
        {
            return (_class != null ? _class.GetHashCode() : 0);
        }
    }
}