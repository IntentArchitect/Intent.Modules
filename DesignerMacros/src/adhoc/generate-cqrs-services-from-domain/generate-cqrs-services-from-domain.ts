/// <reference path="../../services-cqrs-crud/create-crud-macro-advanced-mapping/create-crud-macro-advanced-mapping.ts"/>
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

async function runMe() {

    type IDynamicFormFieldConfig = MacroApi.Context.IDynamicFormFieldConfig;

    let classes = lookupTypesOf("Class").filter(x => DomainHelper.filterClassSelection(x, null));
    let folder = getPackages()[0];//getPackages()[0].getChildren("Folder")[0]  
    
    
    let skipField: IDynamicFormFieldConfig = {
        id: "skipField", 
            fieldType: "text", 
            label: "Skip",
            hint: "Entities to skip",
            value: "0" 
    };
    let takeField: IDynamicFormFieldConfig = {
        id: "takeField", 
            fieldType: "text", 
            label: "Take",
            hint: "Entities to process",
            value: "50" 
    };
    
    
    let formConfig: MacroApi.Context.IDynamicFormConfig = {
        title: "Generate Services", 
            fields: [
                skipField, 
                takeField, 
            ]
    }
            
    let inputs =  await dialogService.openForm(formConfig);    
        
    let skip = Number.parseInt(inputs.skipField);
    let take = Number.parseInt(inputs.takeField);
    
    for (let i = skip; i < Math.min( skip + take, classes.length); i++){
        execute(<any>folder, classes[i]);
    }
    
}
//await runMe();

