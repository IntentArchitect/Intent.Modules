using System.Collections.Generic;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.EntityFramework.Repositories.Decorators
{
    public class DbContextDecorator : DbContextDecoratorBase, IHasNugetDependencies
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

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>()
            {
                new NugetPackageInfo("Intent.Framework.EntityFramework", "1.0.0")
            };
        }
    }
}