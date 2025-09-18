/// <reference path="getSurrogateKeyType.ts"/>
/// <reference path="attributeWithMapPath.ts"/>

interface IKeyChain {
    attribute: IElementApi; 
    expectedName: string; 
}

class DomainHelper {

    static isAggregateRoot(element: MacroApi.Context.IElementApi): boolean {
        let result = !element.getAssociations("Association")
            .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
        return result;
    }

    static getCommandOperations(entity: MacroApi.Context.IElementApi) : MacroApi.Context.IElementApi[] {
        const queryOperationNames = ["Get", "Find", "Filter", "Query", "Is", "Must", "Can"]
        const operations = entity.getChildren("Operation").filter(
            operation => operation.typeReference.getType() == null || 
            !queryOperationNames.some(allowedOperationName => operation.getName().startsWith(allowedOperationName)));
        return operations;
    }

    static isComplexType(element: MacroApi.Context.IElementApi): boolean {
        return element?.specialization === "Data Contract" ||
            element?.specialization === "Value Object" ||
            element?.specialization === "Class";
    }

    static isComplexTypeById(typeId: string): boolean {
        let element = lookup(typeId);
        return DomainHelper.isComplexType(element);
    }


    static getOwningAggregate(entity: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        
        let invalidAssociations = entity.getAssociations("Association").filter(x => x.typeReference.getType() == null);
        if (invalidAssociations.length > 0) {
            console.warn("Invalid associations found:")
            invalidAssociations.forEach(x => {
                console.warn("Invalid associations: " + x.getName())
            })
        }
        let result = entity.getAssociations("Association")
            .filter(x => this.isAggregateRoot(x.typeReference.getType()) && isOwnedBy(x) &&
                // Let's only target collections for now as part of the nested compositional crud support
                // as one-to-one relationships are more expensive to address and possibly not going to
                // be needed.
                x.getOtherEnd().typeReference.isCollection)[0]?.typeReference.getType();
        return result;

        function isOwnedBy(association: MacroApi.Context.IAssociationReadOnlyApi) {
            return association.isSourceEnd() &&
                !association.typeReference.isNullable &&
                !association.typeReference.isCollection;
        }
    }

    static ownerIsAggregateRoot(entity: MacroApi.Context.IElementApi): boolean {
        let result = DomainHelper.getOwningAggregate(entity);
        return result ? true : false;
    }

    static hasPrimaryKey(entity: MacroApi.Context.IElementApi): boolean {
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        return keys.length > 0;
    }

    static getPrimaryKeys(entity: MacroApi.Context.IElementApi): IAttributeWithMapPath[] {
        if (!entity) {
            throw new Error("entity not specified");
        }

        let primaryKeys = DomainHelper.getPrimaryKeysMap(entity);

        return Object.values(primaryKeys);
    }

    static isUserSuppliedPrimaryKey(pk: MacroApi.Context.IElementApi): boolean {
        if (pk == null) return false;
        if (!pk.hasStereotype("Primary Key")) return false;
        var pkStereotype = pk.getStereotype("Primary Key");
        if (!pkStereotype.hasProperty("Data source")) {
            return false;
        }
        return pkStereotype.getProperty("Data source").value == "User supplied";
    }

    static getPrimaryKeysMap(entity: MacroApi.Context.IElementApi): { [characterName: string]: IAttributeWithMapPath } {
        let keydict: { [characterName: string]: IAttributeWithMapPath } = Object.create(null);
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        keys.forEach(key => keydict[key.id] = {
            id: key.id,
            name: key.getName(),
            typeId: key.typeReference.typeId,
            typeReferenceModel: key.typeReference.toModel(),
            mapPath: [key.id],
            isNullable: false,
            isCollection: false
        });

        traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);

        return keydict;

        function traverseInheritanceHierarchyForPrimaryKeys(
            keydict: { [characterName: string]: IAttributeWithMapPath },
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
                keydict[key.id] = {
                    id: key.id,
                    name: key.getName(),
                    typeId: key.typeReference.typeId,
                    typeReferenceModel: key.typeReference.toModel(),
                    mapPath: generalizationStack.concat([key.id]),
                    isNullable: key.typeReference.isNullable,
                    isCollection: key.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
        }
    }

