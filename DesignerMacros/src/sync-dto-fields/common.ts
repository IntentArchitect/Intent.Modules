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

// Build structured field type information from typeReference
function buildFieldType(typeReference: MacroApi.Context.ITypeReference | undefined, displayText?: string): IFieldType {
    if (!typeReference || !typeReference.isTypeFound?.()) {
        return {
            baseType: "Unknown",
            isCollection: false,
            isNullable: false,
            displayText: displayText || "Unknown"
        };
    }
    
    const isCollection = typeReference.isCollection || false;
    const isNullable = typeReference.isNullable || false;
    
    const baseType = typeReference.display?.replace(/[\[\]?]/g, "") || displayText?.replace(/[\[\]?]/g, "") || "Unknown";
    
    let displayTextFinal = baseType;
    if (isCollection) displayTextFinal += "[]";
    if (isNullable) displayTextFinal += "?";
    
    return {
        baseType: baseType,
        isCollection: isCollection,
        isNullable: isNullable,
        displayText: displayTextFinal
    };
}

function isValidSyncElement(element: MacroApi.Context.IElementApi): boolean {
    return (VALID_SPECIALIZATIONS as readonly string[]).includes(element.specialization);
}

function getValidSpecializations(): string[] {
    return [...VALID_SPECIALIZATIONS];
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

function getDtoFields(dtoElement: MacroApi.Context.IElementApi): IDtoField[] {
    const fields: IDtoField[] = [];
    
    // Dynamically determine child type instead of hard-coding "DTO-Field"
    const childType = inferSourceElementChildType(dtoElement);
    
    const children = dtoElement.getChildren(childType);
    
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
    
    // Dynamically determine child type instead of hard-coding "Attribute"
    const childType = inferTargetElementChildType(entity);
    
    const children = entity.getChildren(childType);
    
    for (const child of children) {
        const attribute: IEntityAttribute = {
            id: child.id,
            name: child.getName(),
            typeId: child.typeReference?.getTypeId(),
            typeDisplayText: child.typeReference?.display || "",
            icon: child.getIcon(),
            isManagedKey: child.hasMetadata(METADATA_KEYS.IS_MANAGED_KEY) && child.getMetadata(METADATA_KEYS.IS_MANAGED_KEY) === "true"
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

function extractParameterMappings(associations: MacroApi.Context.IAssociationApi[]): Map<string, string> {
    // Returns a map of Parameter ID -> Attribute ID for primitive parameter mappings
    const parameterMappings = new Map<string, string>();
    
    for (const association of associations) {
        try {
            const advancedMappings = association.getAdvancedMappings();
            
            for (const advancedMapping of advancedMappings) {
                const mappedEnds = advancedMapping.getMappedEnds();
                
                for (const mappedEnd of mappedEnds) {
                    const sourcePath = mappedEnd.sourcePath;
                    const targetPath = mappedEnd.targetPath;
                    
                    if (!sourcePath || sourcePath.length === 0 || !targetPath || targetPath.length === 0) {
                        continue;
                    }
                    
                    // Check if the LAST element in the source path is a Parameter (not a DTO-Field or other field)
                    const lastSourceElement = sourcePath[sourcePath.length - 1];
                    const sourceSpecialization = (lastSourceElement as any).specialization || "unknown";
                    
                    // Only process if last source element is a Parameter (primitive parameters have direct mappings)
                    if (sourceSpecialization === "Parameter") {
                        const paramId = lastSourceElement.id;
                        const targetAttributeId = targetPath[targetPath.length - 1].id;
                        
                        parameterMappings.set(paramId, targetAttributeId);
                    }
                }
            }
        } catch (error) {
            // Association may not have advanced mappings configured
            continue;
        }
    }
    
    return parameterMappings;
}

function inferSourceElementChildType(sourceRoot: MacroApi.Context.IElementApi): string {
    // Try common types in order of likelihood
    const candidates = ["DTO-Field", "Parameter", "Property", "Attribute"];
    
    for (const candidateType of candidates) {
        const children = sourceRoot.getChildren(candidateType);
        if (children && children.length > 0) {
            return candidateType;
        }
    }
    
    // Fallback: return the most common type
    return "DTO-Field";
}

function inferTargetElementChildType(targetRoot: MacroApi.Context.IElementApi): string {
    // Try common types in order of likelihood
    const candidates = ["Attribute", "Property", "Parameter"];
    
    for (const candidateType of candidates) {
        const children = targetRoot.getChildren(candidateType);
        if (children && children.length > 0) {
            return candidateType;
        }
    }
    
    // Fallback: return the most common type
    return "Attribute";
}

function analyzePathStructure(mappings: IFieldMapping[]): IPathTemplate | null {
    if (mappings.length === 0) return null;
    
    // Analyze non-invocation mappings to determine path structure
    const dataMappings = mappings.filter(m => 
        m.mappingType !== "Invocation Mapping" && 
        m.sourcePath.length > 0 && 
        m.targetPath.length > 0
    );
    
    if (dataMappings.length === 0) return null;
    
    // PRIORITY: Select the mapping with the LONGEST source path
    // If multiple have the same length, PREFER "Data Mapping" over others like "Filter Mapping"
    const sampleMapping = dataMappings.reduce((current, next) => {
        const currentLength = current.sourcePath.length;
        const nextLength = next.sourcePath.length;
        
        if (nextLength > currentLength) {
            // Longer path wins
            return next;
        } else if (nextLength === currentLength) {
            // Same length: prefer "Data Mapping" over other types
            if (next.mappingType === "Data Mapping" && current.mappingType !== "Data Mapping") {
                return next;
            }
            // Otherwise keep current
            return current;
        }
        // Keep current (longer)
        return current;
    });
    
    // Build element type arrays by looking up each element in the path
    const sourceElementTypes: string[] = [];
    const targetElementTypes: string[] = [];
    
    for (const elementId of sampleMapping.sourcePath) {
        try {
            const element = lookup(elementId);
            sourceElementTypes.push(element.specialization);
        } catch {
            sourceElementTypes.push("Unknown");
        }
    }
    
    for (const elementId of sampleMapping.targetPath) {
        try {
            const element = lookup(elementId);
            targetElementTypes.push(element.specialization);
        } catch {
            targetElementTypes.push("Unknown");
        }
    }
    
    // Create signature: "Operation>Parameter>DTO-Field -> Class>Attribute"
    const signature = `${sourceElementTypes.join(">")} -> ${targetElementTypes.join(">")}`;
    

    return {
        sourceDepth: sampleMapping.sourcePath.length,
        targetDepth: sampleMapping.targetPath.length,
        sourceElementTypes: sourceElementTypes,
        targetElementTypes: targetElementTypes,
        signature: signature,
        mappingTypeId: sampleMapping.mappingTypeId || "",
        mappingTypeName: sampleMapping.mappingType || "Data Mapping"
    };
}

function buildPathUsingTemplate(
    template: IPathTemplate,
    newFieldId: string,
    targetAttributeId: string,
    sourceRoot: MacroApi.Context.IElementApi,
    targetRoot: MacroApi.Context.IElementApi,
    dtoElement?: MacroApi.Context.IElementApi
): { sourcePath: string[], targetPath: string[] } {
    const sourcePath: string[] = [];
    const targetPath: string[] = [];
    
    // Build source path based on template depth and structure
    if (template.sourceDepth === 1) {
        // Simple: [fieldId]
        sourcePath.push(newFieldId);
    } else if (template.sourceDepth === 2) {
        // Two levels: [parent, fieldId]
        // For Operations: parent is Parameter, not the Operation itself
        // For DTOs: parent is the DTO itself
        if (sourceRoot.specialization === "Operation") {
            // For operations, we need to find the Parameter that references the DTO
            const parameter = findIntermediateElement(sourceRoot, "Parameter", dtoElement);
            if (parameter) {
                sourcePath.push(parameter.id, newFieldId);
            } else {
                // Fallback
                sourcePath.push(sourceRoot.id, newFieldId);
            }
        } else {
            // For DTOs or other elements, use sourceRoot
            sourcePath.push(sourceRoot.id, newFieldId);
        }
    } else if (template.sourceDepth === 3) {
        // Three levels: Need to find intermediate element
        // For Operations with DTO parameters: [operationId, parameterId, fieldId]
        const intermediateElement = findIntermediateElement(sourceRoot, template.sourceElementTypes[1], dtoElement);
        if (intermediateElement) {
            sourcePath.push(sourceRoot.id, intermediateElement.id, newFieldId);
        } else {
            // Fallback to simpler path
            sourcePath.push(sourceRoot.id, newFieldId);
        }
    } else {
        // For deeper paths, attempt to reconstruct
        sourcePath.push(sourceRoot.id);
        // Add intermediate elements if needed
        for (let i = 1; i < template.sourceDepth - 1; i++) {
            const intermediateType = template.sourceElementTypes[i];
            const parentElement = sourcePath.length === 1 ? sourceRoot : lookup(sourcePath[sourcePath.length - 1]);
            const intermediate = findIntermediateElement(parentElement, intermediateType, dtoElement);
            if (intermediate) {
                sourcePath.push(intermediate.id);
            }
        }
        sourcePath.push(newFieldId);
    }
    
    // Build target path based on template depth
    if (template.targetDepth === 1) {
        // Simple: [attributeId]
        targetPath.push(targetAttributeId);
    } else if (template.targetDepth === 2) {
        // Two levels: [entityId, attributeId]
        targetPath.push(targetRoot.id, targetAttributeId);
    } else {
        // For deeper paths
        targetPath.push(targetRoot.id);
        for (let i = 1; i < template.targetDepth - 1; i++) {
            // Add intermediate elements based on template
            const intermediateType = template.targetElementTypes[i];
            const parentElement = lookup(targetPath[targetPath.length - 1]);
            const intermediate = findIntermediateElement(parentElement, intermediateType);
            if (intermediate) {
                targetPath.push(intermediate.id);
            }
        }
        targetPath.push(targetAttributeId);
    }
    
    return { sourcePath, targetPath };
}

function findIntermediateElement(parent: MacroApi.Context.IElementApi, expectedType: string, dtoElement?: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi | null {
    // Special case: For Operations with Parameters, find the parameter that references the DTO
    if (expectedType === "Parameter" && parent.specialization === "Operation" && dtoElement) {
        const parameters = parent.getChildren("Parameter");
        
        for (const param of parameters) {
            if (param.typeReference && param.typeReference.isTypeFound()) {
                const paramType = param.typeReference.getType();
                const paramTypeId = paramType.id;
                const dtoElementId = dtoElement.id;
                
                if (paramTypeId === dtoElementId) {
                    return param;
                }
            }
        }
    }
    
    // Try to find a child element of the expected type
    const children = parent.getChildren(expectedType);
    if (children && children.length > 0) {
        return children[0];
    }
    
    // Try associations
    const associations = parent.getAssociations();
    for (const assoc of associations) {
        try {
            const target = assoc.typeReference?.getType();
            if (target && target.specialization === expectedType) {
                return target as MacroApi.Context.IElementApi;
            }
        } catch {
            continue;
        }
    }
    
    return null;
}

// Phase 2/3: Generic hierarchical parameter discovery
function discoverParameters(element: MacroApi.Context.IElementApi): IParameterNode[] {
    const parameters: IParameterNode[] = [];
    
    const parameterChildren = element.getChildren("Parameter");
    if (!parameterChildren) {
        return parameters;
    }
    
    for (const param of parameterChildren) {
        const paramNode: IParameterNode = {
            id: param.id,
            name: param.getName(),
            type: "Primitive",  // Default
            typeId: param.typeReference?.getTypeId(),
            typeDisplayText: param.typeReference?.display || "Unknown",
            icon: param.getIcon()
        };
        
        // Determine parameter type and discover nested fields
        if (param.typeReference && param.typeReference.isTypeFound()) {
            const paramType = param.typeReference.getType() as MacroApi.Context.IElementApi;
            const paramSpecialization = paramType.specialization;
            
            // Check if it's a DTO or similar complex type
            if (paramSpecialization === "DTO" || paramSpecialization === "Class") {
                paramNode.type = paramSpecialization === "DTO" ? "DTO" : "Complex";
                paramNode.typeId = paramType.id;
                
                // Recursively discover fields in the DTO/Complex type
                paramNode.children = discoverFields(paramType, 0);
            } else {
                // It's a primitive type
                paramNode.type = "Primitive";
            }
        }
        
        parameters.push(paramNode);
    }
    
    return parameters;
}

function discoverFields(element: MacroApi.Context.IElementApi, depth: number): IFieldNode[] {
    if (depth > MAX_HIERARCHY_DEPTH) {
        return [];
    }
    
    const fields: IFieldNode[] = [];
    
    // Determine what child type to look for based on element specialization
    let childType = "DTO-Field";
    if (element.specialization === "Class") {
        childType = "Attribute";
    }
    
    const fieldChildren = element.getChildren(childType);
    if (!fieldChildren) {
        return fields;
    }
    
    for (const field of fieldChildren) {
        const fieldNode: IFieldNode = {
            id: field.id,
            name: field.getName(),
            type: "Primitive",  // Default
            typeId: field.typeReference?.getTypeId(),
            typeDisplayText: field.typeReference?.display || "Unknown",
            icon: field.getIcon()
        };
        
        // Check if field type is complex
        if (field.typeReference && field.typeReference.isTypeFound()) {
            const fieldType = field.typeReference.getType() as MacroApi.Context.IElementApi;
            const fieldSpecialization = fieldType.specialization;
            
            if (fieldSpecialization === "DTO" || fieldSpecialization === "Class") {
                fieldNode.type = fieldSpecialization === "DTO" ? "DTO" : "Complex";
                fieldNode.typeId = fieldType.id;
                
                // Recursively discover nested fields
                fieldNode.children = discoverFields(fieldType, depth + 1);
            }
        }
        
        fields.push(fieldNode);
    }
    
    return fields;
}

// Find the mappable element (the one with advanced mappings) and target entity
function findMappableElement(element: MacroApi.Context.IElementApi): IMappableElement | null {
    const associations = findAssociationsPointingToElement(element, element);
    const targetEntity = getEntityFromAssociations(associations);
    
    if (!targetEntity) {
        return null;
    }
    
    const parameters = discoverParameters(element);
    
    return {
        element: element,
        parameters: parameters,
        targetEntity: targetEntity
    };
}
// Flatten hierarchical parameters to extract all mappable fields
function flattenParametersToFields(parameters: IParameterNode[]): IFieldNode[] {
    const fields: IFieldNode[] = [];
    
    for (const param of parameters) {
        if (param.type === "Primitive") {
            // Primitive parameters map directly
            fields.push({
                id: param.id,
                name: param.name,
                type: "Primitive",
                typeId: param.typeId,
                typeDisplayText: param.typeDisplayText,
                icon: param.icon,
                isMapped: param.isMapped,
                mappedToId: param.mappedToId
            });
        } else if (param.children) {
            // Complex parameters have fields that can be mapped
            fields.push(...flattenFieldNodes(param.children));
        }
    }
    
    return fields;
}

function flattenFieldNodes(nodes: IFieldNode[]): IFieldNode[] {
    const fields: IFieldNode[] = [];
    
    for (const node of nodes) {
        fields.push({
            id: node.id,
            name: node.name,
            type: node.type,
            typeId: node.typeId,
            typeDisplayText: node.typeDisplayText,
            icon: node.icon,
            isMapped: node.isMapped,
            mappedToId: node.mappedToId
        });
        
        if (node.children) {
            fields.push(...flattenFieldNodes(node.children));
        }
    }
    
    return fields;
}

// Helper to find a parameter node by ID in the hierarchy
function findParameterNodeById(parameters: IParameterNode[], id: string): IParameterNode | null {
    for (const param of parameters) {
        if (param.id === id) {
            return param;
        }
    }
    return null;
}

// Helper to find a field node by ID in the hierarchy
function findFieldNodeById(fields: IFieldNode[], id: string): IFieldNode | null {
    for (const field of fields) {
        if (field.id === id) {
            return field;
        }
        if (field.children) {
            const found = findFieldNodeById(field.children, id);
            if (found) return found;
        }
    }
    return null;
}

// Discover composite associations (entity-to-entity relationships where source owns target)
function discoverCompositeAssociations(entity: MacroApi.Context.IElementApi): IAssociationMetadata[] {
    const results: IAssociationMetadata[] = [];

    let assocEnds: MacroApi.Context.IAssociationApi[] = [];
    try {
        assocEnds = (entity.getAssociations("Association") as any) as MacroApi.Context.IAssociationApi[];
    } catch (e) {
        return results;
    }

    for (const end of assocEnds) {
        try {
            // The "navigation property" is typically the OTHER end
            const navEnd = end.isTargetEnd() ? end : end.getOtherEnd ? (end.getOtherEnd() as any as MacroApi.Context.IAssociationApi) : null;
            if (!navEnd || !navEnd.typeReference || !navEnd.typeReference.isTypeFound?.()) continue;

            const target = navEnd.typeReference.getType() as MacroApi.Context.IElementApi;
            if (!target) continue;

            // Only entity-to-entity (Class) relationships
            if (target.specialization !== "Class") continue;

            const associationName = navEnd.getName(); // e.g. "CustomerAddresses"
            const isCollection = !!navEnd.typeReference.isCollection;
            const isNullable = !!navEnd.typeReference.isNullable;

            // Your heuristic: required => composite/owned
            const isComposite = !isNullable;
            if (!isComposite) continue;

            results.push({
                id: navEnd.id,                 // stable id for nav end
                associationName,
                sourceEntity: entity,
                targetEntity: target,
                isCollection,
                isNullable,
                targetAttributes: getEntityAttributes(target),
                icon: target.getIcon()
            });
        } catch (e) {
            continue;
        }
    }

    return results;
}

// Find DTO fields that correspond to associated entities
function findDtoAssociationFields(dtoElement: MacroApi.Context.IElementApi, associations: IAssociationMetadata[]): IDtoAssociationField[] {
    const dtoFields = getDtoFields(dtoElement);
    const results: IDtoAssociationField[] = [];

    for (const assoc of associations) {
        const targetEntityId = assoc.targetEntity.id;

        // 1) Type-based match: DTO field type resolves to target entity or to a DTO that maps to it
        let match = dtoFields.find(f => {
            if (!f.typeId) return false;
            try {
                const fieldType = lookup(f.typeId) as MacroApi.Context.IElementApi;
                if (!fieldType) return false;

                // direct entity type
                if (fieldType.id === targetEntityId) return true;

                // if fieldType is DTO, sometimes it is mapped/represents the entity - if you have metadata/represents, check it
                // (defensive: represents isn't on IElementApi but may exist on IMappableElementApi)
                const anyType: any = fieldType as any;
                if (typeof anyType.represents === "string" && anyType.represents === targetEntityId) return true;
            } catch { }
            return false;
        });

        // 2) Fallback name match
        if (!match) {
            const assocName = assoc.associationName.toLowerCase();
            match = dtoFields.find(f => f.name.toLowerCase() === assocName);
        }

        if (!match) {
            continue;
        }

        results.push({
            id: match.id,
            name: match.name,
            typeId: match.typeId,
            typeDisplayText: match.typeDisplayText,
            isCollection: assoc.isCollection,
            isNullable: assoc.isNullable,
            icon: match.icon
        });
    }

    return results;
}

function getGroupingPath(d: IFieldDiscrepancy): string | null {
  // Prefer dtoFieldName because it already contains nesting like "ClientAddresses.Line111"
  const dto = d.dtoFieldName ?? "";
  const ent = d.entityAttributeName ?? "";

  // If either contains a dot, treat it as nested and group by it.
  if (dto.includes(".")) return dto;
  if (ent.includes(".")) return ent;

  // If it's an association field itself (ClientAddresses) you can still return it,
  // but typically you only want grouping for dotted paths.
  return null;
}

function ensureGroupNode(
  rootMap: Map<string, MacroApi.Context.ISelectableTreeNode>,
  fullPath: string,
  icon?: any
): MacroApi.Context.ISelectableTreeNode {
  const parts = fullPath.split(".").filter(Boolean);

  let currentMap = rootMap;
  let currentNode: MacroApi.Context.ISelectableTreeNode | null = null;
  let prefix = "";

  for (let i = 0; i < parts.length; i++) {
    prefix = prefix ? `${prefix}.${parts[i]}` : parts[i];

    let node = currentMap.get(prefix);
    if (!node) {
      node = {
        id: `group-${prefix}`,                 // synthetic id
        label: parts[i],                       // only the segment label
        specializationId: "sync-group-node",   // NEW specialization
        isExpanded: true,
        isSelected: false,
        icon,
        children: []
      };
      currentMap.set(prefix, node);

      if (currentNode?.children) {
        currentNode.children.push(node);
      }
    }

    currentNode = node;

    // Build a child map for next level
    // We'll store it on the node for convenience.
    (node as any).__childMap ??= new Map<string, MacroApi.Context.ISelectableTreeNode>();
    currentMap = (node as any).__childMap;
  }

  return currentNode!;
}
