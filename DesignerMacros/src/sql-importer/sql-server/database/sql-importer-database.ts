/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />

type IDynamicFormFieldConfig = MacroApi.Context.IDynamicFormFieldConfig;

interface ISqlImportPackageSettings {
    entityNameConvention: string;
    tableStereotypes: string;
    schemaFilter: string;
    includeTables: string;
    includeViews: string;
    includeStoredProcedures: string;
    includeIndexes: string;
    tableViewFilterFilePath: string;
    storedProcedureType: string;
    connectionString: string;
    settingPersistence: string;
}

interface IDatabaseImportModel {
    applicationId: string;
    designerId: string;
    packageId: string;
    entityNameConvention: string;
    tableStereotype: string;
    typesToExport: string[];
    schemaFilter: string[];
    tableViewFilterFilePath: string;
    storedProcedureType: string;
    connectionString: string;
    // Ignoring PackageFileName
    settingPersistence: string;
}

async function importSqlDatabase(element: MacroApi.Context.IElementApi): Promise<void> {

    var defaults = getDialogDefaults(element);

    let connectionString: IDynamicFormFieldConfig = {
        id: "connectionString",
        fieldType: "text",
        label: "Connection String",
        placeholder: null,
        hint: null,
        isRequired: true,
        value: defaults.connectionString
    };

    let tableStereotypes: IDynamicFormFieldConfig = {
        id: "tableStereotypes",
        fieldType: "select",
        label: "Apply Table Stereotypes",
        placeholder: "",
        hint: "When to apply Table stereotypes to your domain entities",
        value: defaults.tableStereotypes,
        selectOptions: [{ id: "WhenDifferent", description: "If They Differ" }, { id: "Always", description: "Always" }]
    };

    let entityNameConvention: IDynamicFormFieldConfig = {
        id: "entityNameConvention",
        fieldType: "select",
        label: "Entity name convention",
        placeholder: "",
        hint: "",
        value: defaults.entityNameConvention,
        selectOptions: [{ id: "SingularEntity", description: "Singularized table name" }, { id: "MatchTable", description: "Table name, as is" }]
    };

    let schemaFilter: IDynamicFormFieldConfig = {
        id: "schemaFilter",
        fieldType: "text",
        label: "Schema Filter",
        placeholder: "all",
        hint: "Specify which SQL schemas to export. (default is all) e.g dbo;accounts;security",
        value: defaults.schemaFilter
    };

    let includeTables: IDynamicFormFieldConfig = {
        id: "includeTables",
        fieldType: "checkbox",
        label: "Include Tables",
        hint: "Export SQL tables",
        value: defaults.includeTables
    };

    let includeViews: IDynamicFormFieldConfig = {
        id: "includeViews",
        fieldType: "checkbox",
        label: "Include Views",
        hint: "Export SQL views",
        value: defaults.includeViews
    };

    let includeStoredProcedures: IDynamicFormFieldConfig = {
        id: "includeStoredProcedures",
        fieldType: "checkbox",
        label: "Include Stored Procedures",
        hint: "Export SQL stored procedures",
        value: defaults.includeStoredProcedures
    };

    let includeIndexes: IDynamicFormFieldConfig = {
        id: "includeIndexes",
        fieldType: "checkbox",
        label: "Include Indexes",
        hint: "Export SQL indexes",
        value: defaults.includeIndexes
    };

    let settingPersistence: IDynamicFormFieldConfig = {
        id: "settingPersistence",
        fieldType: "select",
        label: "Persist Settings",
        hint: "Remember these settings for next time you run the import",
        value: defaults.settingPersistence,
        selectOptions: [
            { id: "None", description: "(None)" }, 
            { id: "All", description: "All Settings" },
            { id: "AllSanitisedConnectionString", description: "All (with Sanitized connection string, no password))" }, 
            { id: "AllWithoutConnectionString", description: "All (without connection string))" }
        ]
    };

    let tableViewFilterFilePath: IDynamicFormFieldConfig = {
        id: "tableViewFilterFilePath",
        fieldType: "text",
        label: "Table / View Filter",
        hint: "Import selection file path (Tables/Views, one per line)",
        placeholder: "(optional)",
        value: defaults.tableViewFilterFilePath
    };

    let storedProcedureType: IDynamicFormFieldConfig = {
        id: "storedProcedureType",
        fieldType: "select",
        label: "Stored Procedure Representation",
        value: defaults.storedProcedureType,
        selectOptions: [
            {id: "Default", description: "(Default)"}, 
            {id: "StoredProcedureElement", description: "Stored Procedure Element"}, 
            {id: "RepositoryOperation", description: "Stored Procedure Operation"}
        ]
    };

    let formConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "Sql Server Import",
        fields: [
            connectionString,
            entityNameConvention,
            tableStereotypes,
            schemaFilter,
            includeTables,
            includeViews,
            includeStoredProcedures,
            includeIndexes,
            tableViewFilterFilePath,
            storedProcedureType,
            settingPersistence
        ]
    }

    let inputs = await dialogService.openForm(formConfig);

    const typesToExport: string[] = [];
    if (inputs.includeTables == "true") {
        typesToExport.push("Table");
    }
    if (inputs.includeViews == "true") {
        typesToExport.push("View");
    }
    if (inputs.includeStoredProcedures == "true") {
        typesToExport.push("StoredProcedure");
    }
    if (inputs.includeIndexes == "true") {
        typesToExport.push("Index");
    }

    const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

    let importConfig: IDatabaseImportModel = {
        applicationId: application.id,
        designerId: domainDesignerId,
        packageId: element.getPackage().id,
        entityNameConvention: inputs.entityNameConvention,
        tableStereotype: inputs.tableStereotypes,
        typesToExport: typesToExport,
        schemaFilter: inputs.schemaFilter ? inputs.schemaFilter.split(";") : [],
        tableViewFilterFilePath: inputs.tableViewFilterFilePath,
        storedProcedureType: inputs.storedProcedureType,
        connectionString: inputs.connectionString,
        settingPersistence: inputs.settingPersistence
    };
    let jsonResponse = await executeModuleTask("Intent.Modules.SqlServerImporter.Tasks.DatabaseImport", JSON.stringify(importConfig));
    let result = JSON.parse(jsonResponse);
    if (result?.errorMessage) {
        await dialogService.error(result?.errorMessage);
    } else {
        if (result?.warnings) {
            await dialogService.warn("Import complete.\r\n\r\n" + result?.warnings);
        } else {
            await dialogService.info("Import complete.");
        }
    }

}

