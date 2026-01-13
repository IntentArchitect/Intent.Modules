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

function normalizeNameForComparison(name: string | null | undefined): string {
    return (name || "").toLowerCase();
}

function namesAreEquivalent(name1: string | null | undefined, name2: string | null | undefined): boolean {
    return normalizeNameForComparison(name1) === normalizeNameForComparison(name2);
}

function isValidSyncElement(element: MacroApi.Context.IElementApi): boolean {
    return (VALID_SPECIALIZATIONS as readonly string[]).includes(element.specialization);
}

/**
 * Safely get associations for an element and action type.
 * Wraps getAssociations() with try/catch and returns empty array on failure.
 * 
 * @param element The element to get associations from
 * @param actionName The action type (e.g., "Create Entity Action")
 * @returns Array of associations, or empty array if call fails
 */
function tryGetAssociations(element: MacroApi.Context.IElementApi, actionName: string): MacroApi.Context.IAssociationApi[] {
    try {
        // SDK limitation: getAssociations() return type needs casting to IAssociationApi[]
        const results = element.getAssociations(actionName);
        return castAssociations(results);
    } catch (e) {
        console.log(`[ASSOC] SDK call failed for ${actionName} on element ${element.id} (${element.getName()}): ${e}`);
        return [];
    }

    function castAssociations(results: unknown): MacroApi.Context.IAssociationApi[] {
        if (!results || !Array.isArray(results)) {
            return [];
        }
        return results as MacroApi.Context.IAssociationApi[];
    }
}

/**
 * Filter associations to only those targeting a specific DTO element.
 * Safely handles SDK errors when accessing typeReference.
 * 
 * @param assocs Array of associations to filter
 * @param dtoElementId The ID of the DTO element to target
 * @returns Filtered array of associations targeting the DTO
 */
function filterAssociationsTargetingDto(assocs: MacroApi.Context.IAssociationApi[], dtoElementId: string): MacroApi.Context.IAssociationApi[] {
    return assocs.filter(assoc => {
        try {
            const typeRef = assoc.typeReference;
            if (typeRef && typeRef.getType()) {
                const type = typeRef.getType() as MacroApi.Context.IElementApi;
                return type.id === dtoElementId;
            }
        } catch (e) {
            // SDK may throw if association is incomplete
            console.log(`[ASSOC] Error accessing typeReference for association: ${e}`);
        }
        return false;
    });
}

/**
 * Find all associations pointing to a specific DTO element.
 * 
 * Searches for entity associations on the given search element. Special handling:
 * - If searchElement is an Operation or is the DTO itself, accepts all associations without filtering
 * - Otherwise, filters associations to only those targeting the specific DTO
 * - If no associations found and searchElement is DTO-like, walks up the hierarchy up to MAX_HIERARCHY_DEPTH
 * 
 * @param searchElement The element to search for associations (DTO, Command, Query, or Operation)
 * @param dtoElement The DTO element that associations should target
 * @returns Array of associations pointing to the DTO element
 */
function findAssociationsPointingToElement(searchElement: MacroApi.Context.IElementApi, dtoElement: MacroApi.Context.IElementApi): MacroApi.Context.IAssociationApi[] {
    const allAssociations: MacroApi.Context.IAssociationApi[] = [];
    
    // First try to get associations from searchElement
    for (const actionName of ENTITY_ACTION_TYPES) {
        const results = tryGetAssociations(searchElement, actionName);
        
        if (results.length > 0) {
            // Operations have associations directly (typeReference points to entity, not DTO)
            // Commands/Queries also have associations directly when searchElement IS the DTO
            // In both cases, accept all associations without filtering
            if (searchElement.specialization === "Operation" || searchElement.id === dtoElement.id) {
                console.log(`[ASSOC] Found ${results.length} associations for ${actionName} on ${searchElement.getName()} (accepting all)`);
                allAssociations.push(...results);
            } else {
                // When searching from a parent of the DTO, filter to only associations targeting our DTO
                const filtered = filterAssociationsTargetingDto(results, dtoElement.id);
                console.log(`[ASSOC] Found ${results.length} associations for ${actionName} on ${searchElement.getName()}, ${filtered.length} target DTO ${dtoElement.getName()}`);
                if (filtered.length > 0) {
                    allAssociations.push(...filtered);
                }
            }
        }
    }
    
    // If no associations found and searchElement is a DTO/Command/Query, walk up the hierarchy
    if (allAssociations.length === 0 && (DTO_LIKE_SPECIALIZATIONS as readonly string[]).includes(searchElement.specialization)) {
        let current: MacroApi.Context.IElementApi | null = searchElement;
        let depth = 0;
        while (current && allAssociations.length === 0 && depth < MAX_HIERARCHY_DEPTH) {
            console.log(`[ASSOC] Walking up hierarchy: checking parent ${current.getName()} at depth ${depth}`);
            
            // Try all action types on current element
            for (const actionName of ENTITY_ACTION_TYPES) {
                const results = tryGetAssociations(current, actionName);
                if (results.length > 0) {
                    // Filter to only associations that reference our DTO element
                    const filtered = filterAssociationsTargetingDto(results, dtoElement.id);
                    console.log(`[ASSOC] Found ${results.length} associations for ${actionName} on parent ${current.getName()}, ${filtered.length} target DTO ${dtoElement.getName()}`);
                    if (filtered.length > 0) {
                        allAssociations.push(...filtered);
                    }
                }
            }
            
            if (allAssociations.length > 0) break;
            current = current.getParent() as MacroApi.Context.IElementApi || null;
            depth++;
        }
    }
    
    return allAssociations;
}

