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
            isMapped: child.isMapped(),
            mappedToAttributeId: child.isMapped() ? child.getMapping().getElement().id : undefined
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
            typeDisplayText: child.typeReference?.display || ""
        };
        attributes.push(attribute);
    }
    
    return attributes;
}

function extractFieldMappings(dtoElement: MacroApi.Context.IElementApi): IFieldMapping[] {
    const mappings: IFieldMapping[] = [];
    const dtoFields = getDtoFields(dtoElement);
    
    for (const field of dtoFields) {
        if (field.isMapped && field.mappedToAttributeId) {
            mappings.push({
                sourcePath: [field.id],
                targetPath: [field.mappedToAttributeId],
                sourceFieldId: field.id,
                targetAttributeId: field.mappedToAttributeId
            });
        }
    }
    
    return mappings;
}

