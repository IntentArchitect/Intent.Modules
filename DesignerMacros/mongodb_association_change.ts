(async () => {
// This script was made using a Typescript source. Don't edit this script directly.
{
    let targetClass = association.typeReference.getType();

    if (targetClass.getPackage().specialization !== "Mongo Domain Package") {
        return;
    }

    updatePrimaryKey(targetClass);
    updateForeignKeys(association);
}

function updatePrimaryKey(element : MacroApi.Context.IElementApi) {
    const PrimaryKeyStereotypeId = "b99aac21-9ca4-467f-a3a6-046255a9eed6";
    let pk = element.getChildren("Attribute").filter(x => x.hasMetadata("is-managed-key") && !x.hasMetadata("association"))[0];

    if (pk && !isAggregateRoot(element)) {
        pk.delete();
        return;
    }

    if (pk && pk.hasStereotype(PrimaryKeyStereotypeId)) {
        return;
    }

    let idAttr = pk || createElement("Attribute", "Id", element.id);
    idAttr.setOrder(0);
    idAttr.typeReference.setType(getDefaultIdType());
    if (!idAttr.hasMetadata("is-managed-key")) {
        idAttr.addMetadata("is-managed-key", "true");
    }
    if (!idAttr.hasStereotype(PrimaryKeyStereotypeId)) {
        idAttr.addStereotype(PrimaryKeyStereotypeId);
    }
}

function updateForeignKeys(associationEnd : MacroApi.Context.IAssociationApi) {
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

function updateForeignKeyAttribute(startingEndType : MacroApi.Context.IElementApi, destinationEndType : MacroApi.Context.IElementApi, associationEnd : MacroApi.Context.IAssociationApi, associationId: string) {
    const ForeignKeyStereotypeId = "793a5128-57a1-440b-a206-af5722b752a6";
    let primaryKeyDict = getPrimaryKeysWithMapPath(destinationEndType);
    let primaryKeyObjects = Object.values(primaryKeyDict);
    let primaryKeysLen = primaryKeyObjects.length;
    primaryKeyObjects.forEach((pk, index) => {
        let fk = startingEndType.getChildren().filter(x => x.getMetadata("association") == associationId)[index] ||
                createElement("Attribute", "", startingEndType.id);
        // This check to avoid a loop where the Domain script is updating the conventions and this keeps renaming it back.
        let fkNameToUse = `${toCamelCase(associationEnd.getName())}${toPascalCase(pk.name)}`;
        if (associationEnd.typeReference.isCollection) {
            fkNameToUse = pluralize(fkNameToUse);
        }
        if (fk.getName().toLocaleLowerCase() !== fkNameToUse.toLocaleLowerCase()) {
            if (!fk.hasMetadata("fk-original-name") || (fk.getMetadata("fk-original-name") == fk.getName())) {
                fk.setName(fkNameToUse);
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
        fkStereotype.getProperty("Association").setValue(associationId);

        fk.typeReference.setType(pk.typeId);
        fk.typeReference.setIsNullable(associationEnd.typeReference.isNullable);
        fk.typeReference.setIsCollection(associationEnd.typeReference.isCollection);
    });
    startingEndType.getChildren().filter(x => x.getMetadata("association") == associationId).forEach((attr, index) => {
        if (index >= primaryKeysLen) {
            attr.delete();
        }
    });
}

function requiresForeignKey(associationEnd : MacroApi.Context.IAssociationApi) : boolean {
    return associationEnd.typeReference.isNavigable && 
        isAggregateRelationship(associationEnd);
}

function isAggregateRelationship(associationEnd : MacroApi.Context.IAssociationApi) : boolean {
    let sourceAssociationEnd = associationEnd;
    if (associationEnd.isTargetEnd()) {
        sourceAssociationEnd = sourceAssociationEnd.getOtherEnd();
    }
    return sourceAssociationEnd.typeReference.isNullable || sourceAssociationEnd.typeReference.isCollection;
}

function isAggregateRoot(element : MacroApi.Context.IElementApi) {
    return !element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
}

function getDefaultIdType() : string  {
    const StringTypeId : string = "d384db9c-a279-45e1-801e-e4e8099625f2";
    const GuidTypeId : string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
    const IntTypeId : string = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74";
    const LongTypeId : string = "33013006-E404-48C2-AC46-24EF5A5774FD";
    const MongoSettingId : string = "d5581fe8-7385-4bb6-88dc-8940e20ec1d4";
    
    switch (application.getSettings(MongoSettingId)?.getField("Id Type")?.value) {
        default:
            return StringTypeId;
        case "guid":
            return GuidTypeId;
        case "int":
            return IntTypeId;
        case "long":
            return LongTypeId;
    }
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