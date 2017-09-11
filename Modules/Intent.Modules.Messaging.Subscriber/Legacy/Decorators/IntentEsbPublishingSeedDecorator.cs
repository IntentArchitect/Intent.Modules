using System.Collections.Generic;
using System.Linq;
using Intent.Packages.EntityFramework.Migrations.Templates.DbMigrationsConfiguration;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.SoftwareFactory.Modules.Decorators.IntentEsb
{
    public class IntentEsbPublishingSeedDecorator : IMigrationSeedDecorator, IHasNugetDependencies
    {
        private readonly ApplicationModel _applicationModel;

        public IntentEsbPublishingSeedDecorator(ApplicationModel applicationModel)
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
            var applicationName = _applicationModel.Name;

            return _applicationModel.EventingModel.PublishingQueues
                .Select(x => $"CreateEventingPublishingTable(" + $"{dbContextVariableName}, \"{applicationName}\", " + $"\"{x.GetOutputTableName(applicationName)}\");")
                .ToArray();
        }

        public IEnumerable<string> Methods()
        {
            if (!_applicationModel.EventingModel.PublishingQueues.SelectMany(x => x.PublishedEvents).Any())
            {
                return new string[0];
            }

            return new List<string>
            {
                CreateEventingPublishingTableMethod(),
            };
        }

        private static string CreateEventingPublishingTableMethod()
        {
            return @"
        private static void CreateEventingPublishingTable(DbContext dbContext, string schema, string tableName)
        {
            var indexName = $""IX_{tableName}_MessageId"";

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
    SELECT * FROM sys.tables T
    INNER JOIN sys.schemas S ON S.schema_id = T.schema_id
    WHERE T.name='{tableName}' AND S.name = '{schema}'
)
BEGIN
    CREATE TABLE [{schema}].[{tableName}](
         [OrderId] [bigint] IDENTITY(1,1) NOT NULL,
         [MessageId] [uniqueidentifier] NOT NULL,
         [Message] [varchar](max) NOT NULL,
         [PublishedOn] [datetime2](7) NOT NULL DEFAULT (getdate()),
     CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
    (
         [OrderId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (
    SELECT * 
    FROM sys.indexes 
    WHERE name='{indexName}' AND object_id = OBJECT_ID('[{schema}].[{tableName}]')
)
BEGIN
CREATE NONCLUSTERED INDEX [{indexName}] ON [{schema}].[{tableName}]
(
    [MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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
