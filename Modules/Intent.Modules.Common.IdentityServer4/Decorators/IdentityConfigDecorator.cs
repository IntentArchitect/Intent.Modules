using Intent.Templates;
using System.Collections.Generic;

namespace Intent.Modules.Common.IdentityServer4.Decorators
{
    public abstract class IdentityConfigDecorator : ITemplateDecorator
    {
        public abstract int Priority { get; }

        public abstract EntityCollection GetClients();
    }

    public class EntityCollection : List<Entity>
    {
    }

    public class Entity : Dictionary<string, string>
    {
    }
}
