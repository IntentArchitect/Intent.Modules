/// <reference path="requiresForeignKey.ts" />
/// <reference path="updateForeignKeyAttribute.ts" />

function updateForeignKeysForAssociation(associationEnd: MacroApi.Context.IAssociationApi) {
    let sourceType = lookup(associationEnd.getOtherEnd().typeReference.typeId);
    let targetType = lookup(associationEnd.typeReference.typeId);
    if (!sourceType || !targetType) {
        return;
    }

    if (requiresForeignKey(associationEnd)) {
        updateForeignKeyAttribute(sourceType, targetType, associationEnd, associationEnd.id);
    } else {
        sourceType.getChildren()
            .filter(x => x.getMetadata("association") == associationEnd.id)
            .forEach(x => x.delete());
    }

    if (requiresForeignKey(associationEnd.getOtherEnd())) {
        updateForeignKeyAttribute(targetType, sourceType, associationEnd.getOtherEnd(), associationEnd.id);
    } else {
        targetType.getChildren()
            .filter(x => x.getMetadata("association") == associationEnd.id)
            .forEach(x => x.delete());
    }
}
