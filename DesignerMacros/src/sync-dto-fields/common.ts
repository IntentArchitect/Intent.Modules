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

function isValidSyncElement(element: MacroApi.Context.IElementApi): boolean {
    return (VALID_SPECIALIZATIONS as readonly string[]).includes(element.specialization);
}

function getValidSpecializations(): string[] {
    return [...VALID_SPECIALIZATIONS];
}

function extractDtoFromElement(element: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi | null {
    console.log(`[extractDtoFromElement] Element: ${element.getName()} (${element.specialization})`);
    
    if ((DTO_LIKE_SPECIALIZATIONS as readonly string[]).includes(element.specialization)) {
        console.log(`  → Element itself is DTO-like`);
        return element;
    }
    
    if (element.specialization === "Operation") {
        const parameters = element.getChildren(ELEMENT_TYPE_NAMES.PARAMETER);
        console.log(`  → Operation with ${parameters.length} parameters, searching for DTO...`);
        
        for (const param of parameters) {
            const typeRef = param.typeReference;
            if (typeRef && typeRef.isTypeFound()) {
                const type = typeRef.getType() as MacroApi.Context.IElementApi;
                console.log(`    - Param "${param.getName()}" type: ${type.specialization}`);
                
                if ((DTO_LIKE_SPECIALIZATIONS as readonly string[]).includes(type.specialization)) {
                    console.log(`      → Found DTO: ${type.getName()}`);
                    return type;
                }
            }
        }
        console.log(`  → No DTO parameter found`);
    }
    
    return null;
}

function findAssociationsPointingToElement(searchElement: MacroApi.Context.IElementApi, dtoElement: MacroApi.Context.IElementApi): MacroApi.Context.IAssociationApi[] {
    const allAssociations: MacroApi.Context.IAssociationApi[] = [];
    
    console.log(`[findAssociationsPointingToElement] Search element: ${searchElement.getName()} (${searchElement.specialization}), DTO: ${dtoElement.getName()}`);
    
    // First try to get associations from searchElement
    for (const actionName of ENTITY_ACTION_TYPES) {
        try {
            // SDK limitation: getAssociations() return type needs casting to IAssociationApi[]
            const results = searchElement.getAssociations(actionName);
            
            if (results && results.length > 0) {
                console.log(`  Found ${results.length} associations of type '${actionName}'`);
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
                    console.log(`  Filtered to ${filtered.length} associations matching DTO`);
                    if (filtered.length > 0) {
                        allAssociations.push(...filtered);
                    }
                }
            }
        } catch (e) {
            // SDK method may fail on certain association types
        }
    }
    
    console.log(`[findAssociationsPointingToElement] Total associations: ${allAssociations.length}`);
    
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
    console.log(`[getEntityFromAssociations] Processing ${associations.length} associations`);
    
    if (associations.length === 0) return null;
    
    // Association typeReference points to the entity being acted upon
    const association = associations[0];
    if (association.typeReference && association.typeReference.isTypeFound()) {
        const entity = association.typeReference.getType() as MacroApi.Context.IElementApi;
        console.log(`  Found entity: ${entity.getName()}`);
        return entity;
    }
    
    console.log(`  No entity found`);
    return null;
}

