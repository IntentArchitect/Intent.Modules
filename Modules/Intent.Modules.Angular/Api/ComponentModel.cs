using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Angular.Api
{
    public class ComponentModel : IComponentModel, IEquatable<IComponentModel>
    {
        private readonly IClass _class;

        public ComponentModel(IClass @class, IModuleModel module)
        {
            _class = @class;
            Module = module;
        }

        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public string Id => _class.Id;
        public string Name => _class.Name;
        public string Comment => _class.Comment;
        public IModuleModel Module { get; }

        public bool Equals(IComponentModel other)
        {
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IComponentModel) obj);
        }

        public override int GetHashCode()
        {
            return (_class != null ? _class.GetHashCode() : 0);
        }
    }
}