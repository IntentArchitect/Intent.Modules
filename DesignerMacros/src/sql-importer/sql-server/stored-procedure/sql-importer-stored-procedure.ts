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

interface IStoredProcListResultModel {
    storedProcs: { [key: string]: string[] };
}

async function openStoredProcedureBrowseDialog(connectionString: string): Promise<string[]> {
    const storedProcIcon: string = "data:image/png;base64,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";
    const schemaIcon: string = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAANpJREFUOE+tk0ESgjAMRX9mdC9cRG4ALPA8ehI8jyzAG8AJHC8A7mUmTklbgRFt1a4IbV9/8hOCWWW3wR1bG6uPNRqkwW3ybxaQjYsuAbic7lOKLKj8AFko0KJl4BsAYF5M/ACnLgLxEUCkJddg2mMX1G4pmFNFKwqyMHl30ey5FVGg8QSoa/UCQKkcHBxR8pWNEQjXIaUhRc6xCGAS6cQVCA1YA4CLBahaLSsYC9U2OqXwcxHFxnxm48Hdxmcrn7WY2K+RDOAPrTyqos8sqHHubRsLZIX60zg/AOCebhHZxIffAAAAAElFTkSuQmCC";

    let storedProcSelection: IDynamicFormFieldConfig = {
        id: "storedProcSelection",
        fieldType: "tree-view",
        label: "Stored Procedure Selection",
        isRequired: true,
        treeViewOptions: {
            isMultiSelect: true,
            selectableTypes: [
                {
                    specializationId: "Schema",
                    autoExpand: true,
                    autoSelectChildren: true
                },
                {
                    specializationId: "Stored-Procedure",
                    isSelectable: true
                }
            ]
        }
    };

    try {
        let jsonResponse = await executeModuleTask("Intent.Modules.SqlServerImporter.Tasks.StoredProcList", JSON.stringify({"connectionString": connectionString}));
        let spListResult = JSON.parse(jsonResponse) as IStoredProcListResultModel;
        
        let schemaTree = Object.keys(spListResult.storedProcs).map(schemaName => {
            return {
                id: `schema.${schemaName}`,
                label: schemaName,
                specializationId: "Schema",
                icon: schemaIcon,
                children: spListResult.storedProcs[schemaName].map(sp => {
                    return {
                        id: `sp.${schemaName}.${sp}`,
                        label: sp,
                        specializationId: "Stored-Procedure",
                        icon: storedProcIcon
                    } as MacroApi.Context.ISelectableTreeNode;
                })
            } as MacroApi.Context.ISelectableTreeNode;
        });
                        
        storedProcSelection.treeViewOptions.rootNode = {
            id: "database",
            specializationId: "Database",
            label: "Database",
            children: schemaTree
        };
    } catch (e) {
        await dialogService.error(e);
        return [];
    }

    let browseFormConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "Browse Stored Procedures",
        fields: [
            storedProcSelection
        ]
    };

    let browseInputs = await dialogService.openForm(browseFormConfig);
    
    if (browseInputs && browseInputs.storedProcSelection) {
        let selection = browseInputs.storedProcSelection as string[];
        let filteredSelection = selection.filter(x => !x.startsWith("schema."));
        return filteredSelection
            .map(x => {
                let parts = x.split(".");
                return `${parts[1]}.${parts[2]}`;
            });
    }
    
    return [];
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
        placeholder: "Enter stored procedure names (comma-separated) or use Browse button",
        value: defaults.storedProcNames,
        isRequired: true
    };

    let storedProcBrowseButton: IDynamicFormFieldConfig = {
        id: "storedProcBrowse",
        fieldType: "button",
        label: "Browse",
        onClick: async (form: MacroApi.Context.IDynamicFormApi) => {
            const connectionStringValue = form.getField("connectionString").value;
            const connectionStringStr = Array.isArray(connectionStringValue) ? connectionStringValue[0] : connectionStringValue;
            if (!connectionStringStr || connectionStringStr.trim() === "") {
                await dialogService.error("Please enter a connection string before browsing stored procedures.");
                return;
            }

            try {
                const selectedProcs = await openStoredProcedureBrowseDialog(connectionStringStr);
                if (selectedProcs.length > 0) {
                    const storedProcNamesField = form.getField("storedProcNames");
                    storedProcNamesField.value = selectedProcs.join(", ");
                }
            } catch (e) {
                await dialogService.error("Error browsing stored procedures: " + e);
            }
        }
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
            storedProcBrowseButton,
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
