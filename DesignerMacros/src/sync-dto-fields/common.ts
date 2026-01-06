/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

function isValidSyncElement(element: MacroApi.Context.IElementApi): boolean {
    const validSpecializations = ["DTO", "Command", "Query", "Operation"];
    return validSpecializations.includes(element.specialization);
}

function getValidSpecializations(): string[] {
    return ["DTO", "Command", "Query", "Operation"];
}

function extractDtoFromElement(element: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi | null {
    // If element is already a DTO, Command, or Query, return it
    if (["DTO", "Command", "Query"].includes(element.specialization)) {
        return element;
    }
    
    // If element is an Operation, find the DTO parameter
    if (element.specialization === "Operation") {
        const parameters = element.getChildren("Parameter");
        for (const param of parameters) {
            const typeRef = param.typeReference;
            if (typeRef && typeRef.isTypeFound()) {
                const type = typeRef.getType() as MacroApi.Context.IElementApi;
                if (["DTO", "Command", "Query"].includes(type.specialization)) {
                    return type;
                }
            }
        }
    }
    
    return null;
}

function findAssociationsPointingToElement(searchElement: MacroApi.Context.IElementApi, dtoElement: MacroApi.Context.IElementApi): MacroApi.Context.IAssociationApi[] {
    const allAssociations: MacroApi.Context.IAssociationApi[] = [];
    const actionNames = ["Create Entity Action", "Update Entity Action", "Delete Entity Action", "Query Entity Action"];
    
    // First try to get associations from searchElement
    for (const actionName of actionNames) {
        try {
            const results = searchElement.getAssociations(actionName);
            
            if (results && results.length > 0) {
                // For Operations, don't filter - all associations from the operation are valid
                // The DTO mapping is in the mapping source paths
                if (searchElement.specialization === "Operation") {
                    allAssociations.push(...(results as any as MacroApi.Context.IAssociationApi[]));
                } else {
                    // For DTOs/Commands/Queries, filter to associations that reference this element
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
            }
        } catch (e) {
            // Continue to next action type
        }
    }
    
    // If no associations found and searchElement is a DTO/Command/Query, walk up the hierarchy
    if (allAssociations.length === 0 && ["DTO", "Command", "Query"].includes(searchElement.specialization)) {
        let current: MacroApi.Context.IElementApi | null = searchElement;
        let depth = 0;
        while (current && allAssociations.length === 0 && depth < 10) {
            // Try all action types on current element
            for (const actionName of actionNames) {
                try {
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
    
    // Get the typeReference from the first association
    // The association's typeReference points to the entity
    const association = associations[0];
    if (association.typeReference && association.typeReference.isTypeFound()) {
        return association.typeReference.getType() as MacroApi.Context.IElementApi;
    }
    
    return null;
}

function getDtoFields(dtoElement: MacroApi.Context.IElementApi): IDtoField[] {
    const fields: IDtoField[] = [];
    const children = dtoElement.getChildren("DTO-Field");
    
    for (const child of children) {
        const field: IDtoField = {
            id: child.id,
            name: child.getName(),
            typeId: child.typeReference?.getTypeId(),
            typeDisplayText: child.typeReference?.display || "",
            isMapped: false, // Will be determined by extractFieldMappings
            mappedToAttributeId: undefined,
            icon: child.getIcon()
        };
        fields.push(field);
    }
    
    return fields;
}

function getEntityAttributes(entity: MacroApi.Context.IElementApi): IEntityAttribute[] {
    const attributes: IEntityAttribute[] = [];
    const children = entity.getChildren("Attribute");
    
    for (const child of children) {
        const attribute: IEntityAttribute = {
            id: child.id,
            name: child.getName(),
            typeId: child.typeReference?.getTypeId(),
            typeDisplayText: child.typeReference?.display || "",
            icon: child.getIcon(),
            isManagedKey: child.hasMetadata("is-managed-key") && child.getMetadata("is-managed-key") === "true"
        };
        attributes.push(attribute);
    }
    
    return attributes;
}

function extractFieldMappings(associations: MacroApi.Context.IAssociationApi[]): IFieldMapping[] {
    const mappings: IFieldMapping[] = [];
    
    // Get advanced mappings from each association
    for (const association of associations) {
        try {
            const advancedMappings = association.getAdvancedMappings();
            
            for (const advancedMapping of advancedMappings) {
                const mappedEnds = advancedMapping.getMappedEnds();
                
                for (const mappedEnd of mappedEnds) {
                    // Get source and target paths
                    const sourcePath = mappedEnd.sourcePath;
                    const targetPath = mappedEnd.targetPath;
                    
                    // Only process if we have valid paths with at least one element
                    if (sourcePath && sourcePath.length > 0 && targetPath && targetPath.length > 0) {
                        const sourceFieldId = sourcePath[sourcePath.length - 1].id;
                        const targetAttributeId = targetPath[targetPath.length - 1].id;
                        
                        mappings.push({
                            sourcePath: sourcePath.map(p => p.id),
                            targetPath: targetPath.map(p => p.id),
                            sourceFieldId: sourceFieldId,
                            targetAttributeId: targetAttributeId
                        });
                    }
                }
            }
        } catch (error) {
            // Skip associations without advanced mappings
            continue;
        }
    }
    
    return mappings;
}