    static getForeignKeys(entity: MacroApi.Context.IElementApi, owningAggregate: MacroApi.Context.IElementApi): IAttributeWithMapPath[] {
        if (!entity) {
            throw new Error("entity not specified");
        }
        if (!owningAggregate) {
            throw new Error("nestedCompOwner not specified");
        }

        // Use the new Associated property on the FK stereotype method for FK Attribute lookup
        let foreignKeys: MacroApi.Context.IElementApi[] = [];
        for (let attr of entity.getChildren("Attribute").filter(x => x.hasStereotype("Foreign Key"))) {
            let associationId = attr.getStereotype("Foreign Key").getProperty("Association")?.getValue() as string;
            if (owningAggregate.getAssociations("Association").some(x => x.id == associationId)) {
                foreignKeys.push(attr)
            }
        }

        // Backward compatible lookup method
        if (foreignKeys.length == 0) {
            let foundFk = entity.getChildren("Attribute")
                .filter(x => x.getName().toLowerCase().indexOf(owningAggregate.getName().toLowerCase()) >= 0 && x.hasStereotype("Foreign Key"))[0];
            if (foundFk) {
                foreignKeys.push(foundFk);
            }
        }

        return foreignKeys.map(x => ({
            name: DomainHelper.getAttributeNameFormat(x.getName()),
            typeId: x.typeReference.typeId,
            typeReferenceModel: x.typeReference.toModel(),
            id: x.id,
            mapPath: [x.id],
            isCollection: x.typeReference.isCollection,
            isNullable: x.typeReference.isNullable,
            element: x
        }));
    }

    /**
     * Returns true if the attribute is a foreign key on a compositional one-to-many relationship (i.e. is managed by the DB and should not be set).
     * @param attribute 
     * @returns 
     */
    static isManagedForeignKey(attribute: IElementApi): boolean {
        let fkAssociation = attribute.getStereotype("Foreign Key")?.getProperty("Association")?.getSelected() as any as IAssociationApi;
        return fkAssociation != null && !fkAssociation.getOtherEnd().typeReference.getIsCollection() && !fkAssociation.getOtherEnd().typeReference.getIsNullable()
    }

