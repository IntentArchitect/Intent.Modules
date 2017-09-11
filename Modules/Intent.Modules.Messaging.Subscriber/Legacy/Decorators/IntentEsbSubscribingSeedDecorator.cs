using System.Collections.Generic;
using Intent.Packages.EntityFramework.Migrations.Templates.DbMigrationsConfiguration;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.SoftwareFactory.Modules.Decorators.IntentEsb
{
    public class IntentEsbSubscribingSeedDecorator : IMigrationSeedDecorator, IHasNugetDependencies
    {
        private readonly ApplicationModel _applicationModel;

        public IntentEsbSubscribingSeedDecorator(ApplicationModel applicationModel)
        {
            _applicationModel = applicationModel;
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "using System.Data.Entity;",
            };
        }

        public IEnumerable<string> Seed(string dbContextVariableName)
        {
            if (!_applicationModel.EventingModel.IsSubscriber)
            {
                return new string[0];
            }

            var applicationName = _applicationModel.Name;

            return new[]
            {
                $"CreateEventingIdempotenceTable({dbContextVariableName}, \"{applicationName}\", \"_EventingMessageReceived\");"
            };
        }

        public IEnumerable<string> Methods()
        {
            if (!_applicationModel.EventingModel.IsSubscriber)
            {
                return new string[0];
            }

            return new List<string>
            {
                CreateEventingIdempotenceTableMethod(),
            };
        }

        private static string CreateEventingIdempotenceTableMethod()
        {
            return @"
        private static void CreateEventingIdempotenceTable(DbContext dbContext, string schema, string tableName)
        {
            dbContext.Database.ExecuteSqlCommand($@""
IF NOT EXISTS (
    SELECT schema_name
    FROM information_schema.schemata
    WHERE schema_name = '{schema}'
)
BEGIN
    EXEC sp_executesql N'CREATE SCHEMA {schema}'
END

IF NOT EXISTS (
    SELECT * from sys.tables t
    INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
    WHERE t.name='{tableName}' AND s.name = '{schema}'
 )
BEGIN
    CREATE TABLE [{schema}].[{tableName}](
        [MessageId] [uniqueidentifier] NOT NULL,
        [Created] [datetime2](7) NOT NULL DEFAULT (getdate()),
     CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
    (
        [MessageId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END"");
        }";
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackageInfo.EntityFramework,
            };
        }
    }
}