function getDtoFields(dtoElement: MacroApi.Context.IElementApi): IDtoField[] {
    const fields: IDtoField[] = [];
    
    // Dynamically determine child type instead of hard-coding "DTO-Field"
    const childType = inferSourceElementChildType(dtoElement);
    console.log(`[getDtoFields] DTO: ${dtoElement.getName()}, Child type: ${childType}`);
    
    const children = dtoElement.getChildren(childType);
    console.log(`  Found ${children.length} fields`);
    
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
    console.log(`[getEntityAttributes] Entity: ${entity.getName()}, Child type: ${childType}`);
    
    const children = entity.getChildren(childType);
    console.log(`  Found ${children.length} attributes`);
    
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
                        const sourceFieldId = sourcePath[sourcePath.length - 1].id;
                        const targetAttributeId = targetPath[targetPath.length - 1].id;
                        
                        console.log(`[extractFieldMappings] Found mapping:`);
                        console.log(`  Source path length: ${sourcePath.length}`);
                        console.log(`  Target path length: ${targetPath.length}`);
                        
                        mappings.push({
                            sourcePath: sourcePath.map(p => p.id),
                            targetPath: targetPath.map(p => p.id),
                            sourceFieldId: sourceFieldId,
                            targetAttributeId: targetAttributeId,
                            mappingType: mappedEnd.mappingType
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
    
    console.log(`[extractParameterMappings] Processing ${associations.length} associations`);
    
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
                    
                    console.log(`  Checking path: last source = ${lastSourceElement.name} (${sourceSpecialization}), length = ${sourcePath.length}`);
                    
                    // Only process if last source element is a Parameter (primitive parameters have direct mappings)
                    if (sourceSpecialization === "Parameter") {
                        const paramId = lastSourceElement.id;
                        const targetAttributeId = targetPath[targetPath.length - 1].id;
                        
                        parameterMappings.set(paramId, targetAttributeId);
                        console.log(`    ✓ Parameter ${lastSourceElement.name} -> Attribute ${targetPath[targetPath.length - 1].name}`);
                    } else {
                        console.log(`    ✗ Skipping (source is ${sourceSpecialization}, not Parameter)`);
                    }
                }
            }
        } catch (error) {
            // Association may not have advanced mappings configured
            continue;
        }
    }
    
    console.log(`[extractParameterMappings] Found ${parameterMappings.size} parameter mappings`);
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
    
    // Analyze the first non-invocation mapping to determine path structure
    const dataMappings = mappings.filter(m => 
        m.mappingType !== "Invocation Mapping" && 
        m.sourcePath.length > 0 && 
        m.targetPath.length > 0
    );
    
    if (dataMappings.length === 0) return null;
    
    const sampleMapping = dataMappings[0];
    
    console.log(`[analyzePathStructure] Analyzing path structure:`);
    console.log(`  Source path IDs length: ${sampleMapping.sourcePath.length}`);
    console.log(`  Target path IDs length: ${sampleMapping.targetPath.length}`);
    
    // Build element type arrays by looking up each element in the path
    const sourceElementTypes: string[] = [];
    const targetElementTypes: string[] = [];
    
    for (const elementId of sampleMapping.sourcePath) {
        try {
            const element = lookup(elementId);
            sourceElementTypes.push(element.specialization);
            console.log(`    Source: ${element.getName()} (${element.specialization})`);
        } catch {
            sourceElementTypes.push("Unknown");
        }
    }
    
    for (const elementId of sampleMapping.targetPath) {
        try {
            const element = lookup(elementId);
            targetElementTypes.push(element.specialization);
            console.log(`    Target: ${element.getName()} (${element.specialization})`);
        } catch {
            targetElementTypes.push("Unknown");
        }
    }
    
    // Create signature: "Operation>Parameter>DTO-Field -> Class>Attribute"
    const signature = `${sourceElementTypes.join(">")} -> ${targetElementTypes.join(">")}`;
    
    console.log(`  Computed signature: ${signature}`);
    console.log(`  Source depth: ${sampleMapping.sourcePath.length}, Target depth: ${sampleMapping.targetPath.length}`);
    
    return {
        sourceDepth: sampleMapping.sourcePath.length,
        targetDepth: sampleMapping.targetPath.length,
        sourceElementTypes: sourceElementTypes,
        targetElementTypes: targetElementTypes,
        signature: signature,
        mappingType: sampleMapping.mappingType || "Data Mapping"
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
    
    console.log(`[buildPathUsingTemplate] Starting path build`);
    console.log(`  Source root: ${sourceRoot.getName()} (${sourceRoot.specialization}), depth: ${template.sourceDepth}`);
    console.log(`  Target root: ${targetRoot.getName()} (${targetRoot.specialization}), depth: ${template.targetDepth}`);
    console.log(`  Template source types: ${template.sourceElementTypes.join(" > ")}`);
    console.log(`  Template target types: ${template.targetElementTypes.join(" > ")}`);
    console.log(`  New field ID: ${newFieldId.substring(0, 8)}..., Target attribute ID: ${targetAttributeId.substring(0, 8)}...`);
    if (dtoElement) {
        console.log(`  DTO element provided: ${dtoElement.getName()} (${dtoElement.specialization})`);
    }
    
    // Build source path based on template depth and structure
    if (template.sourceDepth === 1) {
        // Simple: [fieldId]
        sourcePath.push(newFieldId);
        console.log(`  Source depth 1 case`);
    } else if (template.sourceDepth === 2) {
        // Two levels: [parent, fieldId]
        // For Operations: parent is Parameter, not the Operation itself
        // For DTOs: parent is the DTO itself
        if (sourceRoot.specialization === "Operation") {
            // For operations, we need to find the Parameter that references the DTO
            console.log(`  Source depth 2 case with Operation sourceRoot`);
            console.log(`    Looking for Parameter in Operation that references DTO`);
            const parameter = findIntermediateElement(sourceRoot, "Parameter", dtoElement);
            if (parameter) {
                sourcePath.push(parameter.id, newFieldId);
                console.log(`    ✓ Using Parameter: ${parameter.getName()}`);
            } else {
                // Fallback
                sourcePath.push(sourceRoot.id, newFieldId);
                console.log(`    ✗ Fallback to Operation`);
            }
        } else {
            // For DTOs or other elements, use sourceRoot
            sourcePath.push(sourceRoot.id, newFieldId);
            console.log(`  Source depth 2 case`);
        }
    } else if (template.sourceDepth === 3) {
        // Three levels: Need to find intermediate element
        // For Operations with DTO parameters: [operationId, parameterId, fieldId]
        console.log(`  Source depth 3 case`);
        console.log(`    Looking for intermediate element of type: ${template.sourceElementTypes[1]}`);
        const intermediateElement = findIntermediateElement(sourceRoot, template.sourceElementTypes[1], dtoElement);
        if (intermediateElement) {
            sourcePath.push(sourceRoot.id, intermediateElement.id, newFieldId);
            console.log(`    ✓ Found intermediate: ${intermediateElement.getName()}`);
        } else {
            // Fallback to simpler path
            sourcePath.push(sourceRoot.id, newFieldId);
            console.log(`    ✗ Could not find intermediate, using fallback path`);
        }
    } else {
        // For deeper paths, attempt to reconstruct
        console.log(`  Source depth ${template.sourceDepth} case (deep path)`);
        sourcePath.push(sourceRoot.id);
        // Add intermediate elements if needed
        for (let i = 1; i < template.sourceDepth - 1; i++) {
            const intermediateType = template.sourceElementTypes[i];
            const parentElement = sourcePath.length === 1 ? sourceRoot : lookup(sourcePath[sourcePath.length - 1]);
            const intermediate = findIntermediateElement(parentElement, intermediateType, dtoElement);
            if (intermediate) {
                sourcePath.push(intermediate.id);
                console.log(`    Added intermediate ${i}: ${intermediate.getName()}`);
            }
        }
        sourcePath.push(newFieldId);
    }
    
    // Build target path based on template depth
    if (template.targetDepth === 1) {
        // Simple: [attributeId]
        targetPath.push(targetAttributeId);
        console.log(`  Target depth 1 case`);
    } else if (template.targetDepth === 2) {
        // Two levels: [entityId, attributeId]
        targetPath.push(targetRoot.id, targetAttributeId);
        console.log(`  Target depth 2 case`);
    } else {
        // For deeper paths
        console.log(`  Target depth ${template.targetDepth} case (deep path)`);
        targetPath.push(targetRoot.id);
        for (let i = 1; i < template.targetDepth - 1; i++) {
            // Add intermediate elements based on template
            const intermediateType = template.targetElementTypes[i];
            const parentElement = lookup(targetPath[targetPath.length - 1]);
            const intermediate = findIntermediateElement(parentElement, intermediateType);
            if (intermediate) {
                targetPath.push(intermediate.id);
                console.log(`    Added intermediate ${i}: ${intermediate.getName()}`);
            }
        }
        targetPath.push(targetAttributeId);
    }
    
    console.log(`  Final source path: [${sourcePath.map((id, i) => {
        try {
            const el = lookup(id);
            return `${el.getName()}(${el.specialization})`;
        } catch {
            return id.substring(0, 8) + "...";
        }
    }).join(" → ")}]`);
    console.log(`  Final target path: [${targetPath.map((id, i) => {
        try {
            const el = lookup(id);
            return `${el.getName()}(${el.specialization})`;
        } catch {
            return id.substring(0, 8) + "...";
        }
    }).join(" → ")}]`);
    
    return { sourcePath, targetPath };
}

