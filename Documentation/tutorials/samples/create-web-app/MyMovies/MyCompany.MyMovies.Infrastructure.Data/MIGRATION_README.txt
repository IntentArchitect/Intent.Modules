Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Add-Migration -Name {ChangeName} -StartupProject "MyCompany.MyMovies.Api" -Project MyCompany.MyMovies.Infrastructure.Data


Override an existing migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Add-Migration -Name {ExistingNameWithoutDateComponent} -StartupProject "MyCompany.MyMovies.Api" -Project MyCompany.MyMovies.Infrastructure.Data


Update to latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Update-Database -StartupProject "MyCompany.MyMovies.Api" -Project MyCompany.MyMovies.Infrastructure.Data


Upgrade/downgrade to specific version
-------------------------------------------------------------------------------------------------------------------------------------------------------
Update-Database -StartupProject "MyCompany.MyMovies.Api" -Project MyCompany.MyMovies.Infrastructure.Data -Migration {Target}


Generate script which detects current database version and updates it to the latest:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Script-Migration -StartupProject "MyCompany.MyMovies.Api" -Project MyCompany.MyMovies.Infrastructure.Data -Script -SourceMigration:$InitialDatabase


Generate a script two upgrade from and to a specific version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
Script-Migration -StartupProject "MyCompany.MyMovies.Api" -Project MyCompany.MyMovies.Infrastructure.Data -Script -SourceMigration:{Source} -TargetMigration:{Target}


Drop all tables in schema:
-------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @SCHEMA AS varchar(max) = 'MyMovies'
DECLARE @EXECUTE_STATEMENT AS varchar(max) = (SELECT STUFF((SELECT CHAR(13) + CHAR(10) + [Statement] FROM (
    SELECT 'ALTER TABLE ['+@SCHEMA+'].['+[t].[name]+'] DROP CONSTRAINT ['+[fk].[name]+']' AS [Statement] FROM [sys].[foreign_keys] AS [fk] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [fk].[parent_object_id] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
    UNION ALL
    SELECT 'DROP TABLE ['+@SCHEMA+'].['+[t].[name]+']' AS [Statement] FROM [sys].[tables] AS [t] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [t].[schema_id] WHERE [s].[name] = @SCHEMA
) A FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''))
EXECUTE(@EXECUTE_STATEMENT)
