using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common;

namespace Intent.Modules.EntityFramework.Migrations.Templates.DbMigrationsConfiguration
{
    public interface IMigrationSeedDecorator : ITemplateDecorator, IDeclareUsings
    {
        IEnumerable<string> Seed(string dbContextVariableName);
        IEnumerable<string> Methods();
    }
}
