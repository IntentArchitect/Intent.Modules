(async () => {
// This script was made using a Typescript source. Don't edit this script directly.
if (element.specialization !== "Mongo Domain Package") {
    return;
}

let classes = lookupTypesOf("Class").filter(x => x.getPackage().id === element.id);
for (let classElement of classes) {
    const PrimaryKeyStereotypeId = "b99aac21-9ca4-467f-a3a6-046255a9eed6";
    let pk = classElement.getChildren("Attribute").filter(x => x.hasMetadata("id-managed"))[0];

    if (pk && !isAggregateRoot(classElement)) {
        pk.delete();
        continue;
    }

    if (pk && pk.hasStereotype(PrimaryKeyStereotypeId)) {
        continue;
    }

    let idAttr = pk || createElement("Attribute", "Id", classElement.id);
    idAttr.setOrder(0);
    idAttr.typeReference.setType(getDefaultIdType());
    if (!idAttr.hasMetadata("id-managed")) {
        idAttr.addMetadata("id-managed", "true");
    }
    if (!idAttr.hasStereotype(PrimaryKeyStereotypeId)) {
        idAttr.addStereotype(PrimaryKeyStereotypeId);
    }
}

const foreignKeyStereotypeId = "793a5128-57a1-440b-a206-af5722b752a6";

for (let classElement of classes) {
    for (let association of classElement.getAssociations()) {
        if (!association.isTargetEnd()) {
            continue;
        }
        
        let sourceType = lookup(association.getOtherEnd().typeReference.typeId);
        let targetType = lookup(association.typeReference.typeId);
        if (!sourceType || !targetType) {
            continue;
        }
        
        if (requiresForeignKey(association) && sourceType.getMetadata("auto-manage-keys") != "false") {
            updateForeignKeyAttribute(sourceType, targetType, association, association.id);
        }
        if (requiresForeignKey(association.getOtherEnd()) && targetType.getMetadata("auto-manage-keys") != "false") {
            updateForeignKeyAttribute(targetType, sourceType, association.getOtherEnd(), association.id);
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
        
        let fkStereotype = fk.getStereotype(foreignKeyStereotypeId);
        if (!fkStereotype) {
            fk.addStereotype(foreignKeyStereotypeId);
            fkStereotype = fk.getStereotype(foreignKeyStereotypeId);
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

function isManyToMany(associationEnd : MacroApi.Context.IAssociationApi) : boolean {
    return associationEnd.typeReference.isCollection && associationEnd.getOtherEnd().typeReference.isCollection;
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

function isAggregateRoot(element) {
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