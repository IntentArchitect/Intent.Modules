/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />

type IDynamicFormFieldConfig = MacroApi.Context.IDynamicFormFieldConfig;

interface ISqlImportPackageSettings {
    connectionString: string;
    storedProcedureType: string;
    storedProcNames: string;
}

interface IDatabaseImportModel {
    applicationId: string;
    designerId: string;
    packageId: string;
    storedProcedureType: string;
    connectionString: string;
    storedProcNames: string[];
    repositoryElementId: string;
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

    let storedProcedureType: IDynamicFormFieldConfig = {
        id: "storedProcedureType",
        fieldType: "select",
        label: "Stored Procedure Representation",
        value: defaults.storedProcedureType,
        selectOptions: [
            {id: "", description: "(Default)"},
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

    let formConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "Sql Server Import",
        fields: [
            connectionString,
            storedProcedureType,
            storedProcNames
        ]
    }

    let inputs = await dialogService.openForm(formConfig);

    const storedProcNamesArray = inputs.storedProcNames.split(',').map((name: string) => name.trim());

    const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

    let importConfig: IDatabaseImportModel = {
        applicationId: application.id,
        designerId: domainDesignerId,
        packageId: element.getPackage().id,
        storedProcedureType: inputs.storedProcedureType,
        connectionString: inputs.connectionString,
        storedProcNames: storedProcNamesArray,
        repositoryElementId: element.id
    };
    
    let jsonResponse = await executeModuleTask("Intent.Modules.SqlServerImporter.Tasks.RepositoryImport", JSON.stringify(importConfig));
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
        connectionString: getSettingValue(package, "sql-import:connectionString", null),
        storedProcedureType: getSettingValue(package, "sql-import:storedProcedureType", ""),
        storedProcNames: ""
    };
    return result;
}

function getSettingValue(package: MacroApi.Context.IPackageApi, key: string, defaultValue: string): string {
    let persistedValue = package.getMetadata(key);
    return persistedValue ? persistedValue : defaultValue;
}


// Uncomment below
//await importSqlDatabase(element);