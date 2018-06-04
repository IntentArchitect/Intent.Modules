using System.Collections.Generic;
using Intent.Modules.EntityFramework.Templates.DbContext;

namespace Intent.Modules.EntityFramework.Repositories.Decorators
{
    public class DbContextDecorator : DbContextDecoratorBase
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.DbContextDecorator";

        public override IEnumerable<string> DeclareUsings()
        {
            return new[] { "using Intent.Framework.EntityFramework;" };
        }

        public override string GetBaseClass()
        {
            return "DbContextEx";
        }
    }
}