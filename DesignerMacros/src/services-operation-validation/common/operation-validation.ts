/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function validateDuplicateOperation(operation: MacroApi.Context.IElementApi): String{   
    var possibleDuplicates = findPeerOperations(operation);

    let operationSignature = calculateSignature(operation);
    let duplicate : MacroApi.Context.IElementApi | undefined;    
    possibleDuplicates.forEach(possibleDuplicate => {
        if (duplicate != null)
            return;
        if (operationSignature == calculateSignature(possibleDuplicate)){
            duplicate = possibleDuplicate;
        }
    });
    if (duplicate){
        return `Duplicate operation ${operation.getName()} - ${operationSignature}`;
    } 
    return "";
}

function calculateSignature (operation: MacroApi.Context.IElementApi): string{   
    let result = `${operation.getName()}(`
    let params = operation.getChildren("Parameter");

    result += params.map((p) => p.typeReference.getType().getName()).join(', ');
    result += ")";

    return result;
}

function findPeerOperations (operation: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi[]{   
    return operation.getParent().getChildren("Operation").filter(x => x.id != operation.id);
}