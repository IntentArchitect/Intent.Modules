/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

// Constants
const VALID_SPECIALIZATIONS = ["DTO", "Command", "Query", "Operation"] as const;
const DTO_LIKE_SPECIALIZATIONS = ["DTO", "Command", "Query"] as const;
const ENTITY_ACTION_TYPES = [
    "Create Entity Action", 
    "Update Entity Action", 
    "Delete Entity Action", 
    "Query Entity Action"
] as const;
const MAX_HIERARCHY_DEPTH = 10;
const ELEMENT_TYPE_NAMES = {
    DTO_FIELD: "DTO-Field",
    ATTRIBUTE: "Attribute",
    PARAMETER: "Parameter"
} as const;
const METADATA_KEYS = {
    IS_MANAGED_KEY: "is-managed-key"
} as const;

/**
 * Normalize a name for comparison purposes by converting to lowercase.
 * This allows comparing camelCase, PascalCase, and other conventions uniformly.
 * For example: "id", "Id", "ID" all normalize to "id"
 * 
 * @param name The name to normalize
 * @returns The normalized name (lowercase)
 */
function normalizeNameForComparison(name: string): string {
    return name.toLowerCase();
}

/**
 * Check if two names are semantically equivalent (ignoring naming conventions).
 * Uses normalized comparison to handle camelCase vs PascalCase differences.
 * 
 * @param name1 First name to compare
 * @param name2 Second name to compare
 * @returns true if names are equivalent ignoring case/convention
 */
function namesAreEquivalent(name1: string, name2: string): boolean {
    return normalizeNameForComparison(name1) === normalizeNameForComparison(name2);
}

function isValidSyncElement(element: MacroApi.Context.IElementApi): boolean {
    return (VALID_SPECIALIZATIONS as readonly string[]).includes(element.specialization);
}

