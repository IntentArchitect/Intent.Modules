/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />

type IDynamicFormFieldConfig = MacroApi.Context.IDynamicFormFieldConfig;

interface ISqlImportPackageSettings {
    entityNameConvention: string;
    tableStereotypes: string;
    includeTables: string;
    includeViews: string;
    includeStoredProcedures: string;
    includeIndexes: string;
    importFilterFilePath: string;
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
    importFilterFilePath: string;
    storedProcedureType: string;
    connectionString: string;
    // Ignoring PackageFileName
    settingPersistence: string;
}


function createConnectionStringSectionFields(defaults: ISqlImportPackageSettings): IDynamicFormFieldConfig[] {
    return [
        {
            id: "connectionString",
            fieldType: "text",
            label: "Connection String",
            placeholder: null,
            hint: null,
            isRequired: true,
            value: defaults.connectionString
        },
        {
            id: "connectionString-Test",
            fieldType: "button",
            label: "Test Connection",
            onClick: async (form: MacroApi.Context.IDynamicFormApi) => {
                let input = form.getField("connectionString").value;
                let arg = JSON.stringify({
                    connectionString: input
                });
                let taskResult = await executeModuleTask("Intent.Modules.SqlServerImporter.Tasks.TestConnection", arg);
                let connectionResult = JSON.parse(taskResult);
                if (connectionResult?.success) {
                    await dialogService.info("Connection was successful.");
                } else {
                    await dialogService.error(connectionResult.message);
                }
            }
        }
    ];
}

function createImportSettingsSectionFields(defaults: ISqlImportPackageSettings): IDynamicFormFieldConfig[] {
    return [
        {
            id: "entityNameConvention",
            fieldType: "select",
            label: "Entity name convention",
            placeholder: "",
            hint: "",
            value: defaults.entityNameConvention,
            selectOptions: [{ id: "SingularEntity", description: "Singularized table name" }, { id: "MatchTable", description: "Table name, as is" }]
        },
        {
            id: "tableStereotypes",
            fieldType: "select",
            label: "Apply Table Stereotypes",
            placeholder: "",
            hint: "When to apply Table stereotypes to your domain entities",
            value: defaults.tableStereotypes,
            selectOptions: [{ id: "WhenDifferent", description: "If They Differ" }, { id: "Always", description: "Always" }]
        },
        {
            id: "includeTables",
            fieldType: "checkbox",
            label: "Include Tables",
            hint: "Export SQL tables",
            value: defaults.includeTables
        },
        {
            id: "includeViews",
            fieldType: "checkbox",
            label: "Include Views",
            hint: "Export SQL views",
            value: defaults.includeViews
        },
        {
            id: "includeStoredProcedures",
            fieldType: "checkbox",
            label: "Include Stored Procedures",
            hint: "Export SQL stored procedures",
            value: defaults.includeStoredProcedures
        },
        {
            id: "includeIndexes",
            fieldType: "checkbox",
            label: "Include Indexes",
            hint: "Export SQL indexes",
            value: defaults.includeIndexes
        },
        {
            id: "importFilterFilePath",
            fieldType: "open-file",
            label: "Import Filter File",
            hint: "Path to import filter JSON file (see [documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-sqlserverimporter/intent-sqlserverimporter.html#import-filter-file))",
            placeholder: "(optional)",
            value: defaults.importFilterFilePath,
            openFileOptions: {
                fileFilters: [{ name: "JSON", extensions: ["json"] }]
            }
        },
        {
            id: "createFilterFile",
            fieldType: "button",
            label: "Create Filter File",
            onClick: async () => {
                
            }
        },
        {
            id: "storedProcedureType",
            fieldType: "select",
            label: "Stored Procedure Representation",
            value: defaults.storedProcedureType,
            selectOptions: [
                { id: "Default", description: "(Default)" },
                { id: "StoredProcedureElement", description: "Stored Procedure Element" },
                { id: "RepositoryOperation", description: "Stored Procedure Operation" }
            ]
        }
    ];
}


async function importSqlDatabase(element: MacroApi.Context.IElementApi): Promise<void> {

    var defaults = getDialogDefaults(element);

    let formConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "Sql Server Import",
        fields: [
            {
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
            }
        ],
        sections: [
            {
                name: 'Connection string',
                fields: createConnectionStringSectionFields(defaults),
                isCollapsed: false,
                isHidden: false
            },
            {
                name: 'Import settings',
                fields: createImportSettingsSectionFields(defaults),
                isCollapsed: true,
                isHidden: false
            }
        ],
        height: "60%"
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
        importFilterFilePath: inputs.importFilterFilePath,
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
        includeTables: includeTables,
        includeViews: includeViews,
        includeStoredProcedures: includeStoredProcedures,
        includeIndexes: includeIndexes,
        importFilterFilePath: getSettingValue(package, "sql-import:importFilterFilePath", null),
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