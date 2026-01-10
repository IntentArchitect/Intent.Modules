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
/**
 * Normalize a name for comparison purposes by converting to lowercase.
 * This allows comparing camelCase, PascalCase, and other conventions uniformly.
 * For example: "id", "Id", "ID" all normalize to "id"
 *
 * @param name The name to normalize
 * @returns The normalized name (lowercase)
 */
function normalizeNameForComparison(name) {
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
function namesAreEquivalent(name1, name2) {
    return normalizeNameForComparison(name1) === normalizeNameForComparison(name2);
}
function isValidSyncElement(element) {
    return VALID_SPECIALIZATIONS.includes(element.specialization);
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
            isManagedKey: child.hasMetadata(METADATA_KEYS.IS_MANAGED_KEY) && child.getMetadata(METADATA_KEYS.IS_MANAGED_KEY) === "true",
            hasPrimaryKeyStereotype: child.hasStereotype && child.hasStereotype("Primary Key")
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
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
/**
 * Centralized tree node label building
 */
class TreeNodeLabelBuilder {
    static buildFieldLabel(name, type) {
        return `${name}: ${type}`;
    }
    static buildDiscrepancyLabel(discrepancy) {
        switch (discrepancy.type) {
            case "DELETE":
                return `[DELETE] ${discrepancy.sourceFieldName}: ${discrepancy.sourceFieldTypeName}`;
            case "NEW":
                return `[NEW] ${discrepancy.targetAttributeName}: ${discrepancy.targetAttributeTypeName}`;
            case "RENAME":
                return `[RENAME] ${discrepancy.sourceFieldName} → ${discrepancy.targetAttributeName}`;
            case "CHANGE_TYPE":
                return `[CHANGE_TYPE] ${discrepancy.sourceFieldName}: ${discrepancy.sourceFieldTypeName} → ${discrepancy.targetAttributeTypeName}`;
            default:
                return discrepancy.sourceFieldName || discrepancy.targetAttributeName || "Unknown";
        }
    }
}
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
            const newTypeDisplay = buildTypeDisplay(discrepancy.targetAttributeTypeName || "", discrepancy.targetIsCollection || false, discrepancy.targetIsNullable || false);
            components.push({ text: newTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.NEW, color: statusInfo.color });
            break;
        case "RENAME":
            // cleanFieldName → targetName: type [RENAME]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            const targetName = discrepancy.targetAttributeName.split('.').pop() || discrepancy.targetAttributeName;
            components.push({ text: targetName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const renameTypeDisplay = buildTypeDisplay(discrepancy.sourceFieldTypeName || "", discrepancy.sourceIsCollection || false, discrepancy.sourceIsNullable || false);
            components.push({ text: renameTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.RENAME, color: statusInfo.color });
            break;
        case "CHANGE_TYPE":
            // cleanFieldName: oldType → newType [CHANGE TYPE]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const oldTypeDisplay = buildTypeDisplay(discrepancy.sourceFieldTypeName || "", discrepancy.sourceIsCollection || false, discrepancy.sourceIsNullable || false);
            components.push({ text: oldTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            const newTypeChangeDisplay = buildTypeDisplay(discrepancy.targetAttributeTypeName || "", discrepancy.targetIsCollection || false, discrepancy.targetIsNullable || false);
            components.push({ text: newTypeChangeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.CHANGE_TYPE, color: statusInfo.color });
            break;
        case "DELETE":
            // cleanFieldName: type [DELETE]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const deleteTypeDisplay = buildTypeDisplay(discrepancy.sourceFieldTypeName || "", discrepancy.sourceIsCollection || false, discrepancy.sourceIsNullable || false);
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
    const fieldName = cleanFieldName || discrepancy.sourceFieldName || discrepancy.targetAttributeName;
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
/// <reference path="../../typings/designer-common.api.d.ts" />
/**
 * Executes sync actions on selected tree nodes
 * Applies RENAME, DELETE, CHANGE_TYPE, and NEW field/association operations
 */
class NodeSyncExecutor {
    execute(selectedNodes, dtoElement, entity, associations) {
        console.log(`[APPLY-SYNC] Starting to apply ${selectedNodes.length} sync actions`);
        for (const node of selectedNodes) {
            if (!node.discrepancy) {
                console.log(`[APPLY-SYNC] ⚠ Skipping node ${node.id} - no discrepancy metadata`);
                continue;
            }
            try {
                this.executeNode(node, dtoElement, entity, associations);
                console.log(`[APPLY-SYNC] ✓ Applied [${node.discrepancy.type}] ${node.discrepancy.sourceFieldName}`);
            }
            catch (error) {
                console.log(`[APPLY-SYNC] ✗ Failed [${node.discrepancy.type}] ${node.discrepancy.sourceFieldName}: ${error}`);
                // Continue to next node (no transactions)
            }
        }
        console.log(`[APPLY-SYNC] └─ Sync complete`);
    }
    executeNode(node, dtoElement, entity, associations) {
        const discrepancy = node.discrepancy;
        switch (discrepancy.type) {
            case "RENAME":
                this.applyRename(node, dtoElement);
                break;
            case "DELETE":
                this.applyDelete(node, dtoElement, associations);
                break;
            case "CHANGE_TYPE":
                this.applyChangeType(node, dtoElement);
                break;
            case "NEW":
                // Determine if it's an association or attribute based on DTO field type
                if (discrepancy.sourceFieldTypeName && discrepancy.sourceFieldTypeName.includes("Dto")) {
                    this.applyNewAssociation(node, dtoElement, entity, associations);
                }
                else {
                    this.applyNewAttribute(node, dtoElement, entity, associations);
                }
                break;
            default:
                throw new Error(`Unknown discrepancy type: ${discrepancy.type}`);
        }
    }
    /**
     * Apply RENAME: Rename the DTO field to match entity attribute name
     */
    applyRename(node, dtoElement) {
        const discrepancy = node.discrepancy;
        const children = dtoElement.getChildren("DTO-Field");
        const dtoField = children.find(child => child.id === discrepancy.sourceFieldId);
        if (!dtoField) {
            throw new Error(`DTO field not found: ${discrepancy.sourceFieldId}`);
        }
        // Rename the field to match entity attribute name
        dtoField.setName(discrepancy.targetAttributeName, true);
        console.log(`[APPLY-SYNC-RENAME] Renamed field from '${discrepancy.sourceFieldName}' to '${discrepancy.targetAttributeName}'`);
    }
    /**
     * Apply DELETE: Remove the DTO field if it has no mappings
     */
    applyDelete(node, dtoElement, associations) {
        const discrepancy = node.discrepancy;
        const children = dtoElement.getChildren("DTO-Field");
        const dtoField = children.find(child => child.id === discrepancy.sourceFieldId);
        if (!dtoField) {
            throw new Error(`DTO field not found: ${discrepancy.sourceFieldId}`);
        }
        // Delete the field from the DTO
        dtoField.delete();
        console.log(`[APPLY-SYNC-DELETE] Deleted field '${discrepancy.sourceFieldName}'`);
    }
    /**
     * Apply CHANGE_TYPE: Update the field's type reference to match entity attribute type
     */
    applyChangeType(node, dtoElement) {
        const discrepancy = node.discrepancy;
        const children = dtoElement.getChildren("DTO-Field");
        const dtoField = children.find(child => child.id === discrepancy.sourceFieldId);
        if (!dtoField) {
            throw new Error(`DTO field not found: ${discrepancy.sourceFieldId}`);
        }
        const typeRef = dtoField.typeReference;
        if (!typeRef) {
            throw new Error(`Field has no type reference: ${discrepancy.sourceFieldName}`);
        }
        // Update type to match entity attribute
        if (discrepancy.targetTypeId) {
            typeRef.setType(discrepancy.targetTypeId);
        }
        // Update collection and nullable modifiers if they differ
        if (discrepancy.targetIsCollection !== undefined) {
            typeRef.setIsCollection(discrepancy.targetIsCollection);
        }
        if (discrepancy.targetIsNullable !== undefined) {
            typeRef.setIsNullable(discrepancy.targetIsNullable);
        }
        console.log(`[APPLY-SYNC-CHANGE-TYPE] Updated type for '${discrepancy.sourceFieldName}' to '${discrepancy.targetAttributeTypeName}'`);
    }
    /**
     * Apply NEW ATTRIBUTE: Create a new DTO field with the entity attribute's name and type
     */
    applyNewAttribute(node, dtoElement, entity, associations) {
        const discrepancy = node.discrepancy;
        // Create new field in the DTO
        const newField = dtoElement.addChild("DTO-Field", discrepancy.targetAttributeName);
        if (!newField) {
            throw new Error(`Failed to create new DTO field`);
        }
        // Set the type reference if we have the entity attribute's type ID
        if (discrepancy.targetTypeId && newField.typeReference) {
            newField.typeReference.setType(discrepancy.targetTypeId);
            // Set collection/nullable based on entity attribute
            if (discrepancy.targetIsCollection !== undefined) {
                newField.typeReference.setIsCollection(discrepancy.targetIsCollection);
            }
            if (discrepancy.targetIsNullable !== undefined) {
                newField.typeReference.setIsNullable(discrepancy.targetIsNullable);
            }
        }
        console.log(`[APPLY-SYNC-NEW-ATTR] Created new field '${discrepancy.targetAttributeName}: ${discrepancy.targetAttributeTypeName}'`);
        // Create mapping between the new field and the entity attribute
        try {
            this.createFieldMapping(dtoElement, entity, newField, discrepancy);
            console.log(`[APPLY-SYNC-NEW-ATTR] Created mapping for '${discrepancy.targetAttributeName}'`);
        }
        catch (error) {
            console.log(`[APPLY-SYNC-NEW-ATTR] Warning: Failed to create mapping: ${error}`);
            // Don't throw - field was created successfully even if mapping failed
        }
    }
    /**
     * Create a field-to-attribute mapping
     */
    createFieldMapping(dtoElement, entity, newField, discrepancy) {
        var _a, _b, _c;
        // Find or create the association between DTO and Entity
        const associationType = this.inferAssociationType(dtoElement);
        let association = dtoElement.getAssociations(associationType)
            .find(a => a.typeReference && a.typeReference.typeId === entity.id);
        if (!association) {
            // Create the association
            association = this.createElementAssociation(dtoElement.id, entity.id, associationType);
            if (!association) {
                throw new Error(`Failed to create association between ${dtoElement.getName()} and ${entity.getName()}`);
            }
            console.log(`[APPLY-SYNC-NEW-ATTR] Created ${associationType} association`);
        }
        // Create or get the advanced mapping
        const assocApi = association;
        let mapping = (_b = (_a = assocApi.getAdvancedMappings) === null || _a === void 0 ? void 0 : _a.call(assocApi)) === null || _b === void 0 ? void 0 : _b[0];
        if (!mapping) {
            // Try to create advanced mapping
            if (typeof assocApi.createAdvancedMapping === "function") {
                mapping = (_c = assocApi.createAdvancedMapping) === null || _c === void 0 ? void 0 : _c.call(assocApi);
                if (!mapping) {
                    throw new Error(`Failed to create advanced mapping`);
                }
                console.log(`[APPLY-SYNC-NEW-ATTR] Created advanced mapping`);
            }
            else {
                throw new Error(`Association does not support advanced mappings`);
            }
        }
        // Add the field-to-attribute mapping
        if (mapping && typeof mapping.addMappedEnd === "function") {
            const entityAttr = entity.getChildren("Attribute")
                .find(a => a.id === discrepancy.targetAttributeId);
            if (entityAttr) {
                mapping.addMappedEnd("Data Mapping", [dtoElement.id, newField.id], [entity.id, entityAttr.id]);
                console.log(`[APPLY-SYNC-NEW-ATTR] Added data mapping: ${dtoElement.getName()}.${newField.getName()} → ${entity.getName()}.${entityAttr.getName()}`);
            }
        }
    }
    /**
     * Infer the correct association type based on DTO element type
     */
    inferAssociationType(dtoElement) {
        const spec = dtoElement.specialization;
        if (spec === "Command") {
            return "Create Entity Action";
        }
        else if (spec === "Query") {
            return "Query Entity Action";
        }
        // Default to the generic association type
        return "Association";
    }
    /**
     * Create an association between two elements
     */
    createElementAssociation(sourceId, targetId, associationType) {
        try {
            // Use createAssociation function if available in macro context
            // This is typically available in the macro execution environment
            const createAssoc = (typeof createAssociation !== "undefined") ? createAssociation : undefined;
            if (createAssoc) {
                return createAssoc(associationType, sourceId, targetId);
            }
            return undefined;
        }
        catch (error) {
            console.log(`[APPLY-SYNC-NEW-ATTR] Warning: Could not create association: ${error}`);
            return undefined;
        }
    }
    /**
     * Apply NEW ASSOCIATION: Create a new nested DTO and add mapping
     */
    applyNewAssociation(node, dtoElement, entity, associations) {
        const discrepancy = node.discrepancy;
        // Extract the expected DTO name from dtoFieldType (e.g., "CreateBlock1Level1CommandNewBlockForBlock1Dto[*]")
        const dtoTypeName = discrepancy.sourceFieldTypeName.replace(/[\[\]\*]/g, "").trim();
        const isCollection = discrepancy.sourceFieldTypeName.includes("[*]");
        // Create new nested DTO with the suggested name
        const parentPackage = dtoElement.getParent();
        if (!parentPackage) {
            throw new Error(`Cannot determine parent package for new DTO`);
        }
        const newNestedDto = parentPackage.addChild("DTO", dtoTypeName);
        if (!newNestedDto) {
            throw new Error(`Failed to create new nested DTO`);
        }
        console.log(`[APPLY-SYNC-NEW-ASSOC] Created new DTO '${dtoTypeName}'`);
        // Create a reference field in the source DTO pointing to this new DTO
        const newField = dtoElement.addChild("DTO-Field", discrepancy.targetAttributeName);
        if (!newField) {
            throw new Error(`Failed to create new DTO field for association`);
        }
        // Set the type reference to the new nested DTO
        if (newField.typeReference) {
            newField.typeReference.setType(newNestedDto.id);
            newField.typeReference.setIsCollection(isCollection);
            newField.typeReference.setIsNullable(false);
        }
        console.log(`[APPLY-SYNC-NEW-ASSOC] Created reference field '${discrepancy.targetAttributeName}: ${dtoTypeName}${isCollection ? "[*]" : ""}'`);
        // TODO: Create mappings between the new nested DTO and the target entity
        // This would require access to the mapping API which may need additional context
        console.log(`[APPLY-SYNC-NEW-ASSOC] ⚠ Note: Mappings should be created separately for the new association`);
    }
}
/**
 * Structure-first DTO field synchronization engine
 */
class FieldSyncEngine {
    constructor() {
        this.lastStructureTree = null;
        this.entityMapCache = new Map();
    }
    /**
     * Main entry point: Build, annotate, and prune tree structure
     */
    analyzeFieldDiscrepancies(dtoElement, entity, mappings, sourceElement) {
        console.log(`[BUILD] Starting structure tree for DTO: ${dtoElement.getName()}`);
        console.log(`[BUILD] ├─ Entity: ${entity.getName()}`);
        this.buildStructureTree(dtoElement, entity, mappings, sourceElement);
        const discrepancies = [];
        const parameters = (sourceElement === null || sourceElement === void 0 ? void 0 : sourceElement.getChildren("Parameter")) || [];
        if (parameters.length > 0) {
            this.detectOperationParameterDiscrepancies(sourceElement, dtoElement, entity, mappings, discrepancies);
        }
        // Check DTO fields recursively
        this.detectDiscrepanciesRecursive(dtoElement, entity, mappings, discrepancies, 0);
        this.annotateTreeWithDiscrepancies(this.lastStructureTree, discrepancies);
        console.log(`[ANALYZE] └─ Total discrepancies detected: ${discrepancies.length}`);
        this.pruneTreeWithoutDiscrepancies(this.lastStructureTree);
        return discrepancies;
    }
    /**
     * Check operation parameters for discrepancies
     */
    detectOperationParameterDiscrepancies(sourceElement, dtoElement, entity, mappings, discrepancies) {
        const operationParams = sourceElement.getChildren("Parameter");
        console.log(`[ANALYZE-PARAMS] Checking ${operationParams.length} operation parameters`);
        const entityAttrMap = this.getOrBuildEntityAttributeMap(entity);
        const mappingsBySourceId = this.groupMappingsBySourceId(mappings);
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
                    sourceFieldId: param.id,
                    sourceFieldName: param.getName(),
                    sourceFieldTypeName: (paramTypeRef === null || paramTypeRef === void 0 ? void 0 : paramTypeRef.display) || "Unknown",
                    targetAttributeName: "(no mapping)",
                    icon: param.getIcon(),
                    reason: `Parameter '${param.getName()}' is not mapped to any entity attribute`,
                    sourceParentId: sourceElement.id
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
                    // Check for rename - use naming convention-aware comparison
                    // This allows "id" to match "Id" (camelCase vs PascalCase)
                    if (!namesAreEquivalent(paramName, entityAttrName)) {
                        const discrepancy = {
                            id: `rename-param-${param.id}`,
                            type: "RENAME",
                            sourceFieldId: param.id,
                            sourceFieldName: paramName,
                            sourceFieldTypeName: (paramTypeRef === null || paramTypeRef === void 0 ? void 0 : paramTypeRef.display) || "Unknown",
                            targetAttributeId: targetAttr.id,
                            targetAttributeName: entityAttrName,
                            targetAttributeTypeName: targetAttr.typeDisplayText,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' should be renamed to '${entityAttrName}'`,
                            sourceParentId: sourceElement.id
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
                            sourceFieldId: param.id,
                            sourceFieldName: paramName,
                            sourceFieldTypeName: paramType,
                            targetAttributeId: targetAttr.id,
                            targetAttributeName: entityAttrName,
                            targetAttributeTypeName: entityType,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' type mismatch: ${paramType} vs ${entityType}`,
                            sourceParentId: sourceElement.id
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
    /**
     * Get cached entity attribute map or build it
     */
    getOrBuildEntityAttributeMap(entity) {
        const cached = this.entityMapCache.get(entity.id);
        if (cached) {
            return cached;
        }
        const map = this.buildEntityAttributeMap(entity);
        this.entityMapCache.set(entity.id, map);
        return map;
    }
    buildEntityAttributeMap(entity) {
        const entityAttrs = getEntityAttributes(entity);
        const entityAttrMap = new Map();
        for (const attr of entityAttrs) {
            entityAttrMap.set(attr.id, attr);
            entityAttrMap.set(attr.name.toLowerCase(), attr);
        }
        return entityAttrMap;
    }
    groupMappingsBySourceId(mappings) {
        const grouped = new Map();
        for (const mapping of mappings) {
            const sourceFieldId = mapping.sourceFieldId;
            if (!grouped.has(sourceFieldId)) {
                grouped.set(sourceFieldId, []);
            }
            grouped.get(sourceFieldId).push(mapping);
        }
        return grouped;
    }
    isComplexType(element) {
        return element.specialization === "DTO" || element.specialization === "Class";
    }
    isNestedMapping(mapping, parentFieldId) {
        if (!mapping.sourcePath || mapping.sourcePath.length <= 1) {
            return false;
        }
        const parentIndex = mapping.sourcePath.indexOf(parentFieldId);
        return parentIndex >= 0 && parentIndex < mapping.sourcePath.length - 1;
    }
    adjustMappingForNestedContext(mapping, parentFieldId) {
        const parentIndex = mapping.sourcePath.indexOf(parentFieldId);
        return {
            ...mapping,
            sourcePath: mapping.sourcePath.slice(parentIndex + 1)
        };
    }
    detectDiscrepanciesRecursive(dtoElement, entity, mappings, discrepancies, depth = 0, parentFieldId) {
        var _a, _b, _c;
        const indent = Array(depth).fill("  ").join("");
        const entityAttrMap = this.getOrBuildEntityAttributeMap(entity);
        const mappingsBySourceId = this.groupMappingsBySourceId(mappings);
        // Check each DTO field for discrepancies
        const dtoChildren = dtoElement.getChildren("DTO-Field");
        for (const dtoField of dtoChildren) {
            const fieldMappings = mappingsBySourceId.get(dtoField.id) || [];
            const fieldTypeRef = dtoField.typeReference;
            // Check if this is a nested DTO/association field
            let isAssociationField = false;
            let nestedEntity = null;
            let nestedMappings = [];
            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType();
                if (fieldType && this.isComplexType(fieldType)) {
                    nestedEntity = this.findAssociatedEntity(dtoField, entity, mappings);
                    if (nestedEntity) {
                        isAssociationField = true;
                        nestedMappings = this.getNestedMappings(mappings, dtoField.id);
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
                    sourceFieldId: dtoField.id,
                    sourceFieldName: dtoField.getName(),
                    sourceFieldTypeName: ((_a = dtoField.typeReference) === null || _a === void 0 ? void 0 : _a.display) || "Unknown",
                    targetAttributeName: "(no mapping)",
                    icon: dtoField.getIcon(),
                    reason: `Field '${dtoField.getName()}' is not mapped to any entity attribute`,
                    sourceParentId: dtoElement.id
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
                    // Check for rename - use naming convention-aware comparison
                    // This allows "id" to match "Id" (camelCase vs PascalCase)
                    if (!namesAreEquivalent(dtoFieldName, entityAttrName)) {
                        const discrepancy = {
                            id: `rename-${dtoField.id}`,
                            type: "RENAME",
                            sourceFieldId: dtoField.id,
                            sourceFieldName: dtoFieldName,
                            sourceFieldTypeName: ((_b = dtoField.typeReference) === null || _b === void 0 ? void 0 : _b.display) || "Unknown",
                            targetAttributeId: targetAttr.id,
                            targetAttributeName: entityAttrName,
                            targetAttributeTypeName: targetAttr.typeDisplayText,
                            icon: dtoField.getIcon(),
                            reason: `Field '${dtoFieldName}' should be renamed to '${entityAttrName}'`,
                            sourceParentId: dtoElement.id
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
                            sourceFieldId: dtoField.id,
                            sourceFieldName: dtoFieldName,
                            sourceFieldTypeName: dtoType,
                            targetAttributeId: targetAttr.id,
                            targetAttributeName: entityAttrName,
                            targetAttributeTypeName: entityType,
                            icon: dtoField.getIcon(),
                            reason: `Field '${dtoFieldName}' type mismatch: ${dtoType} vs ${entityType}`,
                            sourceParentId: dtoElement.id
                        };
                        discrepancies.push(discrepancy);
                        console.log(`${indent}[ANALYZE] ├─ [CHANGE_TYPE] ${dtoFieldName}: ${dtoType} → ${entityType}`);
                    }
                }
            }
        }
        const mappedEntityIds = new Set();
        for (const mapping of mappings) {
            mappedEntityIds.add(mapping.targetAttributeId);
        }
        const entityAttrs = getEntityAttributes(entity);
        for (const entityAttr of entityAttrs) {
            // Show NEW discrepancies for unmapped attributes, including FK attributes
            // Only exclude primary keys (which are managed keys with Primary Key stereotype)
            const isPrimaryKey = entityAttr.isManagedKey && entityAttr.hasPrimaryKeyStereotype;
            if (!mappedEntityIds.has(entityAttr.id) && !isPrimaryKey) {
                const contextId = parentFieldId || dtoElement.id;
                const discrepancy = {
                    id: `new-${entityAttr.id}-${contextId}`,
                    type: "NEW",
                    sourceFieldId: contextId,
                    sourceFieldName: "(missing)",
                    sourceFieldTypeName: "N/A",
                    targetAttributeId: entityAttr.id,
                    targetAttributeName: entityAttr.name,
                    targetAttributeTypeName: entityAttr.typeDisplayText,
                    icon: entityAttr.icon,
                    reason: `Entity attribute '${entityAttr.name}' is not present in DTO`,
                    sourceParentId: dtoElement.id
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${entityAttr.name}: Entity attribute not in DTO`);
            }
        }
        // Check for missing associations (nested DTOs)
        // Include both source-end (this entity -> other) and target-end (other -> this entity)
        const allAssociations = entity.getAssociations("Association");
        const sourceEndAssociations = allAssociations.filter(assoc => assoc.isSourceEnd());
        const targetEndAssociations = allAssociations.filter(assoc => !assoc.isSourceEnd());
        console.log(`${indent}[ANALYZE-ASSOC] Found ${sourceEndAssociations.length} source-end and ${targetEndAssociations.length} target-end associations on ${entity.getName()}`);
        const mappedAssociationIds = new Set();
        for (const mapping of mappings) {
            // Check if this mapping represents an association
            if (mapping.targetPath && mapping.targetPath.length > 0) {
                const lastTargetId = mapping.targetPath[mapping.targetPath.length - 1];
                mappedAssociationIds.add(lastTargetId);
            }
        }
        // Process source-end associations (parent -> child)
        for (const assoc of sourceEndAssociations) {
            const assocName = assoc.getName ? assoc.getName() : null;
            const targetTypeRef = assoc.typeReference;
            if (!assocName || !targetTypeRef || !targetTypeRef.isTypeFound()) {
                console.log(`${indent}[ANALYZE-ASSOC] Skipping source-end association - invalid name or type`);
                continue;
            }
            const targetEntity = targetTypeRef.getType();
            const assocId = assoc.id;
            console.log(`${indent}[ANALYZE-ASSOC] Checking source-end association ${assocName} (id: ${assocId}, mapped: ${mappedAssociationIds.has(assocId)})`);
            // Check if this association is mapped
            if (!mappedAssociationIds.has(assocId)) {
                const contextId = parentFieldId || dtoElement.id;
                const dtoName = dtoElement.getName();
                const entityName = targetEntity.getName();
                // Generate expected DTO name: {SourceDTO}{EntityName}Dto
                const expectedDtoName = `${dtoName}${entityName}Dto`;
                const isCollection = targetTypeRef.isCollection ? "[*]" : "";
                const discrepancy = {
                    id: `new-assoc-${assocId}-${contextId}`,
                    type: "NEW",
                    sourceFieldId: contextId,
                    sourceFieldName: "(missing)",
                    sourceFieldTypeName: `${expectedDtoName}${isCollection}`,
                    targetAttributeId: assocId,
                    targetAttributeName: assocName,
                    targetAttributeTypeName: `${entityName}${isCollection}`,
                    icon: targetEntity.getIcon(),
                    reason: `Association '${assocName}' (type: ${entityName}) is not present in DTO. Consider adding: ${expectedDtoName}`,
                    sourceParentId: dtoElement.id
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${assocName}: Association to ${entityName} should be added as ${expectedDtoName}`);
            }
        }
        // Process target-end associations (these represent inverse relationships that should also be included)
        // But only if they're navigable and represent real composite associations from the perspective of the DTO
        for (const assoc of targetEndAssociations) {
            const assocName = assoc.getName ? assoc.getName() : null;
            const typeRef = assoc.typeReference;
            if (!assocName || !typeRef || !typeRef.isTypeFound() || !typeRef.isNavigable) {
                continue;
            }
            // For target-end associations, check the OTHER end (source end) to determine if it's composite
            const otherEnd = assoc.getOtherEnd ? assoc.getOtherEnd() : null;
            if (!otherEnd || !otherEnd.typeReference) {
                continue;
            }
            // Composite relationship: source end should NOT be collection or nullable
            // If source end is collection or nullable, it's an aggregate and should be filtered
            const isComposite = !otherEnd.typeReference.isCollection && !otherEnd.typeReference.isNullable;
            if (!isComposite) {
                console.log(`${indent}[ANALYZE-ASSOC] Skipping target-end association ${assocName} - not composite (source is collection or nullable)`);
                continue;
            }
            const sourceEntity = typeRef.getType();
            const assocId = assoc.id;
            console.log(`${indent}[ANALYZE-ASSOC] Checking target-end association ${assocName} (id: ${assocId}, mapped: ${mappedAssociationIds.has(assocId)}, composite: ${isComposite})`);
            // Check if this association is mapped
            if (!mappedAssociationIds.has(assocId)) {
                const contextId = parentFieldId || dtoElement.id;
                const dtoName = dtoElement.getName();
                const entityName = sourceEntity.getName();
                // Generate expected DTO name: {SourceDTO}{EntityName}Dto
                const expectedDtoName = `${dtoName}${entityName}Dto`;
                const isCollection = typeRef.isCollection ? "[*]" : "";
                const discrepancy = {
                    id: `new-assoc-${assocId}-${contextId}`,
                    type: "NEW",
                    sourceFieldId: contextId,
                    sourceFieldName: "(missing)",
                    sourceFieldTypeName: `${expectedDtoName}${isCollection}`,
                    targetAttributeId: assocId,
                    targetAttributeName: assocName,
                    targetAttributeTypeName: `${entityName}${isCollection}`,
                    icon: sourceEntity.getIcon(),
                    reason: `Association '${assocName}' (type: ${entityName}) is not present in DTO. Consider adding: ${expectedDtoName}`,
                    sourceParentId: dtoElement.id
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${assocName}: Target-end association to ${entityName} should be added as ${expectedDtoName}`);
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
        const associations = entity.getAssociations();
        for (const assoc of associations) {
            const assocName = assoc.getName ? assoc.getName() : null;
            const fieldNameSingular = singularize(dtoFieldName);
            if (assocName && (assocName === dtoFieldName || assocName === fieldNameSingular)) {
                if (assoc.typeReference && assoc.typeReference.isTypeFound()) {
                    return assoc.typeReference.getType();
                }
            }
        }
        return null;
    }
    getNestedMappings(allMappings, parentFieldId) {
        return allMappings
            .filter(mapping => this.isNestedMapping(mapping, parentFieldId))
            .map(mapping => this.adjustMappingForNestedContext(mapping, parentFieldId));
    }
    /**
     * Build the complete natural tree structure from DTO/entity
     */
    buildStructureTree(dtoElement, entity, mappings, sourceElement) {
        var _a;
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
            children: [],
            elementParentId: (_a = dtoElement.getParent()) === null || _a === void 0 ? void 0 : _a.id
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
                    children: [],
                    elementParentId: sourceElement.id
                };
                rootNode.children.push(paramNode);
            }
        }
        // Add DTO fields (direct properties)
        const dtoChildren = dtoElement.getChildren("DTO-Field");
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
                children: [],
                elementParentId: dtoElement.id
            };
            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType();
                if (fieldType && this.isComplexType(fieldType)) {
                    this.addDtoFieldsRecursive(fieldNode, fieldType, 2);
                }
            }
            rootNode.children.push(fieldNode);
        }
        this.lastStructureTree = rootNode;
    }
    addDtoFieldsRecursive(parentNode, dtoElement, depth) {
        const indent = Array(depth).fill("│  ").join("");
        const children = dtoElement.getChildren("DTO-Field");
        for (const child of children) {
            const childTypeRef = child.typeReference;
            const childTypeName = (childTypeRef === null || childTypeRef === void 0 ? void 0 : childTypeRef.display) || "Unknown";
            console.log(`[BUILD] ${indent}├─ ${child.getName()}: ${childTypeName}`);
            const childNode = {
                id: child.id,
                label: TreeNodeLabelBuilder.buildFieldLabel(child.getName(), childTypeName),
                specializationId: "structure-nested-field",
                elementId: child.id,
                elementType: "Nested-Field",
                originalName: child.getName(),
                originalType: childTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: child.getIcon(),
                children: [],
                elementParentId: dtoElement.id
            };
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const nestedType = childTypeRef.getType();
                if (nestedType && this.isComplexType(nestedType)) {
                    this.addDtoFieldsRecursive(childNode, nestedType, depth + 1);
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
            // NEW discrepancies are synthetic nodes, don't mark their parent as having a discrepancy
            if (disc.type === "NEW") {
                continue;
            }
            if (disc.sourceFieldId) {
                elementIdsWithDiscrepancies.add(disc.sourceFieldId);
                discrepancyByElementId.set(disc.sourceFieldId, disc);
            }
            if (disc.targetAttributeId) {
                elementIdsWithDiscrepancies.add(disc.targetAttributeId);
                discrepancyByElementId.set(disc.targetAttributeId, disc);
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
        const newDiscrepanciesForThisNode = discrepancies.filter(d => d.type === "NEW" && d.sourceFieldId === node.elementId);
        if (newDiscrepanciesForThisNode.length > 0) {
            console.log(`[ANNOTATE] Adding ${newDiscrepanciesForThisNode.length} NEW nodes to ${node.originalName} (elementId: ${node.elementId})`);
        }
        // Add them as children
        for (const newDisc of newDiscrepanciesForThisNode) {
            const newNode = {
                id: newDisc.id,
                label: `${newDisc.targetAttributeName}: ${newDisc.targetAttributeTypeName}`,
                specializationId: "structure-dto-field",
                elementId: newDisc.targetAttributeId,
                elementType: "NEW-Field",
                originalName: newDisc.targetAttributeName,
                originalType: newDisc.targetAttributeTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: newDisc.icon,
                discrepancy: newDisc,
                children: [],
                elementParentId: newDisc.sourceParentId
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
        // Start with pre-existing discrepancy marker (for synthetic NEW nodes)
        let nodeHasDiscrepancies = node.discrepancy !== undefined || elementIdsWithDiscrepancies.has(node.elementId);
        // Attach discrepancy object if this node has one
        if (nodeHasDiscrepancies && !node.discrepancy && discrepancyByElementId.has(node.elementId)) {
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
        // Filter children - keep only those with discrepancies
        node.children = node.children.filter((child) => {
            const extChild = child;
            const keep = extChild.hasDiscrepancies ||
                extChild.elementType === "DTO"; // Keep root DTO
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
        const displayNodes = this.convertTreeToDisplayNodes(this.lastStructureTree);
        return displayNodes;
    }
    /**
     * Recursively convert tree nodes to display nodes
     */
    convertTreeToDisplayNodes(node) {
        var _a;
        const result = [];
        // Process this node's children (skip root itself)
        if (node.children) {
            console.log(`[BUILD-DISPLAY] Processing ${node.children.length} children of ${node.originalName} (elementId: ${node.elementId}, type: ${node.elementType})`);
            for (const child of node.children) {
                const extChild = child;
                console.log(`[BUILD-DISPLAY]   ├─ Child: ${extChild.originalName} (elementId: ${extChild.elementId}, type: ${extChild.elementType}, hasDiscrepancy: ${!!extChild.discrepancy}, children: ${((_a = extChild.children) === null || _a === void 0 ? void 0 : _a.length) || 0})`);
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
            children: [],
            discrepancy: node.discrepancy // Preserve discrepancy metadata for execution
        };
        // If this node has a discrepancy, attach the display function
        if (node.discrepancy) {
            const discrepancy = node.discrepancy;
            console.log(`[BUILD-DISPLAY]     └─ Has discrepancy: ${discrepancy.type} (dtoFieldId: ${discrepancy.sourceFieldId}, nodeId: ${node.elementId})`);
            // For NEW items, always use entityAttributeName; for others use dtoFieldName
            const fieldName = discrepancy.type === "NEW"
                ? discrepancy.targetAttributeName
                : (discrepancy.sourceFieldName || discrepancy.targetAttributeName || "Unknown");
            displayNode.displayFunction = createDiscrepancyDisplayFunction(discrepancy, fieldName);
            displayNode.specializationId = `discrepancy-${discrepancy.type.toLowerCase()}`;
            displayNode.label = TreeNodeLabelBuilder.buildDiscrepancyLabel(discrepancy);
        }
        if (node.children && node.children.length > 0) {
            console.log(`[BUILD-DISPLAY]     └─ Processing ${node.children.length} nested children`);
            displayNode.children = [];
            for (const child of node.children) {
                const extChild = child;
                const childDisplayNode = this.createDisplayNode(extChild);
                displayNode.children.push(childDisplayNode);
            }
        }
        return displayNode;
    }
    /**
     * Apply sync actions for selected tree nodes
     */
    applySyncActions(dtoElement, entity, selectedNodes, associations) {
        const executor = new NodeSyncExecutor();
        executor.execute(selectedNodes, dtoElement, entity, associations);
    }
}
/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="sync-dto-fields-engine.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
/**
 * Build a flat map of all tree nodes by ID for quick lookup
 */
function buildNodeMap(treeNodes) {
    const map = new Map();
    function traverse(nodes) {
        for (const node of nodes) {
            map.set(node.id, node);
            // Recurse into children
            if (node.children && node.children.length > 0) {
                traverse(node.children);
            }
        }
    }
    traverse(treeNodes);
    return map;
}
/**
 * Find all tree nodes that match the given selected IDs using the pre-built map
 */
function findSelectedNodes(nodeMap, selectedIds) {
    var _a;
    const result = [];
    for (const id of selectedIds) {
        const node = nodeMap.get(id);
        if (node) {
            result.push(node);
            console.log(`[FIND-NODES]   ├─ Node ${node.id}: has discrepancy = ${!!node.discrepancy}, type = ${(_a = node.discrepancy) === null || _a === void 0 ? void 0 : _a.type}`);
        }
        else {
            console.log(`[FIND-NODES]   ├─ Node ${id}: NOT FOUND in map`);
        }
    }
    console.log(`[FIND-NODES] Found ${result.length} nodes from ${selectedIds.length} selected IDs`);
    return result;
}
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
    const discrepancies = engine.analyzeFieldDiscrepancies(dtoElement, entity, fieldMappings, element);
    console.log(`[SYNC] ├─ Discrepancies found: ${discrepancies.length}`);
    // Build tree view model with hierarchical structure
    const treeNodes = engine.buildHierarchicalTreeNodes(element, dtoElement, discrepancies);
    console.log(`[SYNC] ├─ Tree nodes built: ${treeNodes.length}`);
    // Build node map for quick lookup after dialog (preserves discrepancy metadata)
    const nodeMap = buildNodeMap(treeNodes);
    console.log(`[SYNC] ├─ Node map built with ${nodeMap.size} entries`);
    // Present dialog with results
    const selectedNodeIds = await presentSyncDialog(element, dtoElement, entity, discrepancies, treeNodes);
    console.log(`[SYNC] ├─ Selected nodes: ${selectedNodeIds.length}`);
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        // Find the selected tree nodes using the preserved map
        const selectedNodes = findSelectedNodes(nodeMap, selectedNodeIds);
        console.log(`[SYNC] └─ Applying ${selectedNodes.length} sync actions`);
        engine.applySyncActions(dtoElement, entity, selectedNodes, associations);
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
                        "dto-sync-root",
                        "structure-operation-param",
                        "structure-dto-field",
                        "structure-nested-field",
                        "discrepancy-delete",
                        "discrepancy-new",
                        "discrepancy-rename",
                        "discrepancy-change_type"
                    ].map(id => ({
                        specializationId: id,
                        isSelectable: true,
                        autoExpand: true,
                        autoSelectChildren: true
                    }))
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
