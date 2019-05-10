using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Angular.Api
{
    public class ModuleModel : IModuleModel, IEquatable<IModuleModel>
    {
        private readonly IClass _class;

        public ModuleModel(IClass @class)
        {
            _class = @class;
            Components = @class.ChildClasses.Where(x => x.SpecializationType == "Component").Select(x => new ComponentModel(x, this));
            ServiceProxies = @class.ChildClasses.Where(x => x.SpecializationType == "Service Proxy").Select(x => new ServiceProxyModel(x, this));
            ModelDefinitions = @class.ChildClasses.Where(x => x.SpecializationType == "Model Definition").Select(x => new ModuleDTOModel(x, this));
        }

        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder => _class.Folder;
        public string Id => _class.Id;
        public string Name => _class.Name;
        public IApplication Application => _class.Application;
        public IEnumerable<IServiceProxyModel> ServiceProxies { get; }
        public IEnumerable<IModuleDTOModel> ModelDefinitions { get; }
        public string Comment => _class.Comment;

        public IEnumerable<IComponentModel> Components { get; }

        public bool Equals(IModuleModel other)
        {
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IModuleModel)obj);
        }

        public override int GetHashCode()
        {
            return (_class != null ? _class.GetHashCode() : 0);
        }
    }
}