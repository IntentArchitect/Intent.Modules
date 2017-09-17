using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.EntityFramework.Migrations.Templates.DbMigrationsConfiguration
{
    public interface IMigrationSeedDecorator : ITemplateDecorator, IDeclareUsings
    {
        IEnumerable<string> Seed(string dbContextVariableName);
        IEnumerable<string> Methods();
    }
}
