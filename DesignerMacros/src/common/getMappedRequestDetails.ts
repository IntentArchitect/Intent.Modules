/// <reference path="../common/domainHelper.ts" />
/// <reference path="../common/servicesHelper.ts" />
/// <reference path="../common/getParent.ts" />

interface IFieldDetails {
    /**
     * If a field already exists, this will be populated with its id.
     */
    existingId: string;
    mappingPath: string[];
    name: string;
    typeId: string;
    isCollection: boolean;
    isNullable: boolean;
}

/**
 * Select details of a mapped Command/Query.
 */
interface IMappedRequestDetails {
    mappingTargetType: "Class" | "Operation" | "Class Constructor";
    entity: IElementApi;
    owningEntity?: IElementApi;
    entityKeyFields: IFieldDetails[];
    ownerKeyFields: IFieldDetails[];
}

/**
 * Gets select details of a mapped Command/Query. Intended for centralized logic of working out
 * things like keys (including implicit ones) for both the entity and owning entity if applicable.
 * 
 * If the Command is for entity creation (either due to being mapped to a constructor or being
 * prefixed with "Create"), then primary keys for the entity are not populated.
 * @param request The Command or Query that has been mapped
 */
function getMappedRequestDetails(
    request: MacroApi.Context.IElementApi
): IMappedRequestDetails {
    const queryEntityMappingTypeId = "25f25af9-c38b-4053-9474-b0fabe9d7ea7";
    const createEntityMappingTypeId = "5f172141-fdba-426b-980e-163e782ff53e";

    // Basic mapping:
    let mappedElement = request.getMapping()?.getElement();

    // Advanced mapping:
    if (mappedElement == null) {
        const advancedMappings = request.getAssociations()
            .filter(x =>
                x.hasMappings(queryEntityMappingTypeId) ||
                x.hasMappings(createEntityMappingTypeId))
            .map(x =>
                x.getMapping(queryEntityMappingTypeId) ||
                x.getMapping(createEntityMappingTypeId));

        if (advancedMappings.length === 1) {
            mappedElement = advancedMappings[0].getTargetElement();
        }
    }

    if (mappedElement == null) {
        return null;
    }

    let entity = mappedElement;
    if (entity.specialization !== "Class") {
        entity = getParent(entity, "Class");
    }

    const result: IMappedRequestDetails = {
        entity: entity,
        mappingTargetType: mappedElement.specialization as any,
        entityKeyFields: [],
        ownerKeyFields: []
    };

    result.owningEntity = DomainHelper.getOwningAggregate(entity);

    // As long as it's not for creation, populate the PKs of the entity:
    if (result.mappingTargetType !== "Class Constructor" &&
        !request.getName().toLowerCase().startsWith("Create")
    ) {
        result.entityKeyFields = result.mappingTargetType === "Class"
            ? getKeysForClassMapping(request, entity)
            : getKeysForOperationMapping(request, entity);
    }

    // If the entity is owned, populate its fields:
    if (result.owningEntity != null) {
        result.ownerKeyFields = result.mappingTargetType === "Class"
            ? getKeysForClassMapping(request, entity, result.owningEntity)
            : getKeysForOperationMapping(request, entity, result.owningEntity);
    }

    return result;

    /**
     * Return field details for primary keys. As requests mapped to operations and constructors can
     * never possibly map the attributes, these fields can only ever be matched by name.
     * @param request The CQRS Command or Query entity
     * @param owningEntity The Owning Aggregate Class
     */
    function getKeysForOperationMapping(
        request: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        owningEntity?: MacroApi.Context.IElementApi
    ): IFieldDetails[] {
        const pks = DomainHelper.getPrimaryKeys(owningEntity ?? entity);

        return pks.map(pk => {
            let fieldName = toPascalCase(pk.name);

            if (owningEntity != null) {
                fieldName = removePrefix(fieldName, toPascalCase(owningEntity.getName()));
                fieldName = `${owningEntity.getName()}${toCamelCase(fieldName)}`;
            }

            fieldName = ServicesHelper.getFieldFormat(fieldName);

            const existingField = request.getChildren("DTO-Field").find(field => field.getName().toLowerCase() == fieldName.toLowerCase());

            return {
                existingId: existingField?.id,
                mappingPath: [],
                name: fieldName,
                typeId: pk.typeId,
                isCollection: pk.isCollection,
                isNullable: pk.isNullable
            };
        });
    }

    function getKeysForClassMapping(
        request: IElementApi,
        entity: IElementApi,
        owningEntity?: MacroApi.Context.IElementApi
    ): IFieldDetails[] {
        const keys = owningEntity != null
            ? DomainHelper.getForeignKeys(entity, owningEntity)
            : DomainHelper.getPrimaryKeys(entity);

        return keys.map(pk => {
            const existingField = request.getChildren("DTO-Field").find(field => {
                if (field.getMapping() != null) {
                    return field.getMapping().getPath().some(x => x.id == pk.id);
                }

                return (pk.name.toLowerCase() === field.getName().toLowerCase());
            });

            return {
                existingId: existingField?.id,
                mappingPath: pk.mapPath,
                name: pk.name,
                typeId: pk.typeId,
                isCollection: pk.isCollection,
                isNullable: pk.isNullable
            };
        });
    }
}
