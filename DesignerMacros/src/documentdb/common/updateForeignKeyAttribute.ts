/// <reference path="../common/getPrimaryKeysWithMapPath.ts" />

function updateForeignKeyAttribute(startingEndType : MacroApi.Context.IElementApi, destinationEndType : MacroApi.Context.IElementApi, associationEnd : MacroApi.Context.IAssociationApi, associationId: string) {
    const ForeignKeyStereotypeId = "ced3e970-e900-4f99-bd04-b993228fe17d";
    let primaryKeyDict = getPrimaryKeysWithMapPath(destinationEndType);
    let primaryKeyObjects = Object.values(primaryKeyDict);
    let primaryKeysLen = primaryKeyObjects.length;
    primaryKeyObjects.forEach((pk, index) => {
        let fk = startingEndType.getChildren()
            .filter(x => (x.getMetadata("association") == associationId) || (x.hasStereotype(ForeignKeyStereotypeId) && !x.hasMetadata("association")))[index] || 
                createElement("Attribute", "", startingEndType.id);
        // This check to avoid a loop where the Domain script is updating the conventions and this keeps renaming it back.
        let fkNameToUse = `${toCamelCase(associationEnd.getName())}${toPascalCase(pk.name)}`;
        if (associationEnd.typeReference.isCollection) {
            fkNameToUse = pluralize(fkNameToUse);
        }
        if (fk.getName().toLocaleLowerCase() !== fkNameToUse.toLocaleLowerCase()) {
            if (!fk.hasMetadata("fk-original-name") || (fk.getMetadata("fk-original-name") == fk.getName())) {
                if (fkNameToUse != fk.getName()) {
                    fk.setName(fkNameToUse);
                }
                fk.setMetadata("fk-original-name", fk.getName());
            }
        }
        fk.setMetadata("association", associationId);
        fk.setMetadata("is-managed-key", "true");
        
        let fkStereotype = fk.getStereotype(ForeignKeyStereotypeId);
        if (!fkStereotype) {
            fk.addStereotype(ForeignKeyStereotypeId);
            fkStereotype = fk.getStereotype(ForeignKeyStereotypeId);
        }
        if (fkStereotype.getProperty("Association").getValue() != associationId) {
            fkStereotype.getProperty("Association").setValue(associationId);
        }

        if (fk.typeReference == null) throw new Error("typeReference is undefined");

        if (fk.typeReference.typeId != pk.typeId) {
            fk.typeReference.setType(pk.typeId);
        }
        if (fk.typeReference.isNullable != associationEnd.typeReference.isNullable) {
            fk.typeReference.setIsNullable(associationEnd.typeReference.isNullable);
        }
        if (fk.typeReference.isCollection != associationEnd.typeReference.isCollection) {
            fk.typeReference.setIsCollection(associationEnd.typeReference.isCollection);
        }
    });
    startingEndType.getChildren().filter(x => x.getMetadata("association") == associationId).forEach((attr, index) => {
        if (index >= primaryKeysLen) {
            attr.delete();
        }
    });
}
