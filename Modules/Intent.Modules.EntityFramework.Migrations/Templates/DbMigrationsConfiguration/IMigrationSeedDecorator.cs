using System.Collections.Generic;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.EntityFramework.Migrations.Templates.DbMigrationsConfiguration
{
    public interface IMigrationSeedDecorator : ITemplateDecorator, IDeclareUsings
    {
        IEnumerable<string> Seed(string dbContextVariableName);
        IEnumerable<string> Methods();
    }
}
