/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

type IDynamicFormFieldConfig = MacroApi.Context.IDynamicFormFieldConfig;
interface ISqlImporterSettings {
    connectionString: string;
    tableStereotypes: string;
    entityNameConvention: string;
    schemaFilter: string;
    includeTables: string;
    includeViews: string;
    includeStoredProcedures: string;
    settingPersistence: string;
}

interface IImportConfig {
    applicationId: string;
    designerId: string;
    packageId: string;
    connectionString: string;
    tableStereotypes: string;
    entityNameConvention: string;
    schemaFilter: string[];
    typesToExport: string[];
    settingPersistence: string;
}

async function  importSqlDatabase(element: MacroApi.Context.IElementApi): Promise<void>{   

    var defaults = getDialogDefaults(element);

    let connectionString: IDynamicFormFieldConfig = {
        id: "connectionString", 
            fieldType: "text", 
            label: "Connection String",
            placeholder: null,
            hint: null,
            value: defaults.connectionString 
    };
        
    let tableStereotypes: IDynamicFormFieldConfig = {
        id: "tableStereotypes", 
            fieldType: "select", 
            label: "Apply Table Stereotypes",
            placeholder: "",
            hint: "When to apply Table stereotypes to your domain entities",
            value: defaults.tableStereotypes,
            selectOptions: [{id:"WhenDifferent", description:"If They Differ"}, {id:"Always", description:"Always"}]
    };
    
    let entityNameConvention: IDynamicFormFieldConfig = {
        id: "entityNameConvention", 
            fieldType: "select", 
            label: "Entity name convention",
            placeholder: "",
            hint: "",
            value: defaults.entityNameConvention,
            selectOptions: [{id:"SingularEntity", description:"Singularized table name"}, {id:"MatchTable", description:"Table name, as is"}]
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

    let settingPersistence: IDynamicFormFieldConfig = {
        id: "settingPersistence", 
            fieldType: "select", 
            label: "Persist Settings",
            hint: "Remember these settings for next time you run the import",
            value: defaults.settingPersistence,
            selectOptions: [{id:"None", description:"(None)"}, {id:"All", description:"All Settings"}, {id:"AllSanitisedConnectionString", description:"All (with Sanitized connection string, no password))"}, {id:"AllWithoutConnectionString", description:"All (without connection string))"}]
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
                settingPersistence,
            ]
    }
            
    let inputs = await dialogService.openForm(formConfig);    

    const typesToExport: string[] = [];
    if (inputs.includeTables == "true"){
        typesToExport.push("Table");
    }
    if (inputs.includeViews == "true"){
        typesToExport.push("View");
    }
    if (inputs.includeStoredProcedures == "true"){
        typesToExport.push("StoredProcedure");
    }

    const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

    let importConfig : IImportConfig = {
        applicationId: application.id,
        designerId: domainDesignerId,
        packageId: element.getPackage().id,
        connectionString: inputs.connectionString,
        tableStereotypes: inputs.tableStereotypes,
        entityNameConvention: inputs.entityNameConvention,
        schemaFilter: inputs.schemaFilter ? inputs.schemaFilter.split(";") : [],
        typesToExport: typesToExport,
        settingPersistence: inputs.settingPersistence,        
    };
    let jsonResponse = await executeModuleTask("Intent.Modules.SqlServerImporter.Tasks.DatabaseImport", JSON.stringify(importConfig));
    console.warn(jsonResponse);
    let result = JSON.parse(jsonResponse);
    if (result?.errorMessage){
        dialogService.error(result?.errorMessage);
    }
}

function getDialogDefaults(element: MacroApi.Context.IElementApi) : ISqlImporterSettings{
    
    let package = element.getPackage();
    let persistedValue = getSettingValue(package, "sql-import:typesToExport", "");
    let includeTables = "true";
    let includeViews = "true";
    let includeStoredProcedures = "true";
    if (persistedValue)
    {
        includeTables = "false";
        includeViews = "false";
        includeStoredProcedures = "false";
        persistedValue.split(";").forEach(i =>{
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
                default:
                    break;
            }        
        });
    }        
    let result: ISqlImporterSettings = {
        connectionString: getSettingValue(package, "sql-import:connectionString", null),
        tableStereotypes: getSettingValue(package, "sql-import:tableStereotypes", "WhenDifferent"),
        entityNameConvention: getSettingValue(package, "sql-import:entityNameConvention", "SingularEntity"),
        schemaFilter: getSettingValue(package, "sql-import:schemaFilter", ""),
        includeTables: includeTables,
        includeViews: includeViews,
        includeStoredProcedures: includeStoredProcedures,
        settingPersistence: getSettingValue(package, "sql-import:settingPersistence", "None"),
        };
    return result;
}

function getSettingValue(package: MacroApi.Context.IPackageApi, key: string, defaultValue : string) : string{
    let persistedValue = package.getMetadata(key);
    return persistedValue ? persistedValue : defaultValue;
}


/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.SqlServerImporter
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/sql-importer/sql-server/sql-importer.ts
 */

//Uncomment below
//await importSqlDatabase(element);