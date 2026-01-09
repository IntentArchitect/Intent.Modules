/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
// Constants
const VALID_SPECIALIZATIONS = ["DTO", "Command", "Query", "Operation"];
const DTO_LIKE_SPECIALIZATIONS = ["DTO", "Command", "Query"];
const ENTITY_ACTION_TYPES = [
    "Create Entity Action",
    "Update Entity Action",
    "Delete Entity Action",
    "Query Entity Action"
];
const MAX_HIERARCHY_DEPTH = 10;
const ELEMENT_TYPE_NAMES = {
    DTO_FIELD: "DTO-Field",
    ATTRIBUTE: "Attribute",
    PARAMETER: "Parameter"
};
const METADATA_KEYS = {
    IS_MANAGED_KEY: "is-managed-key"
};
// Build structured field type information from typeReference
function buildFieldType(typeReference, displayText) {
    var _a, _b;
    if (!typeReference || !((_a = typeReference.isTypeFound) === null || _a === void 0 ? void 0 : _a.call(typeReference))) {
        return {
            baseType: "Unknown",
            isCollection: false,
            isNullable: false,
            displayText: displayText || "Unknown"
        };
    }
    const isCollection = typeReference.isCollection || false;
    const isNullable = typeReference.isNullable || false;
    const baseType = ((_b = typeReference.display) === null || _b === void 0 ? void 0 : _b.replace(/[\[\]?]/g, "")) || (displayText === null || displayText === void 0 ? void 0 : displayText.replace(/[\[\]?]/g, "")) || "Unknown";
    let displayTextFinal = baseType;
    if (isCollection)
        displayTextFinal += "[]";
    if (isNullable)
        displayTextFinal += "?";
    return {
        baseType: baseType,
        isCollection: isCollection,
        isNullable: isNullable,
        displayText: displayTextFinal
    };
}
function isValidSyncElement(element) {
    return VALID_SPECIALIZATIONS.includes(element.specialization);
}
function getValidSpecializations() {
    return [...VALID_SPECIALIZATIONS];
}
function extractDtoFromElement(element) {
    if (DTO_LIKE_SPECIALIZATIONS.includes(element.specialization)) {
        return element;
    }
    if (element.specialization === "Operation") {
        const parameters = element.getChildren(ELEMENT_TYPE_NAMES.PARAMETER);
        for (const param of parameters) {
            const typeRef = param.typeReference;
            if (typeRef && typeRef.isTypeFound()) {
                const type = typeRef.getType();
                if (DTO_LIKE_SPECIALIZATIONS.includes(type.specialization)) {
                    return type;
                }
            }
        }
    }
    return null;
}
function findAssociationsPointingToElement(searchElement, dtoElement) {
    const allAssociations = [];
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
                    allAssociations.push(...results);
                }
                else {
                    // When searching from a parent of the DTO, filter to only associations targeting our DTO
                    const filtered = results.filter(assoc => {
                        try {
                            const typeRef = assoc.typeReference;
                            if (typeRef && typeRef.getType()) {
                                const type = typeRef.getType();
                                return type.id === dtoElement.id;
                            }
                        }
                        catch (e) {
                            // SDK may throw if association is incomplete
                        }
                        return false;
                    });
                    if (filtered.length > 0) {
                        allAssociations.push(...filtered);
                    }
                }
            }
        }
        catch (e) {
            // SDK method may fail on certain association types
        }
    }
    // If no associations found and searchElement is a DTO/Command/Query, walk up the hierarchy
    if (allAssociations.length === 0 && DTO_LIKE_SPECIALIZATIONS.includes(searchElement.specialization)) {
        let current = searchElement;
        let depth = 0;
        while (current && allAssociations.length === 0 && depth < MAX_HIERARCHY_DEPTH) {
            // Try all action types on current element
            for (const actionName of ENTITY_ACTION_TYPES) {
                try {
                    // SDK limitation: getAssociations() return type needs casting to IAssociationApi[]
                    const results = current.getAssociations(actionName);
                    if (results && results.length > 0) {
                        // Filter to only associations that reference our DTO element
                        const filtered = results.filter(assoc => {
                            try {
                                const typeRef = assoc.typeReference;
                                if (typeRef && typeRef.getType()) {
                                    const type = typeRef.getType();
                                    return type.id === dtoElement.id;
                                }
                            }
                            catch (e) {
                                // Skip associations that error out
                            }
                            return false;
                        });
                        if (filtered.length > 0) {
                            allAssociations.push(...filtered);
                        }
                    }
                }
                catch (e) {
                    // Continue to next parent if this fails
                }
            }
            if (allAssociations.length > 0)
                break;
            current = current.getParent() || null;
            depth++;
        }
    }
    return allAssociations;
}
function getEntityFromAssociations(associations) {
    if (associations.length === 0)
        return null;
    // Association typeReference points to the entity being acted upon
    const association = associations[0];
    if (association.typeReference && association.typeReference.isTypeFound()) {
        const entity = association.typeReference.getType();
        return entity;
    }
    return null;
}
function getDtoFields(dtoElement) {
    var _a, _b;
    const fields = [];
    // Dynamically determine child type instead of hard-coding "DTO-Field"
    const childType = inferSourceElementChildType(dtoElement);
    const children = dtoElement.getChildren(childType);
    for (const child of children) {
        const field = {
            id: child.id,
            name: child.getName(),
            typeId: (_a = child.typeReference) === null || _a === void 0 ? void 0 : _a.getTypeId(),
            typeDisplayText: ((_b = child.typeReference) === null || _b === void 0 ? void 0 : _b.display) || "",
            isMapped: false, // Will be determined by extractFieldMappings
            mappedToAttributeId: undefined,
            icon: child.getIcon()
        };
        fields.push(field);
    }
    return fields;
}
function getEntityAttributes(entity) {
    var _a, _b;
    const attributes = [];
    // Dynamically determine child type instead of hard-coding "Attribute"
    const childType = inferTargetElementChildType(entity);
    const children = entity.getChildren(childType);
    for (const child of children) {
        const attribute = {
            id: child.id,
            name: child.getName(),
            typeId: (_a = child.typeReference) === null || _a === void 0 ? void 0 : _a.getTypeId(),
            typeDisplayText: ((_b = child.typeReference) === null || _b === void 0 ? void 0 : _b.display) || "",
            icon: child.getIcon(),
            isManagedKey: child.hasMetadata(METADATA_KEYS.IS_MANAGED_KEY) && child.getMetadata(METADATA_KEYS.IS_MANAGED_KEY) === "true"
        };
        attributes.push(attribute);
    }
    return attributes;
}
function extractFieldMappings(associations) {
    const mappings = [];
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
        }
        catch (error) {
            // Association may not have advanced mappings configured
            continue;
        }
    }
    return mappings;
}
function extractParameterMappings(associations) {
    // Returns a map of Parameter ID -> Attribute ID for primitive parameter mappings
    const parameterMappings = new Map();
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
                    const sourceSpecialization = lastSourceElement.specialization || "unknown";
                    // Only process if last source element is a Parameter (primitive parameters have direct mappings)
                    if (sourceSpecialization === "Parameter") {
                        const paramId = lastSourceElement.id;
                        const targetAttributeId = targetPath[targetPath.length - 1].id;
                        parameterMappings.set(paramId, targetAttributeId);
                    }
                }
            }
        }
        catch (error) {
            // Association may not have advanced mappings configured
            continue;
        }
    }
    return parameterMappings;
}
function inferSourceElementChildType(sourceRoot) {
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
function inferTargetElementChildType(targetRoot) {
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
function analyzePathStructure(mappings) {
    if (mappings.length === 0)
        return null;
    // Analyze non-invocation mappings to determine path structure
    const dataMappings = mappings.filter(m => m.mappingType !== "Invocation Mapping" &&
        m.sourcePath.length > 0 &&
        m.targetPath.length > 0);
    if (dataMappings.length === 0)
        return null;
    // PRIORITY: Select the mapping with the LONGEST source path
    // If multiple have the same length, PREFER "Data Mapping" over others like "Filter Mapping"
    const sampleMapping = dataMappings.reduce((current, next) => {
        const currentLength = current.sourcePath.length;
        const nextLength = next.sourcePath.length;
        if (nextLength > currentLength) {
            // Longer path wins
            return next;
        }
        else if (nextLength === currentLength) {
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
    const sourceElementTypes = [];
    const targetElementTypes = [];
    for (const elementId of sampleMapping.sourcePath) {
        try {
            const element = lookup(elementId);
            sourceElementTypes.push(element.specialization);
        }
        catch {
            sourceElementTypes.push("Unknown");
        }
    }
    for (const elementId of sampleMapping.targetPath) {
        try {
            const element = lookup(elementId);
            targetElementTypes.push(element.specialization);
        }
        catch {
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
function buildPathUsingTemplate(template, newFieldId, targetAttributeId, sourceRoot, targetRoot, dtoElement) {
    const sourcePath = [];
    const targetPath = [];
    // Build source path based on template depth and structure
    if (template.sourceDepth === 1) {
        // Simple: [fieldId]
        sourcePath.push(newFieldId);
    }
    else if (template.sourceDepth === 2) {
        // Two levels: [parent, fieldId]
        // For Operations: parent is Parameter, not the Operation itself
        // For DTOs: parent is the DTO itself
        if (sourceRoot.specialization === "Operation") {
            // For operations, we need to find the Parameter that references the DTO
            const parameter = findIntermediateElement(sourceRoot, "Parameter", dtoElement);
            if (parameter) {
                sourcePath.push(parameter.id, newFieldId);
            }
            else {
                // Fallback
                sourcePath.push(sourceRoot.id, newFieldId);
            }
        }
        else {
            // For DTOs or other elements, use sourceRoot
            sourcePath.push(sourceRoot.id, newFieldId);
        }
    }
    else if (template.sourceDepth === 3) {
        // Three levels: Need to find intermediate element
        // For Operations with DTO parameters: [operationId, parameterId, fieldId]
        const intermediateElement = findIntermediateElement(sourceRoot, template.sourceElementTypes[1], dtoElement);
        if (intermediateElement) {
            sourcePath.push(sourceRoot.id, intermediateElement.id, newFieldId);
        }
        else {
            // Fallback to simpler path
            sourcePath.push(sourceRoot.id, newFieldId);
        }
    }
    else {
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
    }
    else if (template.targetDepth === 2) {
        // Two levels: [entityId, attributeId]
        targetPath.push(targetRoot.id, targetAttributeId);
    }
    else {
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
function findIntermediateElement(parent, expectedType, dtoElement) {
    var _a;
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
            const target = (_a = assoc.typeReference) === null || _a === void 0 ? void 0 : _a.getType();
            if (target && target.specialization === expectedType) {
                return target;
            }
        }
        catch {
            continue;
        }
    }
    return null;
}
// Phase 2/3: Generic hierarchical parameter discovery
function discoverParameters(element) {
    var _a, _b;
    const parameters = [];
    const parameterChildren = element.getChildren("Parameter");
    if (!parameterChildren) {
        return parameters;
    }
    for (const param of parameterChildren) {
        const paramNode = {
            id: param.id,
            name: param.getName(),
            type: "Primitive", // Default
            typeId: (_a = param.typeReference) === null || _a === void 0 ? void 0 : _a.getTypeId(),
            typeDisplayText: ((_b = param.typeReference) === null || _b === void 0 ? void 0 : _b.display) || "Unknown",
            icon: param.getIcon()
        };
        // Determine parameter type and discover nested fields
        if (param.typeReference && param.typeReference.isTypeFound()) {
            const paramType = param.typeReference.getType();
            const paramSpecialization = paramType.specialization;
            // Check if it's a DTO or similar complex type
            if (paramSpecialization === "DTO" || paramSpecialization === "Class") {
                paramNode.type = paramSpecialization === "DTO" ? "DTO" : "Complex";
                paramNode.typeId = paramType.id;
                // Recursively discover fields in the DTO/Complex type
                paramNode.children = discoverFields(paramType, 0);
            }
            else {
                // It's a primitive type
                paramNode.type = "Primitive";
            }
        }
        parameters.push(paramNode);
    }
    return parameters;
}
function discoverFields(element, depth) {
    var _a, _b;
    if (depth > MAX_HIERARCHY_DEPTH) {
        return [];
    }
    const fields = [];
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
        const fieldNode = {
            id: field.id,
            name: field.getName(),
            type: "Primitive", // Default
            typeId: (_a = field.typeReference) === null || _a === void 0 ? void 0 : _a.getTypeId(),
            typeDisplayText: ((_b = field.typeReference) === null || _b === void 0 ? void 0 : _b.display) || "Unknown",
            icon: field.getIcon()
        };
        // Check if field type is complex
        if (field.typeReference && field.typeReference.isTypeFound()) {
            const fieldType = field.typeReference.getType();
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
function findMappableElement(element) {
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
function flattenParametersToFields(parameters) {
    const fields = [];
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
        }
        else if (param.children) {
            // Complex parameters have fields that can be mapped
            fields.push(...flattenFieldNodes(param.children));
        }
    }
    return fields;
}
function flattenFieldNodes(nodes) {
    const fields = [];
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
function findParameterNodeById(parameters, id) {
    for (const param of parameters) {
        if (param.id === id) {
            return param;
        }
    }
    return null;
}
// Helper to find a field node by ID in the hierarchy
function findFieldNodeById(fields, id) {
    for (const field of fields) {
        if (field.id === id) {
            return field;
        }
        if (field.children) {
            const found = findFieldNodeById(field.children, id);
            if (found)
                return found;
        }
    }
    return null;
}
// Discover composite associations (entity-to-entity relationships where source owns target)
function discoverCompositeAssociations(entity) {
    var _a, _b;
    const results = [];
    let assocEnds = [];
    try {
        assocEnds = entity.getAssociations("Association");
    }
    catch (e) {
        return results;
    }
    for (const end of assocEnds) {
        try {
            // The "navigation property" is typically the OTHER end
            const navEnd = end.isTargetEnd() ? end : end.getOtherEnd ? end.getOtherEnd() : null;
            if (!navEnd || !navEnd.typeReference || !((_b = (_a = navEnd.typeReference).isTypeFound) === null || _b === void 0 ? void 0 : _b.call(_a)))
                continue;
            const target = navEnd.typeReference.getType();
            if (!target)
                continue;
            // Only entity-to-entity (Class) relationships
            if (target.specialization !== "Class")
                continue;
            const associationName = navEnd.getName(); // e.g. "CustomerAddresses"
            const isCollection = !!navEnd.typeReference.isCollection;
            const isNullable = !!navEnd.typeReference.isNullable;
            // Your heuristic: required => composite/owned
            const isComposite = !isNullable;
            if (!isComposite)
                continue;
            results.push({
                id: navEnd.id, // stable id for nav end
                associationName,
                sourceEntity: entity,
                targetEntity: target,
                isCollection,
                isNullable,
                targetAttributes: getEntityAttributes(target),
                icon: target.getIcon()
            });
        }
        catch (e) {
            continue;
        }
    }
    return results;
}
// Find DTO fields that correspond to associated entities
function findDtoAssociationFields(dtoElement, associations) {
    const dtoFields = getDtoFields(dtoElement);
    const results = [];
    for (const assoc of associations) {
        const targetEntityId = assoc.targetEntity.id;
        // 1) Type-based match: DTO field type resolves to target entity or to a DTO that maps to it
        let match = dtoFields.find(f => {
            if (!f.typeId)
                return false;
            try {
                const fieldType = lookup(f.typeId);
                if (!fieldType)
                    return false;
                // direct entity type
                if (fieldType.id === targetEntityId)
                    return true;
                // if fieldType is DTO, sometimes it is mapped/represents the entity - if you have metadata/represents, check it
                // (defensive: represents isn't on IElementApi but may exist on IMappableElementApi)
                const anyType = fieldType;
                if (typeof anyType.represents === "string" && anyType.represents === targetEntityId)
                    return true;
            }
            catch { }
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
function getGroupingPath(d) {
    var _a, _b;
    // Prefer dtoFieldName because it already contains nesting like "ClientAddresses.Line111"
    const dto = (_a = d.dtoFieldName) !== null && _a !== void 0 ? _a : "";
    const ent = (_b = d.entityAttributeName) !== null && _b !== void 0 ? _b : "";
    // If either contains a dot, treat it as nested and group by it.
    if (dto.includes("."))
        return dto;
    if (ent.includes("."))
        return ent;
    // If it's an association field itself (ClientAddresses) you can still return it,
    // but typically you only want grouping for dotted paths.
    return null;
}
function ensureGroupNode(rootMap, fullPath, icon) {
    var _a;
    var _b;
    const parts = fullPath.split(".").filter(Boolean);
    let currentMap = rootMap;
    let currentNode = null;
    let prefix = "";
    for (let i = 0; i < parts.length; i++) {
        prefix = prefix ? `${prefix}.${parts[i]}` : parts[i];
        let node = currentMap.get(prefix);
        if (!node) {
            node = {
                id: `group-${prefix}`, // synthetic id
                label: parts[i], // only the segment label
                specializationId: "sync-group-node", // NEW specialization
                isExpanded: true,
                isSelected: false,
                icon,
                children: []
            };
            currentMap.set(prefix, node);
            if (currentNode === null || currentNode === void 0 ? void 0 : currentNode.children) {
                currentNode.children.push(node);
            }
        }
        currentNode = node;
        // Build a child map for next level
        // We'll store it on the node for convenience.
        (_a = (_b = node).__childMap) !== null && _a !== void 0 ? _a : (_b.__childMap = new Map());
        currentMap = node.__childMap;
    }
    return currentNode;
}
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
// Discrepancy status colors
const DISCREPANCY_COLORS = {
    NEW: "#22c55e", // Green
    DELETE: "#ef4444", // Red
    RENAME: "#007777", // Teal
    CHANGE_TYPE: "#f97316" // Orange
};
const DISCREPANCY_LABELS = {
    NEW: "[NEW]",
    DELETE: "[DELETE]",
    RENAME: "[RENAME]",
    CHANGE_TYPE: "[CHANGE TYPE]"
};
// Build full type display with modifiers (e.g., "string[]?", "int")
function buildTypeDisplay(baseType, isCollection, isNullable) {
    let display = baseType || "Unknown";
    if (isCollection)
        display += "[]";
    if (isNullable)
        display += "?";
    return display;
}
function renderNode(ctx) {
    // today everything uses formatDiscrepancy, but now you have a single choke point
    // to branch later for association parents, grouping nodes, etc.
    return formatDiscrepancy(ctx.discrepancy, ctx.label);
}
function formatDiscrepancy(discrepancy, cleanFieldName) {
    const components = [];
    const statusInfo = getDiscrepancyStatusInfo(discrepancy.type);
    switch (discrepancy.type) {
        case "NEW":
            // cleanFieldName: type [NEW]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const newTypeDisplay = buildTypeDisplay(discrepancy.entityAttributeType || "", discrepancy.entityIsCollection || false, discrepancy.entityIsNullable || false);
            components.push({ text: newTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.NEW, color: statusInfo.color });
            break;
        case "RENAME":
            // cleanFieldName → targetName: type [RENAME]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            const targetName = discrepancy.entityAttributeName.split('.').pop() || discrepancy.entityAttributeName;
            components.push({ text: targetName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const renameTypeDisplay = buildTypeDisplay(discrepancy.dtoFieldType || "", discrepancy.dtoIsCollection || false, discrepancy.dtoIsNullable || false);
            components.push({ text: renameTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.RENAME, color: statusInfo.color });
            break;
        case "CHANGE_TYPE":
            // cleanFieldName: oldType → newType [CHANGE TYPE]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const oldTypeDisplay = buildTypeDisplay(discrepancy.dtoFieldType || "", discrepancy.dtoIsCollection || false, discrepancy.dtoIsNullable || false);
            components.push({ text: oldTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            const newTypeChangeDisplay = buildTypeDisplay(discrepancy.entityAttributeType || "", discrepancy.entityIsCollection || false, discrepancy.entityIsNullable || false);
            components.push({ text: newTypeChangeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.CHANGE_TYPE, color: statusInfo.color });
            break;
        case "DELETE":
            // cleanFieldName: type [DELETE]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const deleteTypeDisplay = buildTypeDisplay(discrepancy.dtoFieldType || "", discrepancy.dtoIsCollection || false, discrepancy.dtoIsNullable || false);
            components.push({ text: deleteTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.DELETE, color: statusInfo.color });
            break;
    }
    return components;
}
function getDiscrepancyStatusInfo(type) {
    switch (type) {
        case "NEW":
            return { color: DISCREPANCY_COLORS.NEW, cssClass: "keyword" };
        case "DELETE":
            return { color: DISCREPANCY_COLORS.DELETE, cssClass: "typeref" };
        case "RENAME":
            return { color: DISCREPANCY_COLORS.RENAME, cssClass: "annotation" };
        case "CHANGE_TYPE":
            return { color: DISCREPANCY_COLORS.CHANGE_TYPE, cssClass: "muted" };
        default:
            return { color: "#6b7280", cssClass: "" }; // Gray fallback
    }
}
function createDiscrepancyDisplayFunction(discrepancy, cleanFieldName) {
    const fieldName = cleanFieldName || discrepancy.dtoFieldName || discrepancy.entityAttributeName;
    return () => formatDiscrepancy(discrepancy, fieldName);
}
function attachDiscrepancyDisplayFunction(discrepancy, cleanFieldName) {
    // keep name to reduce churn; the "command" is effectively:
    // "use the discrepancy formatter"
    discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy, cleanFieldName);
}
/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="display-formatter.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
/**
 * Structure-first DTO field synchronization engine
 *
 * Current focus: Build correct tree hierarchy
 * (Discrepancy detection deferred until structure is verified)
 */
class FieldSyncEngine {
    constructor() {
        this.lastStructureTree = null;
    }
    /**
     * Main entry point: Build, annotate, and prune tree structure
     * Three-phase approach: BUILD -> ANNOTATE -> PRUNE
     */
    analyzeFieldDiscrepancies(dtoElement, entity, mappings, excludedEntityAttributeIds, sourceElement) {
        console.log(`[BUILD] Starting structure tree for DTO: ${dtoElement.getName()}`);
        console.log(`[BUILD] ├─ Entity: ${entity.getName()}`);
        // Phase 1: Build complete structure and store it
        this.buildStructureTree(dtoElement, entity, mappings, sourceElement);
        // Phase 2: Annotate discrepancies (including nested and operation parameters)
        const discrepancies = [];
        // Check operation parameters if present
        if (sourceElement && sourceElement.specialization === "Operation") {
            this.detectOperationParameterDiscrepancies(sourceElement, dtoElement, entity, mappings, discrepancies);
        }
        // Check DTO fields recursively
        this.detectDiscrepanciesRecursive(dtoElement, entity, mappings, discrepancies, 0);
        this.annotateTreeWithDiscrepancies(this.lastStructureTree, discrepancies);
        console.log(`[ANALYZE] └─ Total discrepancies detected: ${discrepancies.length}`);
        // Phase 3: Prune branches without discrepancies
        this.pruneTreeWithoutDiscrepancies(this.lastStructureTree);
        return discrepancies;
    }
    /**
     * Check operation parameters for discrepancies
     */
    detectOperationParameterDiscrepancies(sourceElement, dtoElement, entity, mappings, discrepancies) {
        const operationParams = sourceElement.getChildren("Parameter");
        console.log(`[ANALYZE-PARAMS] Checking ${operationParams.length} operation parameters`);
        const entityAttrs = getEntityAttributes(entity);
        const entityAttrMap = new Map();
        for (const attr of entityAttrs) {
            entityAttrMap.set(attr.id, attr);
            entityAttrMap.set(attr.name.toLowerCase(), attr);
        }
        // Build a map of parameter ID -> mapping
        const mappingsBySourceId = new Map();
        for (const mapping of mappings) {
            const sourceFieldId = mapping.sourceFieldId;
            if (!mappingsBySourceId.has(sourceFieldId)) {
                mappingsBySourceId.set(sourceFieldId, []);
            }
            mappingsBySourceId.get(sourceFieldId).push(mapping);
        }
        for (const param of operationParams) {
            const paramTypeRef = param.typeReference;
            // Skip the DTO parameter itself
            if (paramTypeRef && paramTypeRef.isTypeFound()) {
                const paramType = paramTypeRef.getType();
                if (paramType && paramType.id === dtoElement.id) {
                    console.log(`[ANALYZE-PARAMS] Skipping DTO parameter: ${param.getName()}`);
                    continue;
                }
            }
            // Check this parameter for discrepancies
            const paramMappings = mappingsBySourceId.get(param.id) || [];
            console.log(`[ANALYZE-PARAMS] Parameter ${param.getName()}: ${paramMappings.length} mappings`);
            if (paramMappings.length === 0) {
                // No mapping - parameter should be deleted or is unused
                const discrepancy = {
                    id: `delete-param-${param.id}`,
                    type: "DELETE",
                    dtoFieldId: param.id,
                    dtoFieldName: param.getName(),
                    dtoFieldType: (paramTypeRef === null || paramTypeRef === void 0 ? void 0 : paramTypeRef.display) || "Unknown",
                    entityAttributeName: "(no mapping)",
                    icon: param.getIcon(),
                    reason: `Parameter '${param.getName()}' is not mapped to any entity attribute`
                };
                discrepancies.push(discrepancy);
                console.log(`[ANALYZE] ├─ [DELETE] Parameter ${param.getName()}: Not mapped to entity`);
            }
            else {
                // Check for renames and type mismatches
                for (const mapping of paramMappings) {
                    const targetAttr = entityAttrMap.get(mapping.targetAttributeId);
                    if (!targetAttr) {
                        continue;
                    }
                    const paramName = param.getName();
                    const entityAttrName = targetAttr.name;
                    // Check for rename - use case-sensitive for parameters
                    if (paramName !== entityAttrName) {
                        const discrepancy = {
                            id: `rename-param-${param.id}`,
                            type: "RENAME",
                            dtoFieldId: param.id,
                            dtoFieldName: paramName,
                            dtoFieldType: (paramTypeRef === null || paramTypeRef === void 0 ? void 0 : paramTypeRef.display) || "Unknown",
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: targetAttr.typeDisplayText,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' should be renamed to '${entityAttrName}'`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`[ANALYZE] ├─ [RENAME] Parameter ${paramName} → ${entityAttrName}`);
                    }
                    // Check for type mismatch
                    const paramType = (paramTypeRef === null || paramTypeRef === void 0 ? void 0 : paramTypeRef.display) || "Unknown";
                    const entityType = targetAttr.typeDisplayText || "Unknown";
                    if (paramType !== entityType) {
                        const discrepancy = {
                            id: `type-param-${param.id}`,
                            type: "CHANGE_TYPE",
                            dtoFieldId: param.id,
                            dtoFieldName: paramName,
                            dtoFieldType: paramType,
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: entityType,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' type mismatch: ${paramType} vs ${entityType}`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`[ANALYZE] ├─ [CHANGE_TYPE] Parameter ${paramName}: ${paramType} → ${entityType}`);
                    }
                }
            }
        }
    }
    /**
     * Recursively detect discrepancies at all levels (root + nested associations)
     * @param parentFieldId - For nested calls, the ID of the field that contains this DTO (for NEW items)
     */
    detectDiscrepanciesRecursive(dtoElement, entity, mappings, discrepancies, depth = 0, parentFieldId) {
        var _a, _b, _c;
        const indent = Array(depth).fill("  ").join("");
        // Get entity attributes to compare against
        const entityAttrs = getEntityAttributes(entity);
        const entityAttrMap = new Map();
        for (const attr of entityAttrs) {
            entityAttrMap.set(attr.id, attr);
            entityAttrMap.set(attr.name.toLowerCase(), attr);
        }
        // Build a map of DTO field ID -> mapping
        const mappingsBySourceId = new Map();
        for (const mapping of mappings) {
            const sourceFieldId = mapping.sourceFieldId;
            if (!mappingsBySourceId.has(sourceFieldId)) {
                mappingsBySourceId.set(sourceFieldId, []);
            }
            mappingsBySourceId.get(sourceFieldId).push(mapping);
        }
        // Check each DTO field for discrepancies
        const childType = inferSourceElementChildType(dtoElement);
        const dtoChildren = dtoElement.getChildren(childType);
        for (const dtoField of dtoChildren) {
            const fieldMappings = mappingsBySourceId.get(dtoField.id) || [];
            const fieldTypeRef = dtoField.typeReference;
            // Check if this is a nested DTO/association field
            let isAssociationField = false;
            let nestedEntity = null;
            let nestedMappings = [];
            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType();
                // Check if field type itself maps to entity attributes through associations
                if (fieldType && (fieldType.specialization === "DTO" || fieldType.specialization === "Class")) {
                    // This might be a nested DTO that maps to an associated entity
                    nestedEntity = this.findAssociatedEntity(dtoField, entity, mappings);
                    if (nestedEntity) {
                        isAssociationField = true;
                        nestedMappings = this.getNestedMappings(mappings, dtoField.id);
                        // Handle association/nested field recursively
                        console.log(`${indent}[ANALYZE-NESTED] Analyzing nested DTO field: ${dtoField.getName()} → ${nestedEntity.getName()}`);
                        this.detectDiscrepanciesRecursive(fieldType, nestedEntity, nestedMappings, discrepancies, depth + 1, dtoField.id);
                    }
                }
            }
            // Skip if already handled as association field
            if (isAssociationField) {
                continue;
            }
            // Handle root-level field
            if (fieldMappings.length === 0) {
                // No mapping found - this field should be deleted
                const discrepancy = {
                    id: `delete-${dtoField.id}`,
                    type: "DELETE",
                    dtoFieldId: dtoField.id,
                    dtoFieldName: dtoField.getName(),
                    dtoFieldType: ((_a = dtoField.typeReference) === null || _a === void 0 ? void 0 : _a.display) || "Unknown",
                    entityAttributeName: "(no mapping)",
                    icon: dtoField.getIcon(),
                    reason: `Field '${dtoField.getName()}' is not mapped to any entity attribute`
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [DELETE] ${dtoField.getName()}: Not mapped to entity`);
            }
            else {
                // Check each mapping for type matches and renames
                for (const mapping of fieldMappings) {
                    const targetAttr = entityAttrMap.get(mapping.targetAttributeId);
                    if (!targetAttr) {
                        continue;
                    }
                    const dtoFieldName = dtoField.getName();
                    const entityAttrName = targetAttr.name;
                    // Check for rename
                    if (dtoFieldName !== entityAttrName) {
                        const discrepancy = {
                            id: `rename-${dtoField.id}`,
                            type: "RENAME",
                            dtoFieldId: dtoField.id,
                            dtoFieldName: dtoFieldName,
                            dtoFieldType: ((_b = dtoField.typeReference) === null || _b === void 0 ? void 0 : _b.display) || "Unknown",
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: targetAttr.typeDisplayText,
                            icon: dtoField.getIcon(),
                            reason: `Field '${dtoFieldName}' should be renamed to '${entityAttrName}'`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`${indent}[ANALYZE] ├─ [RENAME] ${dtoFieldName} → ${entityAttrName}`);
                    }
                    // Check for type mismatch
                    const dtoType = ((_c = dtoField.typeReference) === null || _c === void 0 ? void 0 : _c.display) || "Unknown";
                    const entityType = targetAttr.typeDisplayText || "Unknown";
                    if (dtoType !== entityType) {
                        const discrepancy = {
                            id: `type-${dtoField.id}`,
                            type: "CHANGE_TYPE",
                            dtoFieldId: dtoField.id,
                            dtoFieldName: dtoFieldName,
                            dtoFieldType: dtoType,
                            entityAttributeId: targetAttr.id,
                            entityAttributeName: entityAttrName,
                            entityAttributeType: entityType,
                            icon: dtoField.getIcon(),
                            reason: `Field '${dtoFieldName}' type mismatch: ${dtoType} vs ${entityType}`
                        };
                        discrepancies.push(discrepancy);
                        console.log(`${indent}[ANALYZE] ├─ [CHANGE_TYPE] ${dtoFieldName}: ${dtoType} → ${entityType}`);
                    }
                }
            }
        }
        // Check for NEW fields (entity attributes not covered by any DTO field)
        // This should run at ALL levels, not just root
        const mappedEntityIds = new Set();
        for (const mapping of mappings) {
            mappedEntityIds.add(mapping.targetAttributeId);
        }
        for (const entityAttr of entityAttrs) {
            if (!mappedEntityIds.has(entityAttr.id) && !entityAttr.isManagedKey) {
                // Use parentFieldId if provided (nested context), otherwise use dtoElement.id (root)
                const contextId = parentFieldId || dtoElement.id;
                const discrepancy = {
                    id: `new-${entityAttr.id}-${contextId}`,
                    type: "NEW",
                    dtoFieldId: contextId, // ID of the field/DTO where this should be added
                    dtoFieldName: "(missing)",
                    dtoFieldType: "N/A",
                    entityAttributeId: entityAttr.id,
                    entityAttributeName: entityAttr.name,
                    entityAttributeType: entityAttr.typeDisplayText,
                    icon: entityAttr.icon,
                    reason: `Entity attribute '${entityAttr.name}' is not present in DTO`
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${entityAttr.name}: Entity attribute not in DTO`);
            }
        }
    }
    /**
     * Find the associated entity for a nested DTO field
     * Match by the association name (e.g., "Block1Level3" field should match "Block1Level3" association)
     */
    findAssociatedEntity(dtoField, entity, mappings) {
        const dtoFieldName = dtoField.getName();
        const dtoFieldTypeRef = dtoField.typeReference;
        // Get the actual DTO type that this field references
        if (!dtoFieldTypeRef || !dtoFieldTypeRef.isTypeFound()) {
            return null;
        }
        const nestedDtoType = dtoFieldTypeRef.getType();
        if (!nestedDtoType) {
            return null;
        }
        // Look through entity associations to find matching target
        const associations = entity.getAssociations();
        for (const assoc of associations) {
            // Get association name - this should match the DTO field name or its singular form
            const assocName = assoc.getName ? assoc.getName() : null;
            // Try to match by name first (e.g., "Block1Level3" matches "Block1Level3")
            // Handle both singular and plural (e.g., "Block1Level2s" might have association "Block1Level2")
            const fieldNameSingular = dtoFieldName.replace(/s$/, ''); // Simple pluralization removal
            if (assocName && (assocName === dtoFieldName || assocName === fieldNameSingular)) {
                // Found matching association by name - get its target entity
                if (assoc.typeReference && assoc.typeReference.isTypeFound()) {
                    return assoc.typeReference.getType();
                }
            }
        }
        return null;
    }
    /**
     * Get mappings for a nested DTO field
     * For nested fields, the source path will be: [dto_param_id, parent_field_id, nested_field_id, ...]
     * We want mappings where parent_field_id is in the path
     */
    getNestedMappings(allMappings, parentFieldId) {
        const nestedMappings = [];
        for (const mapping of allMappings) {
            if (mapping.sourcePath && mapping.sourcePath.length > 1) {
                // Check if parent field is in the path (but not the last element)
                const parentIndex = mapping.sourcePath.indexOf(parentFieldId);
                if (parentIndex >= 0 && parentIndex < mapping.sourcePath.length - 1) {
                    // This mapping is for a nested field under this parent
                    // Create a new mapping with adjusted paths
                    const adjustedMapping = {
                        sourcePath: mapping.sourcePath.slice(parentIndex + 1), // Remove parent and ancestors
                        targetPath: mapping.targetPath, // Keep full target path for now
                        sourceFieldId: mapping.sourceFieldId,
                        targetAttributeId: mapping.targetAttributeId,
                        mappingType: mapping.mappingType,
                        mappingTypeId: mapping.mappingTypeId
                    };
                    nestedMappings.push(adjustedMapping);
                }
            }
        }
        return nestedMappings;
    }
    /**
     * Build the complete natural tree structure from DTO/entity
     */
    buildStructureTree(dtoElement, entity, mappings, sourceElement) {
        const rootNode = {
            id: dtoElement.id,
            label: dtoElement.getName(),
            specializationId: "structure-root",
            elementId: dtoElement.id,
            elementType: "DTO",
            originalName: dtoElement.getName(),
            originalType: undefined,
            hasDiscrepancies: true, // Set to true so it shows in tree
            isExpanded: true,
            isSelected: false,
            icon: dtoElement.getIcon(),
            children: []
        };
        // If sourceElement is an Operation, show its parameters first
        if (sourceElement && sourceElement.specialization === "Operation") {
            const operationParams = sourceElement.getChildren("Parameter");
            console.log(`[BUILD] ├─ Operation parameters (${operationParams.length}):`);
            for (const param of operationParams) {
                const paramTypeRef = param.typeReference;
                const paramTypeName = (paramTypeRef === null || paramTypeRef === void 0 ? void 0 : paramTypeRef.display) || "Unknown";
                // Skip the DTO parameter itself - we'll show its fields instead
                if (paramTypeRef && paramTypeRef.isTypeFound()) {
                    const paramType = paramTypeRef.getType();
                    if (paramType && paramType.id === dtoElement.id) {
                        console.log(`[BUILD] │  ├─ (Skipping DTO parameter: ${param.getName()})`);
                        continue; // Skip, we handle it separately
                    }
                }
                console.log(`[BUILD] │  ├─ ${param.getName()}: ${paramTypeName}`);
                const paramNode = {
                    id: param.id,
                    label: `${param.getName()}: ${paramTypeName}`,
                    specializationId: "structure-operation-param",
                    elementId: param.id,
                    elementType: "Operation-Parameter",
                    originalName: param.getName(),
                    originalType: paramTypeName,
                    hasDiscrepancies: true,
                    isExpanded: true,
                    isSelected: false,
                    icon: param.getIcon(),
                    children: []
                };
                rootNode.children.push(paramNode);
            }
        }
        // Add DTO fields (direct properties)
        const childType = inferSourceElementChildType(dtoElement);
        const dtoChildren = dtoElement.getChildren(childType);
        console.log(`[BUILD] ├─ DTO fields (${dtoChildren.length}):`);
        for (const dtoChild of dtoChildren) {
            const fieldTypeRef = dtoChild.typeReference;
            const fieldTypeName = (fieldTypeRef === null || fieldTypeRef === void 0 ? void 0 : fieldTypeRef.display) || "Unknown";
            console.log(`[BUILD] │  ├─ ${dtoChild.getName()}: ${fieldTypeName}`);
            const fieldNode = {
                id: dtoChild.id,
                label: `${dtoChild.getName()}: ${fieldTypeName}`,
                specializationId: "structure-dto-field",
                elementId: dtoChild.id,
                elementType: "DTO-Field",
                originalName: dtoChild.getName(),
                originalType: fieldTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: dtoChild.getIcon(),
                children: []
            };
            // Check if this is a complex type and add nested fields
            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType();
                if (fieldType && (fieldType.specialization === "DTO" || fieldType.specialization === "Class")) {
                    this.addNestedDtoFields(fieldNode, fieldType);
                }
            }
            rootNode.children.push(fieldNode);
        }
        // NOTE: Entity attributes and associations are used for discrepancy detection
        // but we don't show them in the tree visualization for now
        // Only show the DTO fields structure
        // Store the built tree for later use
        this.lastStructureTree = rootNode;
    }
    addNestedDtoFields(parentNode, fieldType) {
        const childType = inferSourceElementChildType(fieldType);
        const children = fieldType.getChildren(childType);
        for (const child of children) {
            const childTypeRef = child.typeReference;
            const childTypeName = (childTypeRef === null || childTypeRef === void 0 ? void 0 : childTypeRef.display) || "Unknown";
            console.log(`[BUILD] │  │  ├─ ${child.getName()}: ${childTypeName}`);
            const childNode = {
                id: child.id,
                label: `${child.getName()}: ${childTypeName}`,
                specializationId: "structure-nested-field",
                elementId: child.id,
                elementType: "Nested-Field",
                originalName: child.getName(),
                originalType: childTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: child.getIcon(),
                children: []
            };
            // Recursively add nested fields
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const nestedType = childTypeRef.getType();
                if (nestedType && (nestedType.specialization === "DTO" || nestedType.specialization === "Class")) {
                    this.addNestedDtoFieldsWithDepth(childNode, nestedType, 3);
                }
            }
            parentNode.children.push(childNode);
        }
    }
    addNestedDtoFieldsWithDepth(parentNode, fieldType, depth) {
        const childType = inferSourceElementChildType(fieldType);
        const children = fieldType.getChildren(childType);
        const indent = Array(depth).fill("│  ").join("");
        for (const child of children) {
            const childTypeRef = child.typeReference;
            const childTypeName = (childTypeRef === null || childTypeRef === void 0 ? void 0 : childTypeRef.display) || "Unknown";
            console.log(`[BUILD] ${indent}├─ ${child.getName()}: ${childTypeName}`);
            const childNode = {
                id: child.id,
                label: `${child.getName()}: ${childTypeName}`,
                specializationId: "structure-nested-field",
                elementId: child.id,
                elementType: "Nested-Field",
                originalName: child.getName(),
                originalType: childTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: child.getIcon(),
                children: []
            };
            // Recursively add nested fields
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const nestedType = childTypeRef.getType();
                if (nestedType && (nestedType.specialization === "DTO" || nestedType.specialization === "Class")) {
                    this.addNestedDtoFieldsWithDepth(childNode, nestedType, depth + 1);
                }
            }
            parentNode.children.push(childNode);
        }
    }
    /**
     * PRUNE phase: Annotate tree nodes with discrepancies, then remove branches without discrepancies
     */
    annotateTreeWithDiscrepancies(node, discrepancies) {
        // First, add NEW discrepancies as synthetic nodes to the tree
        this.addNewDiscrepancyNodes(node, discrepancies);
        // Create a map of element IDs that have discrepancies
        const elementIdsWithDiscrepancies = new Set();
        const discrepancyByElementId = new Map();
        for (const disc of discrepancies) {
            if (disc.dtoFieldId) {
                elementIdsWithDiscrepancies.add(disc.dtoFieldId);
                discrepancyByElementId.set(disc.dtoFieldId, disc);
            }
            if (disc.entityAttributeId) {
                elementIdsWithDiscrepancies.add(disc.entityAttributeId);
                discrepancyByElementId.set(disc.entityAttributeId, disc);
            }
        }
        // Recursively annotate the tree
        return this.annotateNodeRecursive(node, elementIdsWithDiscrepancies, discrepancyByElementId);
    }
    /**
     * Add NEW discrepancies as synthetic nodes to the tree
     * These represent entity attributes that don't exist in the DTO
     */
    addNewDiscrepancyNodes(node, discrepancies) {
        // Find NEW discrepancies that belong to this node (by parent DTO element ID)
        const newDiscrepanciesForThisNode = discrepancies.filter(d => d.type === "NEW" && d.dtoFieldId === node.elementId);
        // Add them as children
        for (const newDisc of newDiscrepanciesForThisNode) {
            const newNode = {
                id: newDisc.id,
                label: `${newDisc.entityAttributeName}: ${newDisc.entityAttributeType}`,
                specializationId: "structure-dto-field",
                elementId: newDisc.entityAttributeId,
                elementType: "NEW-Field",
                originalName: newDisc.entityAttributeName,
                originalType: newDisc.entityAttributeType,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: newDisc.icon,
                discrepancy: newDisc,
                children: []
            };
            node.children.push(newNode);
        }
        // Recursively process children
        if (node.children) {
            for (const child of node.children) {
                this.addNewDiscrepancyNodes(child, discrepancies);
            }
        }
    }
    annotateNodeRecursive(node, elementIdsWithDiscrepancies, discrepancyByElementId) {
        let nodeHasDiscrepancies = elementIdsWithDiscrepancies.has(node.elementId);
        // Attach discrepancy object if this node has one
        if (nodeHasDiscrepancies && discrepancyByElementId.has(node.elementId)) {
            node.discrepancy = discrepancyByElementId.get(node.elementId);
        }
        // Check children
        if (node.children && node.children.length > 0) {
            let anyChildHasDiscrepancies = false;
            for (const child of node.children) {
                const childHasDiscrepancies = this.annotateNodeRecursive(child, elementIdsWithDiscrepancies, discrepancyByElementId);
                anyChildHasDiscrepancies = anyChildHasDiscrepancies || childHasDiscrepancies;
            }
            nodeHasDiscrepancies = nodeHasDiscrepancies || anyChildHasDiscrepancies;
        }
        // Mark this node with discrepancy info
        node.hasDiscrepancies = nodeHasDiscrepancies;
        return nodeHasDiscrepancies;
    }
    /**
     * Prune branches from tree that don't have discrepancies
     */
    pruneTreeWithoutDiscrepancies(node) {
        if (!node.children) {
            return;
        }
        // Filter children - keep only those with discrepancies (and root/operation-param)
        node.children = node.children.filter((child) => {
            const extChild = child;
            const keep = extChild.hasDiscrepancies ||
                extChild.elementType === "Operation-Parameter" ||
                extChild.elementType === "DTO"; // Keep root
            if (keep && extChild.children) {
                this.pruneTreeWithoutDiscrepancies(extChild);
            }
            return keep;
        });
    }
    /**
     * Convert the pruned tree structure to display nodes
     */
    buildHierarchicalTreeNodes(sourceElement, dtoElement, discrepancies) {
        // Use the pruned tree structure that was built and annotated
        if (!this.lastStructureTree) {
            return [];
        }
        // Convert the tree to display nodes
        return this.convertTreeToDisplayNodes(this.lastStructureTree);
    }
    /**
     * Recursively convert tree nodes to display nodes
     */
    convertTreeToDisplayNodes(node) {
        const result = [];
        // Process this node's children (skip root itself)
        if (node.children) {
            for (const child of node.children) {
                const extChild = child;
                const displayNode = this.createDisplayNode(extChild);
                result.push(displayNode);
            }
        }
        return result;
    }
    /**
     * Create a display node from a tree node
     */
    createDisplayNode(node) {
        const displayNode = {
            id: node.id,
            label: node.label,
            specializationId: node.specializationId,
            isExpanded: true,
            isSelected: false,
            icon: node.icon,
            children: []
        };
        // If this node has a discrepancy, attach the display function
        if (node.discrepancy) {
            const discrepancy = node.discrepancy;
            // For NEW items, always use entityAttributeName; for others use dtoFieldName
            const fieldName = discrepancy.type === "NEW"
                ? discrepancy.entityAttributeName
                : (discrepancy.dtoFieldName || discrepancy.entityAttributeName || "Unknown");
            displayNode.displayFunction = createDiscrepancyDisplayFunction(discrepancy, fieldName);
            displayNode.specializationId = `discrepancy-${discrepancy.type.toLowerCase()}`;
            displayNode.label = this.formatDiscrepancyLabel(discrepancy);
        }
        // Recursively convert children
        if (node.children && node.children.length > 0) {
            displayNode.children = [];
            for (const child of node.children) {
                const extChild = child;
                const childDisplayNode = this.createDisplayNode(extChild);
                displayNode.children.push(childDisplayNode);
            }
        }
        return displayNode;
    }
    formatDiscrepancyLabel(discrepancy) {
        switch (discrepancy.type) {
            case "DELETE":
                return `[DELETE] ${discrepancy.dtoFieldName}: ${discrepancy.dtoFieldType}`;
            case "NEW":
                return `[NEW] ${discrepancy.entityAttributeName}: ${discrepancy.entityAttributeType}`;
            case "RENAME":
                return `[RENAME] ${discrepancy.dtoFieldName} → ${discrepancy.entityAttributeName}`;
            case "CHANGE_TYPE":
                return `[CHANGE_TYPE] ${discrepancy.dtoFieldName}: ${discrepancy.dtoFieldType} → ${discrepancy.entityAttributeType}`;
            default:
                return discrepancy.dtoFieldName || discrepancy.entityAttributeName || "Unknown";
        }
    }
    /**
     * Apply sync actions (placeholder for later)
     */
    applySyncActions(dtoElement, entity, selectedDiscrepancies, associations) {
        // Not implemented yet
    }
}
/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="sync-dto-fields-engine.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
async function syncDtoFields(element) {
    console.log(`[SYNC] Starting sync for element: ${element.getName()} (${element.specialization})`);
    // Validate element
    if (!isValidSyncElement(element)) {
        throw new Error(`Invalid element type: The selected element must be a DTO, Command, Query, or Service Operation. The current element is a '${element.specialization}'.`);
    }
    // Extract DTO from element (handles both direct DTO/Command/Query and Operation with DTO parameter)
    const dtoElement = extractDtoFromElement(element);
    if (!dtoElement) {
        throw new Error(`Unable to find DTO: For Operations, a DTO parameter must be present. The operation '${element.getName()}' does not have a DTO parameter.`);
    }
    console.log(`[SYNC] ├─ DTO extracted: ${dtoElement.getName()}`);
    // Find action associations - use the original element if it's an Operation, otherwise use the DTO
    const elementToSearchForAssociations = element.specialization === "Operation" ? element : dtoElement;
    const associations = findAssociationsPointingToElement(elementToSearchForAssociations, dtoElement);
    // Try to get entity from associations
    let entity = getEntityFromAssociations(associations);
    // If no associations found, throw error
    if (!entity) {
        throw new Error(`No entity mappings found: The '${dtoElement.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`);
    }
    console.log(`[SYNC] ├─ Entity found: ${entity.getName()}`);
    console.log(`[SYNC] ├─ Associations: ${associations.length}`);
    // Extract field mappings from associations
    const fieldMappings = extractFieldMappings(associations);
    console.log(`[SYNC] ├─ Field mappings: ${fieldMappings.length}`);
    // Analyze discrepancies using new structure-first approach
    const engine = new FieldSyncEngine();
    const discrepancies = engine.analyzeFieldDiscrepancies(dtoElement, entity, fieldMappings, new Set(), element);
    console.log(`[SYNC] ├─ Discrepancies found: ${discrepancies.length}`);
    // Build tree view model with hierarchical structure
    const treeNodes = engine.buildHierarchicalTreeNodes(element, dtoElement, discrepancies);
    console.log(`[SYNC] ├─ Tree nodes built: ${treeNodes.length}`);
    // Present dialog with results
    const selectedNodeIds = await presentSyncDialog(element, dtoElement, entity, discrepancies, treeNodes);
    console.log(`[SYNC] ├─ Selected nodes: ${selectedNodeIds.length}`);
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        // Filter to only actual discrepancy IDs
        const discrepancyIds = new Set(discrepancies.map(d => d.id));
        const selectedDiscrepancies = discrepancies.filter(d => selectedNodeIds.includes(d.id) && discrepancyIds.has(d.id));
        console.log(`[SYNC] └─ Applying ${selectedDiscrepancies.length} sync actions`);
        engine.applySyncActions(dtoElement, entity, selectedDiscrepancies, associations);
    }
    else {
        console.log(`[SYNC] └─ No discrepancies selected`);
    }
}
async function presentSyncDialog(sourceElement, dtoElement, entity, discrepancies, treeNodes) {
    // Determine root display based on source element type
    // If it's an Operation, show the Operation; otherwise show the DTO
    const rootElement = sourceElement.specialization === "Operation" ? sourceElement : dtoElement;
    const rootLabel = sourceElement.specialization === "Operation"
        ? `${sourceElement.getName()} (${sourceElement.specialization})`
        : dtoElement.getName();
    console.log(`[DIALOG] sourceElement: ${JSON.stringify(sourceElement)}`);
    console.log(`[DIALOG] treeNodes: ${JSON.stringify(treeNodes.map(x => {
        return {
            id: x.id,
            label: x.label,
            childrenCount: x.children ? x.children.length : 0,
            isExpanded: x.isExpanded,
            isSelected: x.isSelected,
            specializationId: x.specializationId
        };
    }))}`);
    const config = {
        title: `Synchronize ${sourceElement.getName()} with ${entity.getName()}`,
        icon: "fa-sync",
        helpText: "Select the field discrepancies you want to synchronize. The utility will create missing DTO fields, remove orphaned fields, and update mappings to match the target entity structure.",
        submitButtonText: "Done",
        minWidth: "800px",
        height: "600px",
        fields: [
            {
                id: "discrepancies",
                fieldType: "tree-view",
                label: "Field Discrepancies",
                isRequired: false,
                isHidden: false,
                treeViewOptions: {
                    rootNode: {
                        id: "root",
                        label: rootLabel,
                        specializationId: "dto-sync-root",
                        children: treeNodes,
                        isExpanded: true,
                        isSelected: false,
                        icon: rootElement.getIcon()
                    },
                    height: "400px",
                    isMultiSelect: true,
                    selectableTypes: [
                        {
                            specializationId: "dto-sync-root",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "structure-operation-param",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "structure-dto-field",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "structure-nested-field",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "discrepancy-delete",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "discrepancy-new",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "discrepancy-rename",
                            isSelectable: true,
                            autoExpand: true
                        },
                        {
                            specializationId: "discrepancy-change_type",
                            isSelectable: true,
                            autoExpand: true
                        }
                    ]
                }
            }
        ]
    };
    const result = await dialogService.openForm(config);
    if (!result) {
        return [];
    }
    // Tree-view returns selected IDs as a comma-separated string (single) or array (multiple)
    const selectedValue = result.discrepancies;
    if (!selectedValue) {
        return [];
    }
    // Handle both string and array formats
    let selectedIds = [];
    if (typeof selectedValue === 'string') {
        // Single selection or comma-separated string
        selectedIds = selectedValue.split(',').map(id => id.trim()).filter(id => id.length > 0);
    }
    else if (Array.isArray(selectedValue)) {
        // Multiple selections as array
        selectedIds = selectedValue;
    }
    return selectedIds;
}
