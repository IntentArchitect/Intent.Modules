using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    public abstract class DomainEntityDecoratorBase : ITemplateDecorator, IDeclareUsings
    {
        protected DomainEntityDecoratorBase(DomainEntityTemplate template)
        {
            Template = template;
        }

        public DomainEntityTemplate Template { get; }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }

        public virtual string Constructors(ClassModel @class) { return null; }

        public int Priority { get; set; } = 0;
    }
}