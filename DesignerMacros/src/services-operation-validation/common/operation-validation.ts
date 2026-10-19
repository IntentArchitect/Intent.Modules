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

    result += params.map((p) => calculateTypeSignature(p.typeReference.getType(), p.typeReference.toModel())).join(', ');
    result += ")";

    return result;
}

// C# only overloads on nullability for value types, and a nullable collection is the collection being nullable.
function calculateTypeSignature (type: MacroApi.Context.IElementApi | null, typeReference: MacroApi.Context.ITypeReferenceData): string{
    let result = type?.getName() ?? typeReference.typeId;

    let genericTypeParameters = typeReference.genericTypeParameters ?? [];
    if (genericTypeParameters.length > 0){
        result += `<${genericTypeParameters.map((x) => calculateTypeSignature(lookup(x.typeId), x)).join(', ')}>`;
    }

    if (typeReference.isCollection){
        return `${result}[]`;
    }

    if (typeReference.isNullable && isValueType(type)){
        result += "?";
    }

    return result;
}

function isValueType (type: MacroApi.Context.IElementApi | null): boolean{
    if (type == null)
        return false;
    if (type.specialization == "Enum")
        return true;
    // Mirrors CSharpType.NonNullableValueTypes in Intent.Modules.Common.CSharp, using designer type names.
    let valueTypeNames = ["bool", "byte", "sbyte", "char", "date", "dateonly", "datetime", "datetimeoffset", "decimal", "double", "float", "guid", "int", "uint", "long", "ulong", "short", "ushort", "timeonly", "timespan"];
    return type.specialization == "Type-Definition" && valueTypeNames.includes(type.getName().toLowerCase());
}

function findPeerOperations (operation: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi[]{   
    return operation.getParent().getChildren("Operation").filter(x => x.id != operation.id);
}