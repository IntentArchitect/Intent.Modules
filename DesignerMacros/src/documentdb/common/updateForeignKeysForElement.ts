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
}
