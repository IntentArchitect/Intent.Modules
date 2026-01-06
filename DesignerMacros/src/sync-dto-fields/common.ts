/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

function isValidSyncElement(element: MacroApi.Context.IElementApi): boolean {
    const validSpecializations = ["DTO", "Command", "Query"];
    return validSpecializations.includes(element.specialization);
}

function getValidSpecializations(): string[] {
    return ["DTO", "Command", "Query"];
}

function findAssociationsPointingToElement(dtoElement: MacroApi.Context.IElementApi): MacroApi.Context.IAssociationApi[] {
    const package_ = dtoElement.getPackage();
    if (!package_) return [];
    
    const allAssociations: MacroApi.Context.IAssociationApi[] = [];
    const actionNames = ["Create Entity Action", "Update Entity Action", "Delete Entity Action", "Query Entity Action"];
    
    // Get all associations from the element  
    for (const actionName of actionNames) {
        const results = dtoElement.getAssociations(actionName);
        if (results && results.length > 0) {
            allAssociations.push(...(results as any as MacroApi.Context.IAssociationApi[]));
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
            mappedToAttributeId: undefined
        };
        fields.push(field);
    }
    
    return fields;
}

function getEntityAttributes(entity: MacroApi.Context.IElementApi): IEntityAttribute[] {
    const attributes: IEntityAttribute[] = [];
    const children = entity.getChildren("Attribute");
    
    for (const child of children) {
        // Skip managed keys (auto-generated primary keys)
        const isManagedKey = child.hasMetadata("is-managed-key") && child.getMetadata("is-managed-key") === "true";
        if (isManagedKey) {
            continue;
        }
        
        const attribute: IEntityAttribute = {
            id: child.id,
            name: child.getName(),
            typeId: child.typeReference?.getTypeId(),
            typeDisplayText: child.typeReference?.display || ""
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