/**
 * Extract entity element from associations.
 *
 * Gets the target entity from the first association's typeReference.
 * Assumes all associations in the array target the same entity.
 *
 * @param associations Array of associations (should not be empty)
 * @returns The entity element that the associations target, or null if associations is empty or typeReference is invalid
 */
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

/**
 * Check if an attribute is an auto-generated primary key.
 * An auto-generated PK has the Primary Key stereotype with a non-user-supplied data source.
 * 
 * @param attributeElement The attribute element to check
 * @returns true if this is an auto-generated primary key
 */
function isAutoGeneratedPrimaryKey(attributeElement: MacroApi.Context.IElementApi): boolean {
    if (attributeElement.hasStereotype && attributeElement.hasStereotype("Primary Key")) {
        const pkStereotype = attributeElement.getStereotype("Primary Key");
        if (pkStereotype && pkStereotype.getProperty) {
            const dataSourceProp = pkStereotype.getProperty("Data source");
            return dataSourceProp && dataSourceProp.value !== "User supplied";
        }
    }
    return false;
}

/**
 * Get foreign key information for an attribute.
 * 
 * @param attributeElement The attribute element to check
 * @returns Object with isForeignKey flag and whether it's a required parent relationship
 */
function getForeignKeyInfo(attributeElement: MacroApi.Context.IElementApi): { isForeignKey: boolean; fkIsRequiredParent: boolean } {
    let isForeignKey = false;
    let fkIsRequiredParent = false;
    
    if (attributeElement.hasStereotype && attributeElement.hasStereotype("Foreign Key")) {
        isForeignKey = true;
        const fkStereotype = attributeElement.getStereotype("Foreign Key");
        if (fkStereotype && fkStereotype.getProperty) {
            const assocProp = fkStereotype.getProperty("Association");
            if (assocProp && assocProp.getSelected) {
                // SDK limitation: stereotype property getters return unknown types that need casting
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
    
    return { isForeignKey, fkIsRequiredParent };
}

/**
 * Check if an attribute is set by infrastructure.
 * Infrastructure-managed fields have the "set-by-infrastructure" metadata set to "true".
 * 
 * @param attributeElement The attribute element to check
 * @returns true if this attribute is managed by infrastructure
 */
function isSetByInfrastructure(attributeElement: MacroApi.Context.IElementApi): boolean {
    return attributeElement.hasMetadata && 
           attributeElement.hasMetadata("set-by-infrastructure") && 
           attributeElement.getMetadata("set-by-infrastructure") === "true";
}

/**
 * Determine if an attribute is mappable according to Intent Architect's mapping rules.
 * 
 * Mapping rules:
 * - NOT an auto-generated primary key
 * - If it's a foreign key, only if it's NOT a required parent (non-collection AND non-nullable)
 * - NOT infrastructure-managed
 * 
 * @param isAutoGeneratedPk Whether the attribute is an auto-generated PK
 * @param isForeignKey Whether the attribute is a foreign key
 * @param fkIsRequiredParent Whether the FK is a required parent relationship
 * @param isSetByInfrastructure Whether the attribute is infrastructure-managed
 * @returns true if the attribute can be mapped
 */
function isMappableAttribute(isAutoGeneratedPk: boolean, isForeignKey: boolean, fkIsRequiredParent: boolean, isSetByInfrastructure: boolean): boolean {
    return !isAutoGeneratedPk && 
           (!isForeignKey || !fkIsRequiredParent) &&
           !isSetByInfrastructure;
}

/**
 * Get all mappable attributes from an entity.
 * 
 * Filters entity attributes according to Intent Architect's mapping requirements.
 * Only attributes that can be safely mapped to DTO fields are included.
 * 
 * @param entity The entity element to extract attributes from
 * @returns Array of entity attributes with mapping metadata
 */
function getEntityAttributes(entity: MacroApi.Context.IElementApi): IEntityAttribute[] {
    const attributes: IEntityAttribute[] = [];
    
    const children = entity.getChildren("Attribute");
    
    for (const child of children) {
        // Determine mappability per Intent Architect's mapping requirements
        const isAutoGeneratedPk = isAutoGeneratedPrimaryKey(child);
        const { isForeignKey, fkIsRequiredParent } = getForeignKeyInfo(child);
        const isSetByInfrastructureFlag = isSetByInfrastructure(child);
        const isMappable = isMappableAttribute(isAutoGeneratedPk, isForeignKey, fkIsRequiredParent, isSetByInfrastructureFlag);
        
        // Debug log the mapping decision
        console.log(`[ATTR] ${child.getName()}: autoGenPK=${isAutoGeneratedPk}, fk=${isForeignKey}, reqParent=${fkIsRequiredParent}, infra=${isSetByInfrastructureFlag}, mappable=${isMappable}`);
        
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
            isSetByInfrastructure: isSetByInfrastructureFlag,
            isMappable
        };
        attributes.push(attribute);
    }
    
    return attributes;
}

/**
 * Extract field mappings from associations.
 *
 * Processes advanced mappings from associations, extracting source and target path information.
 * Currently includes all mappings for compatibility, but flags nested mappings (path length > 2)
 * for potential future filtering.
 *
 * @param associations Array of associations to extract mappings from
 * @returns Array of field mappings with nested classification metadata
 */
function extractFieldMappings(associations: MacroApi.Context.IAssociationApi[]): IFieldMapping[] {
    const mappings: IFieldMapping[] = [];
    
    for (const association of associations) {
        // Get association name/id for logging
        const assocName = (association as any).getName ? (association as any).getName() : `id:${association.id}`;
        
        try {
            const advancedMappings = association.getAdvancedMappings();
            console.log(`[MAPPING] Processing association ${assocName}: ${advancedMappings.length} advanced mappings`);
            
            for (const advancedMapping of advancedMappings) {
                const mappedEnds = advancedMapping.getMappedEnds();
                
                for (const mappedEnd of mappedEnds) {
                    const sourcePath = mappedEnd.sourcePath;
                    const targetPath = mappedEnd.targetPath;
                    
                    if (sourcePath && sourcePath.length > 0 && targetPath && targetPath.length > 0) {
                        // IMPORTANT: Still includes nested mappings for compatibility, but now flags them
                        // Nested mappings have sourcePath.length > 2 or targetPath.length > 2
                        // This indicates complex mappings like [Command, AssociationField, NestedField]
                        // Simple mappings are like [Command, Field] or [Field] (length 1-2)
                        
                        const sourceFieldId = sourcePath[sourcePath.length - 1].id;
                        const targetAttributeId = targetPath[targetPath.length - 1].id;
                        
                        mappings.push({
                            sourcePath: sourcePath.map(p => p.id),
                            targetPath: targetPath.map(p => p.id),
                            sourceFieldId: sourceFieldId,
                            targetAttributeId: targetAttributeId,
                            mappingType: mappedEnd.mappingType,
                            mappingTypeId: mappedEnd.mappingTypeId,
                        });
                    }
                }
            }
        } catch (error) {
            // Association may not have advanced mappings configured
            console.log(`[MAPPING] Error getting advanced mappings for association ${assocName}: ${error}`);
            continue;
        }
    }
    
    console.log(`[MAPPING] Total mappings extracted: ${mappings.length}`);
    return mappings;
}