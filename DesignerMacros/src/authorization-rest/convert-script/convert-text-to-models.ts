/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/authorization-rest/convert-script/convert-text-to-models.ts
 */

convertTextToModel(element);


function convertTextToModel(securityElement: MacroApi.Context.IElementApi): void {
    let commands = lookupTypesOf("Command").filter(c => hasRoleOrPolicy(c)).map(x => getSecurityStereotype(x));
    let queries = lookupTypesOf("Query").filter(c => hasRoleOrPolicy(c)).map(x => getSecurityStereotype(x));
    let operations = lookupTypesOf("Operation").filter(x => hasRoleOrPolicy(x)).map(x => getSecurityStereotype(x));
    let attributes = lookupTypesOf("Attribute").filter(x => hasRoleOrPolicy(x)).map(x => getSecurityStereotype(x));
    let allStereotypes = [...commands, ...queries, ...operations, ...attributes];
    
    allStereotypes.forEach(stereotype =>{
        if (hasConvertibleValues(stereotype.getProperty("Roles")?.value)){
            let roles = convertStringToArray(stereotype.getProperty("Roles").value);
            stereotype.getProperty("Roles").setValue("");
            if(arrayHasValues(roles)){
                let roleElements : MacroApi.Context.IElementApi[] = [];
                roles.forEach(role => roleElements.push(getOrCreateSecurityChildElement(securityElement, role, "Role")));
                stereotype.getProperty("Security Roles").setValue(convertElementsToIdString(roleElements));
            }
        }
        if (hasConvertibleValues(stereotype.getProperty("Policy")?.value)){
            let policies = convertStringToArray(stereotype.getProperty("Policy").value);
            stereotype.getProperty("Policy").setValue("");
            if(arrayHasValues(policies)){
                let policyElements : MacroApi.Context.IElementApi[] = [];
                policies.forEach(policy => policyElements.push(getOrCreateSecurityChildElement(securityElement, policy, "Policy")));
                stereotype.getProperty("Security Policies").setValue(convertElementsToIdString(policyElements));
            }
        }
    });
}

function hasConvertibleValues(value: string):boolean{
    if (!value || value == "")
        return false;
    if (value.includes('+'))
        return false;
    return true;
}

function convertElementsToIdString(objects : MacroApi.Context.IElementApi[]) {
    // Check if the input is a non-empty array
    if (Array.isArray(objects) && objects.length > 0) {
        // Map the array to extract the Id properties and join them into a string
        return '[' + objects.map(obj => "\"" + obj.id + "\"").join(',') + ']';
    } else {
        // Return an empty array representation if the input is empty or not an array
        return '[]';
    }
}

function arrayHasValues(arr:string[]):boolean {
    if (!arr)
        return false;
    return arr.some(item => item.trim().length > 0);
}

function convertStringToArray(inputString:string):string[] {
    // Check if the input string is not empty or null
    if (inputString) {
        // Use split to convert the string into an array, trimming whitespace around elements
        return inputString.split(',').map(item => item.trim());
    } else {
        // Return an empty array if the input string is empty or null
        return [];
    }
}

function getSecurityStereotype(x: MacroApi.Context.IElementApi): MacroApi.Context.IStereotypeApi{
    if (x.hasStereotype("Authorize")){
        return x.getStereotype("Authorize");
    }
    if (x.hasStereotype("Secured")){
        return x.getStereotype("Secured");
    }
    if (x.hasStereotype("Data Masking")){
        return x.getStereotype("Data Masking");
    }
    return null;
}


function hasRoleOrPolicy(x: MacroApi.Context.IElementApi): boolean{
    if (x.hasStereotype("Authorize")){
        var auth = x.getStereotype("Authorize");
        if (auth.getProperty("Roles")?.value){
            return true;
        }
        if (auth.getProperty("Policy")?.value){
            return true;
        }
    }
    if (x.hasStereotype("Secured")){
        var auth = x.getStereotype("Secured");
        if (auth.getProperty("Roles")?.value){
            return true;
        }
    }
    if (x.hasStereotype("Data Masking")){
        var dm = x.getStereotype("Data Masking");
        if (dm.getProperty("Roles")?.value){
            return true;
        }
        if (dm.getProperty("Policy")?.value){
            return true;
        }
    }
    return false;
}

function getOrCreateSecurityChildElement(securityElement: MacroApi.Context.IElementApi, elementName: string, elementType: string) : MacroApi.Context.IElementApi{

    let roleOrPolicy = securityElement.getChildren(elementType).find(x => x.getName() == elementName);
    if (!roleOrPolicy){
        //Create a new attribute
        roleOrPolicy = createElement(elementType, elementName, securityElement.id);
    }
    return roleOrPolicy;
}

