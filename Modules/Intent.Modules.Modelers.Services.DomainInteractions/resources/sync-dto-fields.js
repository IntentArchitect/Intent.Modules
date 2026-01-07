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
        return association.typeReference.getType();
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
        }
        catch (error) {
            // Association may not have advanced mappings configured
            continue;
        }
    }
    return mappings;
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
    // Analyze the first non-invocation mapping to determine path structure
    const dataMappings = mappings.filter(m => m.mappingType !== "Invocation Mapping" &&
        m.sourcePath.length > 0 &&
        m.targetPath.length > 0);
    if (dataMappings.length === 0)
        return null;
    const sampleMapping = dataMappings[0];
    console.log(`[analyzePathStructure] Analyzing path structure:`);
    console.log(`  Source path IDs length: ${sampleMapping.sourcePath.length}`);
    console.log(`  Target path IDs length: ${sampleMapping.targetPath.length}`);
    // Build element type arrays by looking up each element in the path
    const sourceElementTypes = [];
    const targetElementTypes = [];
    for (const elementId of sampleMapping.sourcePath) {
        try {
            const element = lookup(elementId);
            sourceElementTypes.push(element.specialization);
            console.log(`    Source: ${element.getName()} (${element.specialization})`);
        }
        catch {
            sourceElementTypes.push("Unknown");
        }
    }
    for (const elementId of sampleMapping.targetPath) {
        try {
            const element = lookup(elementId);
            targetElementTypes.push(element.specialization);
            console.log(`    Target: ${element.getName()} (${element.specialization})`);
        }
        catch {
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
function buildPathUsingTemplate(template, newFieldId, targetAttributeId, sourceRoot, targetRoot, dtoElement) {
    const sourcePath = [];
    const targetPath = [];
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
    }
    else if (template.sourceDepth === 2) {
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
            }
            else {
                // Fallback
                sourcePath.push(sourceRoot.id, newFieldId);
                console.log(`    ✗ Fallback to Operation`);
            }
        }
        else {
            // For DTOs or other elements, use sourceRoot
            sourcePath.push(sourceRoot.id, newFieldId);
            console.log(`  Source depth 2 case`);
        }
    }
    else if (template.sourceDepth === 3) {
        // Three levels: Need to find intermediate element
        // For Operations with DTO parameters: [operationId, parameterId, fieldId]
        console.log(`  Source depth 3 case`);
        console.log(`    Looking for intermediate element of type: ${template.sourceElementTypes[1]}`);
        const intermediateElement = findIntermediateElement(sourceRoot, template.sourceElementTypes[1], dtoElement);
        if (intermediateElement) {
            sourcePath.push(sourceRoot.id, intermediateElement.id, newFieldId);
            console.log(`    ✓ Found intermediate: ${intermediateElement.getName()}`);
        }
        else {
            // Fallback to simpler path
            sourcePath.push(sourceRoot.id, newFieldId);
            console.log(`    ✗ Could not find intermediate, using fallback path`);
        }
    }
    else {
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
    }
    else if (template.targetDepth === 2) {
        // Two levels: [entityId, attributeId]
        targetPath.push(targetRoot.id, targetAttributeId);
        console.log(`  Target depth 2 case`);
    }
    else {
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
        }
        catch {
            return id.substring(0, 8) + "...";
        }
    }).join(" → ")}]`);
    console.log(`  Final target path: [${targetPath.map((id, i) => {
        try {
            const el = lookup(id);
            return `${el.getName()}(${el.specialization})`;
        }
        catch {
            return id.substring(0, 8) + "...";
        }
    }).join(" → ")}]`);
    return { sourcePath, targetPath };
}
function findIntermediateElement(parent, expectedType, dtoElement) {
    var _a;
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
            }
            else {
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
            const target = (_a = assoc.typeReference) === null || _a === void 0 ? void 0 : _a.getType();
            if (target && target.specialization === expectedType) {
                console.log(`  ✓ Found via association: ${target.getName()}`);
                return target;
            }
        }
        catch {
            continue;
        }
    }
    console.log(`  ✗ No intermediate element found`);
    return null;
}
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
// Discrepancy status colors
const DISCREPANCY_COLORS = {
    NEW: "#22c55e", // Green
    DELETED: "#ef4444", // Red
    RENAMED: "#007777", // Teal
    TYPE_CHANGED: "#f97316" // Orange
};
const DISCREPANCY_LABELS = {
    NEW: "[NEW]",
    DELETED: "[DELETED]",
    RENAMED: "[RENAMED]",
    TYPE_CHANGED: "[TYPE CHANGED]"
};
function formatDiscrepancy(discrepancy) {
    const components = [];
    const statusInfo = getDiscrepancyStatusInfo(discrepancy.type);
    switch (discrepancy.type) {
        case "NEW":
            // EntityName: type [NEW]
            components.push({ text: discrepancy.entityAttributeName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.NEW, color: statusInfo.color });
            break;
        case "RENAMED":
            // CurrentName → EntityName: type [RENAMED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.RENAMED, color: statusInfo.color });
            break;
        case "TYPE_CHANGED":
            // FieldName: currentType → entityType [TYPE CHANGED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.TYPE_CHANGED, color: statusInfo.color });
            break;
        case "DELETED":
            // FieldName: type [DELETED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.DELETED, color: statusInfo.color });
            break;
    }
    return components;
}
function getDiscrepancyStatusInfo(type) {
    switch (type) {
        case "NEW":
            return { color: DISCREPANCY_COLORS.NEW, cssClass: "keyword" };
        case "DELETED":
            return { color: DISCREPANCY_COLORS.DELETED, cssClass: "typeref" };
        case "RENAMED":
            return { color: DISCREPANCY_COLORS.RENAMED, cssClass: "annotation" };
        case "TYPE_CHANGED":
            return { color: DISCREPANCY_COLORS.TYPE_CHANGED, cssClass: "muted" };
        default:
            return { color: "#6b7280", cssClass: "" }; // Gray fallback
    }
}
function createDiscrepancyDisplayFunction(discrepancy) {
    return () => formatDiscrepancy(discrepancy);
}
/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="display-formatter.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
class FieldSyncEngine {
    analyzeFieldDiscrepancies(dtoElement, entity, mappings) {
        const discrepancies = [];
        const dtoFields = getDtoFields(dtoElement);
        const entityAttributes = getEntityAttributes(entity);
        // Create a map of mapped DTO fields -> Entity attributes
        const mappedDtoToEntity = new Map();
        const mappedEntityToDto = new Map();
        // SIDE EFFECT: This loop mutates dtoFields elements by setting isMapped and mappedToAttributeId
        // This is intentional for performance to avoid creating new objects
        for (const mapping of mappings) {
            // Only consider field-level mappings (ignore invocation mappings to constructors)
            const sourceField = dtoFields.find(f => f.id === mapping.sourceFieldId);
            const targetAttr = entityAttributes.find(a => a.id === mapping.targetAttributeId);
            if (sourceField && targetAttr) {
                mappedDtoToEntity.set(mapping.sourceFieldId, mapping.targetAttributeId);
                mappedEntityToDto.set(mapping.targetAttributeId, mapping.sourceFieldId);
                sourceField.isMapped = true;
                sourceField.mappedToAttributeId = mapping.targetAttributeId;
            }
        }
        // Check for DELETED fields (DTO fields not mapped to any entity attribute)
        for (const dtoField of dtoFields) {
            if (!dtoField.isMapped) {
                const discrepancy = {
                    id: `deleted-${dtoField.id}`,
                    type: "DELETED",
                    dtoFieldId: dtoField.id,
                    dtoFieldName: dtoField.name,
                    dtoFieldType: dtoField.typeDisplayText,
                    entityAttributeName: "N/A",
                    icon: dtoField.icon,
                    reason: "Field exists in DTO but has no entity mapping"
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            }
        }
        // Check for NEW, RENAMED, and TYPE_CHANGED
        for (const entityAttr of entityAttributes) {
            const mappedDtoFieldId = mappedEntityToDto.get(entityAttr.id);
            if (!mappedDtoFieldId) {
                // Skip NEW discrepancy for unmapped managed keys (they're auto-generated or should be mapped in the association)
                if (entityAttr.isManagedKey) {
                    continue;
                }
                // NEW: Entity attribute not in DTO
                const discrepancy = {
                    id: `new-${entityAttr.id}`,
                    type: "NEW",
                    dtoFieldId: undefined,
                    dtoFieldName: "[Missing]",
                    entityAttributeId: entityAttr.id,
                    entityAttributeName: entityAttr.name,
                    entityAttributeType: entityAttr.typeDisplayText,
                    reason: "Entity attribute does not exist in DTO",
                    icon: entityAttr.icon
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            }
            else {
                const mappedDtoField = dtoFields.find(f => f.id === mappedDtoFieldId);
                if (mappedDtoField) {
                    // RENAMED check
                    if (mappedDtoField.name !== entityAttr.name) {
                        const discrepancy = {
                            id: `renamed-${mappedDtoField.id}`,
                            type: "RENAMED",
                            dtoFieldId: mappedDtoField.id,
                            dtoFieldName: mappedDtoField.name,
                            entityAttributeId: entityAttr.id,
                            entityAttributeName: entityAttr.name,
                            dtoFieldType: mappedDtoField.typeDisplayText,
                            entityAttributeType: entityAttr.typeDisplayText,
                            reason: "Field name differs from entity attribute name",
                            icon: mappedDtoField.icon
                        };
                        discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                        discrepancies.push(discrepancy);
                    }
                    // TYPE_CHANGED check
                    if (mappedDtoField.typeId !== entityAttr.typeId) {
                        const discrepancy = {
                            id: `type-changed-${mappedDtoField.id}`,
                            type: "TYPE_CHANGED",
                            dtoFieldId: mappedDtoField.id,
                            dtoFieldName: mappedDtoField.name,
                            dtoFieldType: mappedDtoField.typeDisplayText,
                            entityAttributeId: entityAttr.id,
                            entityAttributeName: entityAttr.name,
                            entityAttributeType: entityAttr.typeDisplayText,
                            reason: "Field type differs from entity attribute type",
                            icon: mappedDtoField.icon
                        };
                        discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                        discrepancies.push(discrepancy);
                    }
                }
            }
        }
        return discrepancies;
    }
    buildTreeNodes(discrepancies) {
        const nodes = [];
        for (const discrepancy of discrepancies) {
            // Use the actual field name, not "[Missing]"
            const displayName = discrepancy.type === "NEW"
                ? discrepancy.entityAttributeName
                : discrepancy.dtoFieldName;
            const node = {
                id: discrepancy.id,
                label: displayName,
                specializationId: "sync-field-discrepancy",
                isExpanded: true,
                isSelected: false,
                icon: discrepancy.icon,
                displayFunction: discrepancy.displayFunction,
                displayMetadata: {
                    type: discrepancy.type,
                    dtoFieldName: discrepancy.dtoFieldName,
                    dtoFieldType: discrepancy.dtoFieldType,
                    entityAttributeName: discrepancy.entityAttributeName,
                    entityAttributeType: discrepancy.entityAttributeType,
                    reason: discrepancy.reason
                }
            };
            nodes.push(node);
        }
        return nodes;
    }
    applySyncActions(dtoElement, entity, selectedDiscrepancies, associations) {
        if (selectedDiscrepancies.length === 0) {
            return;
        }
        // Group discrepancies by type for ordered processing
        const deleted = selectedDiscrepancies.filter(d => d.type === "DELETED");
        const renamed = selectedDiscrepancies.filter(d => d.type === "RENAMED");
        const typeChanged = selectedDiscrepancies.filter(d => d.type === "TYPE_CHANGED");
        const newFields = selectedDiscrepancies.filter(d => d.type === "NEW");
        // Process DELETED first (remove fields)
        for (const discrepancy of deleted) {
            this.deleteField(dtoElement, discrepancy);
        }
        // Process RENAMED (update field names)
        for (const discrepancy of renamed) {
            this.renameField(dtoElement, discrepancy);
        }
        // Process TYPE_CHANGED (update type references)
        for (const discrepancy of typeChanged) {
            this.changeFieldType(dtoElement, entity, discrepancy);
        }
        // Process NEW last (create fields and mappings)
        for (const discrepancy of newFields) {
            this.createFieldWithMapping(dtoElement, entity, discrepancy, associations);
        }
    }
    deleteField(dtoElement, discrepancy) {
        if (!discrepancy.dtoFieldId)
            return;
        // Dynamically determine child type
        const childType = inferSourceElementChildType(dtoElement);
        const field = dtoElement.getChildren(childType)
            .find(f => f.id === discrepancy.dtoFieldId);
        if (field) {
            field.delete();
        }
    }
    renameField(dtoElement, discrepancy) {
        if (!discrepancy.dtoFieldId || !discrepancy.entityAttributeName)
            return;
        // Dynamically determine child type
        const childType = inferSourceElementChildType(dtoElement);
        const field = dtoElement.getChildren(childType)
            .find(f => f.id === discrepancy.dtoFieldId);
        if (field) {
            field.setName(discrepancy.entityAttributeName, false);
        }
    }
    changeFieldType(dtoElement, entity, discrepancy) {
        if (!discrepancy.dtoFieldId || !discrepancy.entityAttributeId)
            return;
        // Dynamically determine child types
        const sourceChildType = inferSourceElementChildType(dtoElement);
        const targetChildType = inferTargetElementChildType(entity);
        const field = dtoElement.getChildren(sourceChildType)
            .find(f => f.id === discrepancy.dtoFieldId);
        const attribute = entity.getChildren(targetChildType)
            .find(a => a.id === discrepancy.entityAttributeId);
        if (field && field.typeReference && attribute && attribute.typeReference) {
            field.typeReference.setType(attribute.typeReference.typeId);
            field.typeReference.setIsNullable(attribute.typeReference.isNullable);
            field.typeReference.setIsCollection(attribute.typeReference.isCollection);
        }
    }
    createFieldWithMapping(dtoElement, entity, discrepancy, associations) {
        if (!discrepancy.entityAttributeId || !discrepancy.entityAttributeName)
            return;
        // Dynamically determine target element type (not hard-coded to "Attribute")
        const targetChildType = inferTargetElementChildType(entity);
        const attribute = entity.getChildren(targetChildType)
            .find(a => a.id === discrepancy.entityAttributeId);
        if (!attribute)
            return;
        // Dynamically determine source element type (not hard-coded to "DTO-Field")
        const sourceChildType = inferSourceElementChildType(dtoElement);
        const newField = createElement(sourceChildType, discrepancy.entityAttributeName, dtoElement.id);
        // Set type to match entity attribute
        if (newField.typeReference && attribute.typeReference) {
            newField.typeReference.setType(attribute.typeReference.typeId);
            newField.typeReference.setIsNullable(attribute.typeReference.isNullable);
            newField.typeReference.setIsCollection(attribute.typeReference.isCollection);
        }
        // Create mapping using generic path resolution
        if (associations.length > 0) {
            const association = associations[0];
            try {
                const advancedMappings = association.getAdvancedMappings();
                if (advancedMappings.length > 0) {
                    const targetMapping = advancedMappings[0];
                    if (targetMapping) {
                        // Extract existing mappings to learn the path structure
                        const existingMappings = extractFieldMappings([association]);
                        // Analyze path structure from existing mappings
                        const pathTemplate = analyzePathStructure(existingMappings);
                        let sourcePath;
                        let targetPath;
                        let mappingType = "Data Mapping"; // Default
                        if (pathTemplate) {
                            // Use learned template to build paths generically
                            // Determine sourceRoot from first existing mapping
                            let sourceRoot;
                            let targetRoot;
                            if (existingMappings.length > 0) {
                                // Get the actual roots from the existing mappings
                                const firstMapping = existingMappings[0];
                                sourceRoot = lookup(firstMapping.sourcePath[0]);
                                targetRoot = lookup(firstMapping.targetPath[0]);
                                console.log(`=== Determined Source/Target Roots from Existing Mappings ===`);
                                console.log(`Source root from mapping: ${sourceRoot.getName()} (${sourceRoot.specialization})`);
                                console.log(`Target root from mapping: ${targetRoot.getName()} (${targetRoot.specialization})`);
                            }
                            else {
                                // Fallback to using mapping's source/target elements
                                sourceRoot = targetMapping.getSourceElement();
                                targetRoot = targetMapping.getTargetElement();
                                console.log(`Using targetMapping.getSourceElement/getTargetElement`);
                            }
                            console.log(`=== Path Building Diagnostics ===`);
                            console.log(`Template signature: ${pathTemplate.signature}`);
                            console.log(`Source root: ${sourceRoot.getName()} (${sourceRoot.specialization})`);
                            console.log(`Target root: ${targetRoot.getName()} (${targetRoot.specialization})`);
                            console.log(`DTO element: ${dtoElement === null || dtoElement === void 0 ? void 0 : dtoElement.getName()} (${dtoElement === null || dtoElement === void 0 ? void 0 : dtoElement.specialization})`);
                            console.log(`Template source depth: ${pathTemplate.sourceDepth}, element types: ${pathTemplate.sourceElementTypes.join(" > ")}`);
                            const paths = buildPathUsingTemplate(pathTemplate, newField.id, attribute.id, sourceRoot, targetRoot, dtoElement // Pass dtoElement for Operations with Parameters
                            );
                            sourcePath = paths.sourcePath;
                            targetPath = paths.targetPath;
                            mappingType = pathTemplate.mappingType;
                            console.log(`Built source path: [${sourcePath.map((id, i) => {
                                try {
                                    return `${lookup(id).getName()}(${lookup(id).specialization})`;
                                }
                                catch {
                                    return id;
                                }
                            }).join(", ")}]`);
                            console.log(`Built target path: [${targetPath.map((id, i) => {
                                try {
                                    return `${lookup(id).getName()}(${lookup(id).specialization})`;
                                }
                                catch {
                                    return id;
                                }
                            }).join(", ")}]`);
                        }
                        else {
                            // Fallback: infer from source element type
                            const sourceElement = targetMapping.getSourceElement();
                            if (sourceElement.specialization === "Operation") {
                                // For Operations: try to find parameter containing DTO
                                const dtoParameter = this.findDtoParameter(sourceElement, dtoElement);
                                if (dtoParameter) {
                                    sourcePath = [sourceElement.id, dtoParameter.id, newField.id];
                                    targetPath = [entity.id, attribute.id];
                                }
                                else {
                                    // Fallback: simpler path
                                    sourcePath = [sourceElement.id, newField.id];
                                    targetPath = [entity.id, attribute.id];
                                }
                            }
                            else if (sourceElement.id === dtoElement.id) {
                                // Direct DTO/Command/Query: simple path
                                sourcePath = [newField.id];
                                targetPath = [attribute.id];
                            }
                            else {
                                // Parent element: two-level path
                                sourcePath = [dtoElement.id, newField.id];
                                targetPath = [entity.id, attribute.id];
                            }
                        }
                        targetMapping.addMappedEnd(mappingType, sourcePath, targetPath);
                    }
                }
            }
            catch (error) {
                console.warn("Failed to create advanced mapping:", error);
                // Fallback: try simple mapping
                try {
                    newField.setMapping(attribute.id);
                }
                catch (fallbackError) {
                    console.warn("Fallback mapping also failed:", fallbackError);
                }
            }
        }
    }
    findDtoParameter(operation, dtoElement) {
        const parameters = operation.getChildren(ELEMENT_TYPE_NAMES.PARAMETER);
        for (const param of parameters) {
            if (param.typeReference && param.typeReference.typeId === dtoElement.id) {
                return param;
            }
        }
        return null;
    }
}
/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="sync-dto-fields-engine.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
async function syncDtoFields(element) {
    // Validate element
    if (!isValidSyncElement(element)) {
        await dialogService.error(`Invalid element type.\n\nThe selected element must be a DTO, Command, Query, or Service Operation. The current element is a '${element.specialization}'.`);
        return;
    }
    // Extract DTO from element (handles both direct DTO/Command/Query and Operation with DTO parameter)
    const dtoElement = extractDtoFromElement(element);
    if (!dtoElement) {
        await dialogService.error(`Unable to find DTO.\n\nFor Operations, a DTO parameter must be present. The operation '${element.getName()}' does not have a DTO parameter.`);
        return;
    }
    // Find action associations - use the original element if it's an Operation, otherwise use the DTO
    const elementToSearchForAssociations = element.specialization === "Operation" ? element : dtoElement;
    const associations = findAssociationsPointingToElement(elementToSearchForAssociations, dtoElement);
    // Try to get entity from associations
    let entity = getEntityFromAssociations(associations);
    // If no associations found, ask user to select entity
    if (!entity) {
        await dialogService.warn(`No entity mappings found.\n\nThe '${dtoElement.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`);
        return;
    }
    // Extract field mappings from associations (not from DTO fields directly)
    const fieldMappings = extractFieldMappings(associations);
    // Analyze discrepancies
    const engine = new FieldSyncEngine();
    const discrepancies = engine.analyzeFieldDiscrepancies(dtoElement, entity, fieldMappings);
    if (discrepancies.length === 0) {
        await dialogService.info(`All fields are synchronized.\n\nThe DTO fields are properly synchronized with the entity '${entity.getName()}' attributes.`);
        return;
    }
    // Build tree view model
    const treeNodes = engine.buildTreeNodes(discrepancies);
    // Present dialog with results
    const selectedNodeIds = await presentSyncDialog(dtoElement, entity, discrepancies, treeNodes);
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        const selectedDiscrepancies = discrepancies.filter(d => selectedNodeIds.includes(d.id));
        engine.applySyncActions(dtoElement, entity, selectedDiscrepancies, associations);
        await dialogService.info(`Synchronization complete.\n\n${selectedDiscrepancies.length} field(s) synchronized successfully.`);
    }
}
async function presentSyncDialog(dtoElement, entity, discrepancies, treeNodes) {
    const config = {
        title: "Synchronize DTO Fields",
        icon: "fa-sync",
        submitButtonText: "Done",
        minWidth: "600px",
        maxWidth: "800px",
        height: "600px",
        fields: [
            {
                id: "info",
                fieldType: "alert",
                label: "",
                isHidden: false,
                hint: `Comparing DTO '${dtoElement.getName()}' with entity '${entity.getName()}' - Found ${discrepancies.length} field discrepancy(ies).`,
                hintType: "info"
            },
            {
                id: "discrepancies",
                fieldType: "tree-view",
                label: "Field Discrepancies",
                isRequired: false,
                isHidden: false,
                treeViewOptions: {
                    rootNode: {
                        id: "root",
                        label: dtoElement.getName(),
                        specializationId: "dto-sync-root",
                        children: treeNodes,
                        isExpanded: true,
                        isSelected: false,
                        icon: dtoElement.getIcon()
                    },
                    height: "400px",
                    isMultiSelect: true,
                    selectableTypes: [
                        {
                            specializationId: "sync-field-discrepancy",
                            isSelectable: true
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
