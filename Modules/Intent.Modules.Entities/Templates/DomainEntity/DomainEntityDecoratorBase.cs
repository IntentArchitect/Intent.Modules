using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Templates

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    public abstract class DomainEntityDecoratorBase : ITemplateDecorator, IDeclareUsings, IPriorityDecorator
    {
        public DomainEntityTemplate Template { get; internal set; }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }

        public virtual string Constructors(IClass @class) { return null; }

        public int Priority { get; set; } = 0;
    }
}