function findIntermediateElement(parent: MacroApi.Context.IElementApi, expectedType: string, dtoElement?: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi | null {
    console.log(`  Finding intermediate element of type '${expectedType}' in parent '${parent.getName()}' (${parent.specialization})`);
    console.log(`  DtoElement provided: ${dtoElement ? dtoElement.getName() : "null"}`);
    
    // Special case: For Operations with Parameters, find the parameter that references the DTO
    if (expectedType === "Parameter" && parent.specialization === "Operation" && dtoElement) {
        console.log(`  Looking for Parameter that references DTO '${dtoElement.getName()}'`);
        const parameters = parent.getChildren("Parameter");
        console.log(`  Found ${parameters.length} parameters`);
        
        for (const param of parameters) {
            console.log(`    Checking parameter '${param.getName()}'...`);
            if (param.typeReference && param.typeReference.isTypeFound()) {
                const paramType = param.typeReference.getType();
                const paramTypeId = paramType.id;
                const dtoElementId = dtoElement.id;
                
                console.log(`      Parameter type: ${paramType.getName()} (id: ${paramTypeId})`);
                console.log(`      DTO element id: ${dtoElementId}`);
                console.log(`      Match: ${paramTypeId === dtoElementId}`);
                
                if (paramTypeId === dtoElementId) {
                    console.log(`      ✓ Found matching parameter!`);
                    return param;
                }
            } else {
                console.log(`      Parameter type not found or not navigable`);
            }
        }
        console.log(`  No parameter found referencing DTO`);
    }
    
    // Try to find a child element of the expected type
    const children = parent.getChildren(expectedType);
    console.log(`  Children of type '${expectedType}': ${children.length}`);
    if (children && children.length > 0) {
        console.log(`  ✓ Returning first child: ${children[0].getName()}`);
        return children[0];
    }
    
    // Try associations
    const associations = parent.getAssociations();
    for (const assoc of associations) {
        try {
            const target = assoc.typeReference?.getType();
            if (target && target.specialization === expectedType) {
                console.log(`  ✓ Found via association: ${target.getName()}`);
                return target as MacroApi.Context.IElementApi;
            }
        } catch {
            continue;
        }
    }
    
    console.log(`  ✗ No intermediate element found`);
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