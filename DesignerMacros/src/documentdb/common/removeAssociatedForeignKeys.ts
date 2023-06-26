/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function removeAssociatedForeignKeys(associationEnd: MacroApi.Context.IAssociationApi) {
    let targetClass = associationEnd.typeReference.getType();
    let sourceClass = associationEnd.getOtherEnd().typeReference.getType();

    targetClass.getChildren()
        .filter(x => x.getMetadata("association") == associationEnd.id)
        .forEach(x => x.delete());

    sourceClass.getChildren()
        .filter(x => x.getMetadata("association") == associationEnd.id)
        .forEach(x => x.delete());
}
