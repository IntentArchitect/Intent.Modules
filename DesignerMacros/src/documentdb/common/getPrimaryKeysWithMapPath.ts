/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

interface IAttributeWithMapPath {
    id: string,
    name: string,
    typeId: string,
    mapPath: string[],
    isNullable: boolean,
    isCollection: boolean
};

function getPrimaryKeysWithMapPath(entity: MacroApi.Context.IElementApi) {
    let keyDict: { [characterName: string]: IAttributeWithMapPath } = Object.create(null);
    let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));

    let generalizations = entity.getAssociations("Generalization").filter(x => x.isTargetEnd());
    // There is a problem with execution order where this script executes before
    // the generalization script had a chance to potentially remove a PK attribute
    // and so I have to perform an inheritance check and ignore any PKs on derived classes.
    if (generalizations.length == 0) {
        keys.forEach(key => {
            if (key.typeReference == null) throw new Error("typeReference is undefined")

            return keyDict[key.id] = {
                id: key.id,
                name: key.getName(),
                typeId: key.typeReference.typeId,
                mapPath: [key.id],
                isNullable: false,
                isCollection: false
            };
        });
    }

    traverseInheritanceHierarchyForPrimaryKeys(keyDict, entity, []);

    return keyDict;

    function traverseInheritanceHierarchyForPrimaryKeys(
        keyDict: { [characterName: string]: IAttributeWithMapPath },
        curEntity: MacroApi.Context.IElementApi,
        generalizationStack: string[]
    ) {
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
            if (key.typeReference == null) throw new Error("typeReference is undefined")

            keyDict[key.id] = {
                id: key.id,
                name: key.getName(),
                typeId: key.typeReference.typeId,
                mapPath: generalizationStack.concat([key.id]),
                isNullable: key.typeReference.isNullable,
                isCollection: key.typeReference.isCollection
            };
        });
        traverseInheritanceHierarchyForPrimaryKeys(keyDict, nextEntity, generalizationStack);
    }
}
