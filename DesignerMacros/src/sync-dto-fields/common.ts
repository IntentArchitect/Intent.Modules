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
        return association.typeReference.getType() as MacroApi.Context.IElementApi;
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

