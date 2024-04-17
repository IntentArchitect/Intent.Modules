/// <reference path="../../typings/elementmacro.context.api.d.ts" />

type IDynamicFormFieldConfig = MacroApi.Context.IDynamicFormFieldConfig;
interface IOpenApiImporterSettings {
    openApiFile: string;
    addPostFixes: string;
    serviceType: string;
    allowRemoval: string;
    settingPersistence: string;
}

interface IImportConfig {
    openApiSpecificationFile: string;
    packageId: string;
    targetFolderId: string;
    addPostFixes: boolean;
    isAzureFunctions: boolean;
    serviceType: string;
    allowRemoval: boolean;
    settingPersistence: string;
}

async function  importOpenApi(element: MacroApi.Context.IElementApi): Promise<void>{   

    var defaults = getDialogDefaults(element);

    let openApiFile: IDynamicFormFieldConfig = {
        id: "openApiFile", 
            fieldType: "text", 
            label: "OpenApi File",
            placeholder: null,
            hint: "Path to file, including name.",
            isRequired: true,
            value: defaults.openApiFile 
    };
        
    let serviceType: IDynamicFormFieldConfig = {
        id: "serviceType", 
            fieldType: "select", 
            label: "Service Paradigm",
            placeholder: "",
            hint: "Which service paradigm would you like",
            value: defaults.serviceType,
            selectOptions: [{id:"CQRS", description:"CQRS"}, {id:"Service", description:"Traditional Services"}]
    };
        
    let allowRemoval: IDynamicFormFieldConfig = {
        id: "allowRemoval", 
            fieldType: "checkbox", 
            label: "Allow Removal",
            hint: "Remove previously imported elements?",
            value: defaults.allowRemoval 
    };

    let addPostfixes: IDynamicFormFieldConfig = {
        id: "addPostfixes", 
            fieldType: "checkbox", 
            label: "Add Postfixes",
            hint: "Add postfixes of Commands/Query/Services if they are missing.",
            value: defaults.addPostFixes 
    };

    let settingPersistence: IDynamicFormFieldConfig = {
        id: "settingPersistence", 
            fieldType: "select", 
            label: "Persist Settings",
            hint: "Remember these settings for next time you run the import",
            value: defaults.settingPersistence,
            selectOptions: [{id:"None", description:"(None)"}, {id:"All", description:"All Settings"}]
    };    
    
    let formConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "OpenApi Import", 
            fields: [
                openApiFile, 
                serviceType, 
                allowRemoval, 
                addPostfixes,
                settingPersistence,
            ]
    }
            
    let inputs = await dialogService.openForm(formConfig);    

    const typesToExport: string[] = [];

    let importConfig : IImportConfig = {
        openApiSpecificationFile: inputs.openApiFile,
        packageId: element.getPackage().id,
        isAzureFunctions: false,//service sets this based on installed modules
        addPostFixes: inputs.addPostfixes === "true",
        allowRemoval: inputs.allowRemoval === "true",
        serviceType: inputs.serviceType,
        targetFolderId: element.specialization === "Folder" ? element.id : null,
        settingPersistence: inputs.settingPersistence,        
    };
    let jsonResponse = await executeModuleTask("Intent.Modules.OpenApi.Importer.Tasks.OpenApiImport", JSON.stringify(importConfig));
    let result = JSON.parse(jsonResponse);
    if (result?.errorMessage){
        await dialogService.error(result?.errorMessage);
    } else {
        if (result?.warnings){
            await dialogService.warn("Import complete.\r\n\r\n" + result?.warnings);
        }else{
            await dialogService.info("Import complete.");
        }
    }

}

function getDialogDefaults(element: MacroApi.Context.IElementApi) : IOpenApiImporterSettings{
    
    let package = element.getPackage();
    let result: IOpenApiImporterSettings = {
        openApiFile: getSettingValue(package, "open-api-import:open-api-file", null),
        addPostFixes: getSettingValue(package, "open-api-import:add-postfixes", "true"),
        allowRemoval: getSettingValue(package, "open-api-import:allow-removal", "true"),
        serviceType: getSettingValue(package, "open-api-import:service-type", "CQRS"),
        settingPersistence: getSettingValue(package, "open-api-import:setting-persistence", "None"),
        };
    return result;
}

function getSettingValue(package: MacroApi.Context.IPackageApi, key: string, defaultValue : string) : string{
    let persistedValue = package.getMetadata(key);
    return persistedValue ? persistedValue : defaultValue;
}


/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.OpenApi.Imported
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/opem-api-importer/open-api-importer.ts
 */

//Uncomment below
//await importOpenApi(element);