function getDialogDefaults(element: MacroApi.Context.IElementApi): ISqlImportPackageSettings {

    let package = element.getPackage();
    let persistedValue = getSettingValue(package, "sql-import:typesToExport", "");
    let includeTables = "true";
    let includeViews = "true";
    let includeStoredProcedures = "true";
    let includeIndexes = "true";
    if (persistedValue) {
        includeTables = "false";
        includeViews = "false";
        includeStoredProcedures = "false";
        includeIndexes = "false";
        persistedValue.split(";").forEach(i => {
            switch (i.toLocaleLowerCase()) {
                case 'table':
                    includeTables = "true";
                    break;
                case 'view':
                    includeViews = "true";
                    break;
                case 'storedprocedure':
                    includeStoredProcedures = "true";
                    break;
                case 'index':
                    includeIndexes = "true";
                    break;
                default:
                    break;
            }
        });
    }
    let result: ISqlImportPackageSettings = {
        
        entityNameConvention: getSettingValue(package, "sql-import:entityNameConvention", "SingularEntity"),
        tableStereotypes: getSettingValue(package, "sql-import:tableStereotypes", "WhenDifferent"),
        schemaFilter: getSettingValue(package, "sql-import:schemaFilter", ""),
        includeTables: includeTables,
        includeViews: includeViews,
        includeStoredProcedures: includeStoredProcedures,
        includeIndexes: includeIndexes,
        tableViewFilterFilePath: getSettingValue(package, "sql-import:tableViewFilterFilePath", null),
        connectionString: getSettingValue(package, "sql-import:connectionString", null),
        storedProcedureType: getSettingValue(package, "sql-import:storedProcedureType", ""),
        settingPersistence: getSettingValue(package, "sql-import:settingPersistence", "None")
    };
    return result;
}

function getSettingValue(package: MacroApi.Context.IPackageApi, key: string, defaultValue: string): string {
    let persistedValue = package.getMetadata(key);
    return persistedValue ? persistedValue : defaultValue;
}


/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.SqlServerImporter
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/sql-importer/sql-server/database/sql-importer-database.ts
 */

//Uncomment below
//await importSqlDatabase(element);