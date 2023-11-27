type IElementMappingApi = MacroApi.Context.IElementMappingApi;

class MappingHelper {
    /**
     * Ensures that a mapped to type exists, is mapped to the {@link EnsureMappedToTypeOptions.property}'s mapped element and updates
     * the {@link options.property}'s type reference to point to the type.
     * 
     * @returns The existing or created type
     */
    static ensureMappedToType(options: EnsureMappedToTypeOptions): IElementApi {
        let {
            property,
            name,
            mappingSettingsId,
            includeKeys,
            sourcePropertySpecialization,
            typePropertySpecialization,
            typeSpecialization
        } = options;
        const domainElement = property.getMapping().getElement().typeReference.getType();

        name ??= domainElement.getName();
        includeKeys ??= true;
        sourcePropertySpecialization ??= "Attribute";

        const type = MappingHelper.getOrCreateType(name, property.getParent().getParent(), typeSpecialization);
        type.setMapping(domainElement.id, mappingSettingsId);
        property.typeReference.setType(type.id);

        let typeUpdated = false;
        let primaryKeyFound = false;
        for (const attribute of MappingHelper.getAttributesToMap(domainElement, sourcePropertySpecialization, [])) {
            const { element, path } = attribute;

            if (!includeKeys && (
                element.getName() == "id" ||
                element.hasStereotype("Foreign Key") ||
                element.hasStereotype("Primary Key") ||
                (element.hasStereotype("Partition Key") && element.getName() === "PartitionKey") || // Legacy Cosmos DB designer option
                MappingHelper.isOwnerForeignKey(element.getName(), domainElement))
            ) {
                continue;
            }

            if (type.getChildren(typePropertySpecialization).some(x => x.getName().toLowerCase() === element.getName().toLowerCase())) {
                continue;
            }

            const typeProperty = createElement(typePropertySpecialization, element.getName(), type.id);
            typeProperty.typeReference.setType(element.typeReference.getTypeId());
            typeProperty.typeReference.setIsNullable(element.typeReference.isNullable);
            typeProperty.typeReference.setIsCollection(element.typeReference.isCollection);
            typeProperty.setMapping(path);

            typeUpdated = true;
        }

        const implicitPkName = MappingHelper.applyNamingConvention("id");
        if (includeKeys &&
            !primaryKeyFound &&
            !type.getChildren(typePropertySpecialization).some(x => x.getName().toLowerCase() === implicitPkName.toLowerCase())
        ) {
            const typeProperty = createElement(typePropertySpecialization, implicitPkName, MappingHelper.getSurrogateKeyType());
            typeProperty.setOrder(0);

            typeUpdated = true;
        }

        if (typeUpdated) {
            type.collapse();
        }

        return type;
    }

    private static isOwnerForeignKey(attributeName: string, domainElement: IElementApi): boolean {
        for (let association of domainElement.getAssociations().filter(x => !x.typeReference.isCollection && !x.typeReference.isNullable)) {
            if (attributeName.toLowerCase().indexOf(association.getName().toLowerCase()) >= 0) {
                return true;
            }
        }

        return false;
    }

    private static getOrCreateType(
        elementName: string,
        parentElement: MacroApi.Context.IElementApi,
        typeSpecialization: string
    ): MacroApi.Context.IElementApi {
        let type = parentElement.getChildren(typeSpecialization).filter(x => x.getName().toLowerCase() === elementName.toLowerCase())[0];
        type ??= createElement(typeSpecialization, elementName, parentElement.id);

        return type;
    }

    private static *getAttributesToMap(
        classElement: IElementApi,
        propertySourceType: string,
        currentPath: string[]
    ): IterableIterator<AttributeToMap> {
        for (const attribute of classElement.getChildren(propertySourceType)) {
            yield {
                element: attribute,
                path: currentPath.concat([attribute.id])
            };
        }

        let generalizations = classElement.getAssociations("Generalization").filter(x => x.isTargetEnd());
        if (generalizations.length != 1) {
            return;
        }

        for (const generalization of generalizations) {
            const baseType = generalization.typeReference.getType();

            for (const baseTypeAttribute of MappingHelper.getAttributesToMap(baseType, propertySourceType, [baseType.id])) {
                yield baseTypeAttribute;
            }
        }
    }

    private static getSurrogateKeyType(): string {
        const commonTypes = {
            guid: "6b649125-18ea-48fd-a6ba-0bfff0d8f488",
            long: "33013006-E404-48C2-AC46-24EF5A5774FD",
            int: "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74"
        };

        const javaTypes = {
            long: "e9e575eb-f8de-4ce4-9838-2d09665a752d",
            int: "b3e5cb3b-8a26-4346-810b-9789afa25a82"
        };

        const typeNameToIdMap = new Map();
        typeNameToIdMap.set("guid", commonTypes.guid);
        typeNameToIdMap.set("int", lookup(javaTypes.int) != null ? javaTypes.int : commonTypes.int);
        typeNameToIdMap.set("long", lookup(javaTypes.long) != null ? javaTypes.long : commonTypes.long);

        const typeName = application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Type")?.value ?? "guid";
        return typeNameToIdMap.has(typeName)
            ? typeNameToIdMap.get(typeName)
            : typeNameToIdMap.get("guid");
    }

    private static applyNamingConvention(str: string): string {
        const convention = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")?.getField("Attribute Naming Convention")?.value ?? "pascal-case";

        switch (convention) {
            case "pascal-case":
                return toPascalCase(str);
            case "camel-case":
                return toCamelCase(str);
        }

        return str;
    }
}

interface AttributeToMap {
    path: string[];
    element: IElementApi;
}

interface EnsureMappedToTypeOptions {
    /**
     * The mapped element of this {@link IElementApi} for which to ensure is mapped and this is set to it.
     */
    property: IElementApi;

    /**
     * The name for the DTO, if unset default to {@link IElementApi.getMapping()}'s {@link IElementMappingApi.getElement()}'s "<{@link IElementApi.getName()}>Dto".
     */
    name?: string;

    /**
     * The value to use for {@link IElementApi.setMapping()}'s mappingSettingsId parameter.
     */
    mappingSettingsId: string;

    /**
     * The value to pass to {@link createElement}'s when creating the type that will be mapped to.
     */
    typeSpecialization: string;

    /**
     * The value to pass to {@link createElement}'s when creating the sub-element type that will be mapped to from attributes.
     */
    typePropertySpecialization: string;

    /**
     * The type of source properties on the source type to also map. Defaults to "Attribute".
     */
    sourcePropertySpecialization?: string;

    /**
     * Whether or not to map attributes which are primary or foreign keys, defaults to true.
     */
    includeKeys?: boolean;
}
