/// <reference path="../common/requiresForeignKey.ts" />
/// <reference path="../common/updateForeignKeyAttribute.ts" />

function updateForeignKeysForElement(element: MacroApi.Context.IElementApi) {
    for (let association of element.getAssociations()) {
        if (!association.isTargetEnd()) {
            continue;
        }

        let sourceType = lookup(association.getOtherEnd().typeReference.typeId);
        let targetType = lookup(association.typeReference.typeId);
        if (!sourceType || !targetType) {
            continue;
        }

        if (requiresForeignKey(association)) {
            updateForeignKeyAttribute(sourceType, targetType, association, association.id);
        }
        if (requiresForeignKey(association.getOtherEnd())) {
            updateForeignKeyAttribute(targetType, sourceType, association.getOtherEnd(), association.id);
        }
    }

    removeOrphanedAssociations(element);
}

function removeOrphanedAssociations(element: MacroApi.Context.IElementApi) {
    let existingAssociations = element.getAssociations();
    let existingAttributes = element.getChildren("Attribute");
    for (let attr of existingAttributes) {
        if (! attr.getMetadata("association")) {
            continue;
        }
        if (! existingAssociations.some(x => attr.getMetadata("association") == x.id)) {
            attr.delete();
        }
    }
}

