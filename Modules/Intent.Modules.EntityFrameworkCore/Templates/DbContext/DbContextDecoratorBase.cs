using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.DbContext
{
    public abstract class DbContextDecoratorBase : ITemplateDecorator, IDeclareUsings
    {
        public virtual string GetBaseClass() { return null; }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }

        public virtual IEnumerable<string> GetMethods() { return new List<string>(); }

        public int Priority { get; set; } = 0;
    }
}