    static getChildrenOfType(entity: MacroApi.Context.IElementApi, type: string): IAttributeWithMapPath[] {
        let attrDict: { [characterName: string]: IAttributeWithMapPath } = Object.create(null);
        let attributes = entity.getChildren(type)
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            typeReferenceModel: attr.typeReference.toModel(),
            mapPath: [attr.id],
            isNullable: attr.typeReference.isNullable,
            isCollection: attr.typeReference.isCollection
        });
        return Object.values(attrDict);
    }

    static getAttributesWithMapPath(entity: MacroApi.Context.IElementApi): IAttributeWithMapPath[] {
        let attrDict: { [characterName: string]: IAttributeWithMapPath } = Object.create(null);
        let attributes = entity
            .getChildren("Attribute")
            .filter(x => !x.hasStereotype("Primary Key") &&
                !DomainHelper.legacyPartitionKey(x) &&
                (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || x.getMetadata("set-by-infrastructure")?.toLocaleLowerCase() != "true")));
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            typeReferenceModel: attr.typeReference.toModel(),
            mapPath: [attr.id],
            isNullable: false,
            isCollection: false
        });

        traverseInheritanceHierarchyForAttributes(attrDict, entity, []);

        return Object.values(attrDict);

        function traverseInheritanceHierarchyForAttributes(
            attrDict: { [characterName: string]: IAttributeWithMapPath },
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
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !DomainHelper.legacyPartitionKey(x));
            baseKeys.forEach(attr => {
                attrDict[attr.id] = {
                    id: attr.id,
                    name: attr.getName(),
                    typeId: attr.typeReference.typeId,
                    typeReferenceModel: attr.typeReference.toModel(),
                    mapPath: generalizationStack.concat([attr.id]),
                    isNullable: attr.typeReference.isNullable,
                    isCollection: attr.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
        }
    }

    static getMandatoryAssociationsWithMapPath(entity: MacroApi.Context.IElementApi): IAttributeWithMapPath[] {
        return traverseInheritanceHierarchy(entity, [], []);

        function traverseInheritanceHierarchy(
            entity: MacroApi.Context.IElementApi,
            results: IAttributeWithMapPath[],
            generalizationStack: string[]
        ): IAttributeWithMapPath[] {
            entity
                .getAssociations("Association")
                .filter(x => !x.typeReference.isCollection && !x.typeReference.isNullable && x.typeReference.isNavigable &&
                    !x.getOtherEnd().typeReference.isCollection && !x.getOtherEnd().typeReference.isNullable)
                .forEach(association => {
                    return results.push({
                        id: association.id,
                        name: association.getName(),
                        typeId: null,
                        typeReferenceModel: null,
                        mapPath: generalizationStack.concat([association.id]),
                        isNullable: false,
                        isCollection: false
                    });
                });


            let generalizations = entity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return results;
            }

            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);

            return traverseInheritanceHierarchy(generalization.typeReference.getType(), results, generalizationStack);
        }
    }

    static getAttributeNameFormat(str: string): string {
        let convention = DomainHelper.getDomainAttributeNamingConvention();
        switch (convention) {
            case "pascal-case":
                return toPascalCase(str);
            case "camel-case":
                return toCamelCase(str);
        }
        return str;
    }

    static getDomainAttributeNamingConvention() {
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return application.getSettings(domainSettingsId)
            ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
    }

    static getSurrogateKeyType(): string {
        return getSurrogateKeyType();
    }

    // Just in case someone still uses this convention. Used to filter out those attributes when mapping
    // to domain entities that are within a Cosmos DB paradigm.
    static legacyPartitionKey(attribute: MacroApi.Context.IElementApi) {
        return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
    }

    static requiresForeignKey(associationEnd: MacroApi.Context.IAssociationApi): boolean {
        return DomainHelper.isManyToVariantsOfOne(associationEnd) || DomainHelper.isSelfReferencingZeroToOne(associationEnd);
    }

    static isManyToVariantsOfOne(associationEnd: MacroApi.Context.IAssociationApi): boolean {
        return !associationEnd.typeReference.isCollection && associationEnd.getOtherEnd().typeReference.isCollection;
    }

    static isSelfReferencingZeroToOne(associationEnd: MacroApi.Context.IAssociationApi): boolean {
        return !associationEnd.typeReference.isCollection && associationEnd.typeReference.isNullable &&
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
    }

    static getOwningAggregateRecursive(entity: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi | null {
        let owners = DomainHelper.getOwnersRecursive(entity);

        if (owners.length == 0)
            return null;

        const uniqueIds = new Set(owners.map(item => item.id));
        if (uniqueIds.size !== 1) {
            throw new Error(`Entity : '${entity.getName()}' has more than 1 owner.`);
        }
        return owners[0];
    }

    static getOwnersRecursive(entity: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi[] {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        
        let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));        

        let result: MacroApi.Context.IElementApi[] = [];

        for (let i = 0; i < results.length; i++) {
            let owner = results[i].typeReference.getType()
            if (DomainHelper.isAggregateRoot(owner)){
                result.push(owner);
            } else {
                result.push(...DomainHelper.getOwnersRecursive(owner));
            }               
        }        
        return result;
    }

    static isOwnedByAssociation(association: MacroApi.Context.IAssociationReadOnlyApi) {
        return association.isSourceEnd() &&
            !association.typeReference.isNullable &&
            !association.typeReference.isCollection;
    }

    static getOwningAggregateKeyChain(entity: MacroApi.Context.IElementApi): IKeyChain[] {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        
        let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));        

        let result: IKeyChain[] = [];

        if (results.length == 0)
            return result;

        let owner = results[0].typeReference.getType()

        let pks = DomainHelper.getPrimaryKeys(owner);

        pks.forEach(pk =>{
            let attribute = lookup(pk.id);

            //expectedName would typically be CountryId if you have a Agg: Country with a Pk: Id
            let expectedName = attribute.getParent().getName();
            if (!attribute.getName().startsWith(expectedName)) {
                expectedName += attribute.getName();
            }
            else {
                expectedName = attribute.getName();
            }

            result.push({ attribute: attribute, expectedName: expectedName });            
        });

        if (!DomainHelper.isAggregateRoot(owner)){
            result.unshift(...DomainHelper.getOwningAggregateKeyChain(owner));
        }               
        return result;
    }
}