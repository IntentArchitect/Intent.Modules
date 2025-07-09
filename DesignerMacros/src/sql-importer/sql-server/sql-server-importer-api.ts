/// <reference path="./strategy-database-import.ts" />
/// <reference path="./strategy-stored-procedures-import.ts" />

let SqlServerImporterApi = {
    importDatabase,
    importStoredProcedures
};

async function importDatabase(packageElement: IElementApi): Promise<void> {
    var strategy = new DatabaseImportStrategy();
    await strategy.execute(packageElement);
} 

async function importStoredProcedures(repositoryElement: IElementApi): Promise<void> {
    var strategy = new StoredProceduresImportStrategy();
    await strategy.execute(repositoryElement);
}
