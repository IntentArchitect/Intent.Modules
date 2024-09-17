/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />

type IDynamicFormFieldConfig = MacroApi.Context.IDynamicFormFieldConfig;

interface ISqlImportPackageSettings {
    connectionString: string;
    storedProcedureType: string;
    storedProcNames: string;
    settingPersistence: string;
}

interface IDatabaseImportModel {
    applicationId: string;
    designerId: string;
    packageId: string;
    storedProcedureType: string;
    connectionString: string;
    storedProcNames: string[];
    repositoryElementId: string;
    settingPersistence: string;
}

async function importSqlStoredProcedure(element: MacroApi.Context.IElementApi): Promise<void> {

    var defaults = getDialogDefaults(element);

    let connectionString: IDynamicFormFieldConfig = {
        id: "connectionString",
        fieldType: "text",
        label: "Connection String",
        placeholder: "(optional if inherited setting)",
        hint: null,
        value: defaults.connectionString
    };

    let storedProcedureType: IDynamicFormFieldConfig = {
        id: "storedProcedureType",
        fieldType: "select",
        label: "Stored Procedure Representation",
        value: defaults.storedProcedureType,
        selectOptions: [
            {id: "", description: "(default or inherited setting)"},
            {id: "StoredProcedureElement", description: "Stored Procedure Element"},
            {id: "RepositoryOperation", description: "Stored Procedure Operation"}
        ]
    };

    let storedProcNames: IDynamicFormFieldConfig = {
        id: "storedProcNames",
        fieldType: "text",
        label: "Stored Procedure Names",
        hint: "Enter a comma-separated list of stored procedure names",
        value: "",
        isRequired: true
    };

    let settingPersistence: IDynamicFormFieldConfig = {
        id: "settingPersistence",
        fieldType: "select",
        label: "Persist Settings",
        hint: "Remember these settings for next time you run the import",
        value: defaults.settingPersistence,
        selectOptions: [
            { id: "None", description: "(None)" },
            { id: "InheritDb", description: "Inherit Database Settings" },
            { id: "All", description: "All Settings" },
            { id: "AllSanitisedConnectionString", description: "All (with Sanitized connection string, no password))" }, 
            { id: "AllWithoutConnectionString", description: "All (without connection string))" }
        ]
    };

    let formConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "Sql Server Import",
        fields: [
            connectionString,
            storedProcedureType,
            storedProcNames,
            settingPersistence
        ]
    }

    let inputs = await dialogService.openForm(formConfig);

    if (inputs.settingPersistence != "InheritDb" && (!inputs.connectionString || inputs.connectionString?.trim() === "")) {
        await dialogService.error("Connection String was not set.");
    }

    const storedProcNamesArray = inputs.storedProcNames.split(',').map((name: string) => name.trim());

    const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

    let importConfig: IDatabaseImportModel = {
        applicationId: application.id,
        designerId: domainDesignerId,
        packageId: element.getPackage().id,
        storedProcedureType: inputs.storedProcedureType,
        connectionString: inputs.connectionString,
        storedProcNames: storedProcNamesArray,
        repositoryElementId: element.id,
        settingPersistence: inputs.settingPersistence
    };
    
    let jsonResponse = await executeModuleTask("Intent.Modules.SqlServerImporter.Tasks.StoredProcedureImport", JSON.stringify(importConfig));
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

    let result: ISqlImportPackageSettings = {
        connectionString: getSettingValue(package, "sql-import-repository:connectionString", null),
        storedProcedureType: getSettingValue(package, "sql-import-repository:storedProcedureType", ""),
        storedProcNames: "",
        settingPersistence: getSettingValue(package, "sql-import-repository:settingPersistence", "None")
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
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/sql-importer/sql-server/stored-procedure/sql-importer-stored-procedure.ts
 */

//Uncomment below
//await importSqlStoredProcedure(element);