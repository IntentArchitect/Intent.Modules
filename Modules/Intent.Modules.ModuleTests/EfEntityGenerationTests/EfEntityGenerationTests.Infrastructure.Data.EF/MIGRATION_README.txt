Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
add-migration -Name {ChangeName} -StartupProjectName "EfEntityGenerationTests.Domain" -ProjectName EfEntityGenerationTests.Infrastructure.Data.EF -ConfigurationTypeName EfEntityGenerationTestsDbContextConfiguration


Override an existing migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
add-migration -Name {ExistingNameWithoutDateComponent} -StartupProjectName "EfEntityGenerationTests.Domain" -ProjectName EfEntityGenerationTests.Infrastructure.Data.EF -ConfigurationTypeName EfEntityGenerationTestsDbContextConfiguration -Force


Update to latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
update-database -StartupProjectName "EfEntityGenerationTests.Domain" -ProjectName EfEntityGenerationTests.Infrastructure.Data.EF -ConfigurationTypeName EfEntityGenerationTestsDbContextConfiguration


Upgrade/downgrade to specific version
-------------------------------------------------------------------------------------------------------------------------------------------------------
update-database -StartupProjectName "EfEntityGenerationTests.Domain" -ProjectName EfEntityGenerationTests.Infrastructure.Data.EF -ConfigurationTypeName EfEntityGenerationTestsDbContextConfiguration -TargetMigration:{Target}


Generate script which detects current database version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
update-database -StartupProjectName "EfEntityGenerationTests.Domain" -ProjectName EfEntityGenerationTests.Infrastructure.Data.EF -ConfigurationTypeName EfEntityGenerationTestsDbContextConfiguration -Script -SourceMigration:$InitialDatabase


Generate a script two upgrade from and to a specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
update-database -StartupProjectName "EfEntityGenerationTests.Domain" -ProjectName EfEntityGenerationTests.Infrastructure.Data.EF -ConfigurationTypeName EfEntityGenerationTestsDbContextConfiguration -Script -SourceMigration:{Source} -TargetMigration:{Target}


Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'EfEntityGenerationTests'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