function extractDtoFromElement(element: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi | null {
    if ((DTO_LIKE_SPECIALIZATIONS as readonly string[]).includes(element.specialization)) {
        return element;
    }
    
    if (element.specialization === "Operation") {
        const parameters = element.getChildren(ELEMENT_TYPE_NAMES.PARAMETER);
        
        for (const param of parameters) {
            const typeRef = param.typeReference;
            if (typeRef && typeRef.isTypeFound()) {
                const type = typeRef.getType() as MacroApi.Context.IElementApi;
                
                if ((DTO_LIKE_SPECIALIZATIONS as readonly string[]).includes(type.specialization)) {
                    return type;
                }
            }
        }
    }
    
    return null;
}

function findAssociationsPointingToElement(searchElement: MacroApi.Context.IElementApi, dtoElement: MacroApi.Context.IElementApi): MacroApi.Context.IAssociationApi[] {
    const allAssociations: MacroApi.Context.IAssociationApi[] = [];
    
    // First try to get associations from searchElement
    for (const actionName of ENTITY_ACTION_TYPES) {
        try {
            // SDK limitation: getAssociations() return type needs casting to IAssociationApi[]
            const results = searchElement.getAssociations(actionName);
            
            if (results && results.length > 0) {
                // Operations have associations directly (typeReference points to entity, not DTO)
                // Commands/Queries also have associations directly when searchElement IS the DTO
                // In both cases, accept all associations without filtering
                if (searchElement.specialization === "Operation" || searchElement.id === dtoElement.id) {
                    allAssociations.push(...(results as any as MacroApi.Context.IAssociationApi[]));
                } else {
                    // When searching from a parent of the DTO, filter to only associations targeting our DTO
                    const filtered = (results as any as MacroApi.Context.IAssociationApi[]).filter(assoc => {
                        try {
                            const typeRef = assoc.typeReference;
                            if (typeRef && typeRef.getType()) {
                                const type = typeRef.getType() as MacroApi.Context.IElementApi;
                                return type.id === dtoElement.id;
                            }
                        } catch (e) {
                            // SDK may throw if association is incomplete
                        }
                        return false;
                    });
                    if (filtered.length > 0) {
                        allAssociations.push(...filtered);
                    }
                }
            }
        } catch (e) {
            // SDK method may fail on certain association types
        }
    }
    
    // If no associations found and searchElement is a DTO/Command/Query, walk up the hierarchy
    if (allAssociations.length === 0 && (DTO_LIKE_SPECIALIZATIONS as readonly string[]).includes(searchElement.specialization)) {
        let current: MacroApi.Context.IElementApi | null = searchElement;
        let depth = 0;
        while (current && allAssociations.length === 0 && depth < MAX_HIERARCHY_DEPTH) {
            // Try all action types on current element
            for (const actionName of ENTITY_ACTION_TYPES) {
                try {
                    // SDK limitation: getAssociations() return type needs casting to IAssociationApi[]
                    const results = current.getAssociations(actionName);
                    if (results && results.length > 0) {
                        // Filter to only associations that reference our DTO element
                        const filtered = (results as any as MacroApi.Context.IAssociationApi[]).filter(assoc => {
                            try {
                                const typeRef = assoc.typeReference;
                                if (typeRef && typeRef.getType()) {
                                    const type = typeRef.getType() as MacroApi.Context.IElementApi;
                                    return type.id === dtoElement.id;
                                }
                            } catch (e) {
                                // Skip associations that error out
                            }
                            return false;
                        });
                        if (filtered.length > 0) {
                            allAssociations.push(...filtered);
                        }
                    }
                } catch (e) {
                    // Continue to next parent if this fails
                }
            }
            
            if (allAssociations.length > 0) break;
            current = current.getParent() as MacroApi.Context.IElementApi || null;
            depth++;
        }
    }
    
    return allAssociations;
}

function getEntityFromAssociations(associations: MacroApi.Context.IAssociationApi[]): MacroApi.Context.IElementApi | null {
    if (associations.length === 0) return null;
    
    // Association typeReference points to the entity being acted upon
    const association = associations[0];
    if (association.typeReference && association.typeReference.isTypeFound()) {
        const entity = association.typeReference.getType() as MacroApi.Context.IElementApi;
        return entity;
    }
    
    return null;
}

function getEntityAttributes(entity: MacroApi.Context.IElementApi): IEntityAttribute[] {
    const attributes: IEntityAttribute[] = [];
    
    const children = entity.getChildren("Attribute");
    
    for (const child of children) {
        // Determine mappability per Intent Architect's mapping requirements
        
        // Check for auto-generated primary key (PK with non-user-supplied data source)
        let isAutoGeneratedPk = false;
        if (child.hasStereotype && child.hasStereotype("Primary Key")) {
            const pkStereotype = child.getStereotype("Primary Key");
            if (pkStereotype && pkStereotype.getProperty) {
                const dataSourceProp = pkStereotype.getProperty("Data source");
                isAutoGeneratedPk = dataSourceProp && dataSourceProp.value !== "User supplied";
            }
        }
        
        // Check for foreign key
        let isForeignKey = false;
        let fkIsRequiredParent = false;
        if (child.hasStereotype && child.hasStereotype("Foreign Key")) {
            isForeignKey = true;
            const fkStereotype = child.getStereotype("Foreign Key");
            if (fkStereotype && fkStereotype.getProperty) {
                const assocProp = fkStereotype.getProperty("Association");
                if (assocProp && assocProp.getSelected) {
                    const fkAssociation = assocProp.getSelected() as any as MacroApi.Context.IAssociationApi;
                    if (fkAssociation) {
                        const otherEnd = fkAssociation.getOtherEnd?.();
                        if (otherEnd && otherEnd.typeReference) {
                            // FK is required parent if NOT a collection and NOT nullable
                            fkIsRequiredParent = !otherEnd.typeReference.isCollection && !otherEnd.typeReference.isNullable;
                        }
                    }
                }
            }
        }
        
        // Check for infrastructure-managed field
        const isSetByInfrastructure = child.hasMetadata && child.hasMetadata("set-by-infrastructure") && 
                                      child.getMetadata("set-by-infrastructure") === "true";
        
        // Determine if mappable using Intent Architect's logic
        // An attribute is mappable if:
        // - It's NOT an auto-generated primary key
        // - If it's a foreign key, only if it's NOT a required parent (non-collection, non-nullable)
        // - It's NOT infrastructure-managed
        const isMappable = !isAutoGeneratedPk && 
                          (!isForeignKey || !fkIsRequiredParent) &&
                          !isSetByInfrastructure;
        
        const attribute: IEntityAttribute = {
            id: child.id,
            name: child.getName(),
            typeId: child.typeReference?.getTypeId(),
            typeDisplayText: child.typeReference?.display || "",
            icon: child.getIcon(),
            isManagedKey: child.hasMetadata(METADATA_KEYS.IS_MANAGED_KEY) && child.getMetadata(METADATA_KEYS.IS_MANAGED_KEY) === "true",
            hasPrimaryKeyStereotype: child.hasStereotype && child.hasStereotype("Primary Key"),
            isCollection: child.typeReference?.isCollection,
            isNullable: child.typeReference?.isNullable,
            isAutoGeneratedPk,
            isForeignKey,
            fkIsRequiredParent,
            isSetByInfrastructure,
            isMappable
        };
        attributes.push(attribute);
    }
    
    return attributes;
}

function extractFieldMappings(associations: MacroApi.Context.IAssociationApi[]): IFieldMapping[] {
    const mappings: IFieldMapping[] = [];
    
    for (const association of associations) {
        try {
            const advancedMappings = association.getAdvancedMappings();
            
            for (const advancedMapping of advancedMappings) {
                const mappedEnds = advancedMapping.getMappedEnds();
                
                for (const mappedEnd of mappedEnds) {
                    const sourcePath = mappedEnd.sourcePath;
                    const targetPath = mappedEnd.targetPath;
                    
                    if (sourcePath && sourcePath.length > 0 && targetPath && targetPath.length > 0) {
                        // IMPORTANT: Only extract mappings for direct fields (path length 1 for simple cases, or where source is a direct child)
                        // Skip nested mappings like [Command, AssociationField, NestedField] - these are handled separately
                        // We only want mappings like [Command, Field] or [Field] where the second element is a direct child
                        
                        // For now, include all mappings for compatibility, but log them for debugging
                        const sourceFieldId = sourcePath[sourcePath.length - 1].id;
                        const targetAttributeId = targetPath[targetPath.length - 1].id;
                        
                        mappings.push({
                            sourcePath: sourcePath.map(p => p.id),
                            targetPath: targetPath.map(p => p.id),
                            sourceFieldId: sourceFieldId,
                            targetAttributeId: targetAttributeId,
                            mappingType: mappedEnd.mappingType,
                            mappingTypeId: mappedEnd.mappingTypeId
                        });
                    }
                }
            }
        } catch (error) {
            // Association may not have advanced mappings configured
            continue;
        }
    }
    
    return mappings;
}