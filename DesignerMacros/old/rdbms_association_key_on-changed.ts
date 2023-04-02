(async () => {
// This script was made using a Typescript source. Don't edit this script directly.
const foreignKeyStereotypeId = "793a5128-57a1-440b-a206-af5722b752a6";

if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit"){
    return;
}

let sourceEnd = association.getOtherEnd().typeReference;
if (sourceEnd.getType().getPackage().specialization !== "Domain Package") {
    return;
}

let sourceType = lookup(association.getOtherEnd().typeReference.typeId);
let targetType = lookup(association.typeReference.typeId);

if (sourceType && targetType) {

    if (requiresForeignKey(association) && sourceType.getMetadata("auto-manage-keys") != "false") {
        updateForeignKeyAttribute(sourceType, targetType, association, association.id);
    }
    else if (requiresForeignKey(association.getOtherEnd()) && targetType.getMetadata("auto-manage-keys") != "false") {
        updateForeignKeyAttribute(targetType, sourceType, association.getOtherEnd(), association.id);
    }
    else { // many-to-many
        if (targetType.getMetadata("auto-manage-keys") != "false") {
            targetType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
        }
        if (sourceType.getMetadata("auto-manage-keys") != "false") {
            sourceType.getChildren().filter(x => x.getMetadata("association") == association.id).forEach(x => x.delete());
        }
    }
}

function updateForeignKeyAttribute(startingEndType : MacroApi.Context.IElementApi, destinationEndType : MacroApi.Context.IElementApi, associationEnd : MacroApi.Context.IAssociationApi, associationId: string) {
    let primaryKeyDict = getPrimaryKeysWithMapPath(destinationEndType);
    let primaryKeyObjects = Object.values(primaryKeyDict);
    let primaryKeysLen = primaryKeyObjects.length;
    primaryKeyObjects.forEach((pk, index) => {
        let fk = startingEndType.getChildren().filter(x => x.getMetadata("association") == associationId)[index] ||
                createElement("Attribute", "", startingEndType.id);
        // This check to avoid a loop where the Domain script is updating the conventions and this keeps renaming it back.
        let fkNameToUse = `${toCamelCase(associationEnd.getName())}${toPascalCase(pk.name)}`;
        if (fk.getName().toLocaleLowerCase() !== fkNameToUse.toLocaleLowerCase()) {
            if (!fk.hasMetadata("fk-original-name") || (fk.getMetadata("fk-original-name") == fk.getName())) {
                fk.setName(fkNameToUse);
                fk.setMetadata("fk-original-name", fk.getName());
            }
        }
        fk.setMetadata("association", associationId);
        fk.setMetadata("is-managed-key", "true");
        
        let fkStereotype = fk.getStereotype(foreignKeyStereotypeId);
        if (!fkStereotype) {
            fk.addStereotype(foreignKeyStereotypeId);
            fkStereotype = fk.getStereotype(foreignKeyStereotypeId);
        }
        fkStereotype.getProperty("Association").setValue(associationId);

        fk.typeReference.setType(pk.typeId);
        fk.typeReference.setIsNullable(associationEnd.typeReference.isNullable);
    });
    startingEndType.getChildren().filter(x => x.getMetadata("association") == associationId).forEach((attr, index) => {
        if (index >= primaryKeysLen) {
            attr.delete();
        }
    });
    if (destinationEndType.id !== startingEndType.id && destinationEndType.getMetadata("auto-manage-keys") != "false") {
        destinationEndType.getChildren().filter(x => x.getMetadata("association") == associationId).forEach(x => x.delete());
    }
}

function requiresForeignKey(associationEnd : MacroApi.Context.IAssociationApi) : boolean {
    return isManyToVariantsOfOne(associationEnd) || isSelfReferencingZeroToOne(associationEnd);
}

function isManyToVariantsOfOne(associationEnd : MacroApi.Context.IAssociationApi) : boolean {
    return !associationEnd.typeReference.isCollection && associationEnd.getOtherEnd().typeReference.isCollection;
}

function isSelfReferencingZeroToOne(associationEnd : MacroApi.Context.IAssociationApi) : boolean {
    return !associationEnd.typeReference.isCollection && associationEnd.typeReference.isNullable && 
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
}

interface IAttributeWithMapPath {
    id: string,
    name: string,
    typeId: string,
    mapPath: string[],
    isNullable: boolean,
    isCollection: boolean
};

function getPrimaryKeysWithMapPath(entity : MacroApi.Context.IElementApi) {
    let keydict : { [characterName: string]: IAttributeWithMapPath } = Object.create(null);
    let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
    
    let generalizations = entity.getAssociations("Generalization").filter(x => x.isTargetEnd());
    // There is a problem with execution order where this script executes before
    // the generalization script had a chance to potentially remove a PK attribute
    // and so I have to perform an inheritance check and ignore any PKs on derived classes.
    if (generalizations.length == 0) {
        keys.forEach(key => keydict[key.id] = { 
            id: key.id, 
            name: key.getName(), 
            typeId: key.typeReference.typeId,
            mapPath: [key.id],
            isNullable: false,
            isCollection: false
        });
    }

    traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);

    return keydict;

    function traverseInheritanceHierarchyForPrimaryKeys(
        keydict: { [characterName: string]: IAttributeWithMapPath }, 
        curEntity: MacroApi.Context.IElementApi, 
        generalizationStack) {
        if (!curEntity) {
            return;
        }
        let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
        if (generalizations.length == 0) {
            return;
        }
        let generalization = generalizations[0];
        generalizationStack.push(generalization.id);
        let nextEntity = generalization.typeReference.getType();
        let baseKeys = nextEntity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        baseKeys.forEach(key => { 
            keydict[key.id] = { 
                id: key.id, 
                name: key.getName(),
                typeId: key.typeReference.typeId,
                mapPath: generalizationStack.concat([key.id]),
                isNullable: key.typeReference.isNullable,
                isCollection: key.typeReference.isCollection
            };
        });
        traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
    }
}

})();