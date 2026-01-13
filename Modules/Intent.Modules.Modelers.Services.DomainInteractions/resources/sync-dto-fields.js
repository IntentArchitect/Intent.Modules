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
    var _a, _b, _c, _d, _e;
    const attributes = [];
    const children = entity.getChildren("Attribute");
    for (const child of children) {
        // Determine mappability per Intent Architect's mapping requirements
        // Check for auto-generated primary key (PK with non-user-supplied data source)
        let isAutoGeneratedPk = false;
        if (child.hasStereotype && child.hasStereotype("Primary Key")) {
            const pkStereotype = child.getStereotype("Primary Key");
            if (pkStereotype && pkStereotype.getProperty) {
                const dataSourceProp = pkStereotype.getProperty("Data source");
                isAutoGeneratedPk = dataSourceProp && dataSourceProp.value !== "User supplied";
            }
        }
        // Check for foreign key
        let isForeignKey = false;
        let fkIsRequiredParent = false;
        if (child.hasStereotype && child.hasStereotype("Foreign Key")) {
            isForeignKey = true;
            const fkStereotype = child.getStereotype("Foreign Key");
            if (fkStereotype && fkStereotype.getProperty) {
                const assocProp = fkStereotype.getProperty("Association");
                if (assocProp && assocProp.getSelected) {
                    const fkAssociation = assocProp.getSelected();
                    if (fkAssociation) {
                        const otherEnd = (_a = fkAssociation.getOtherEnd) === null || _a === void 0 ? void 0 : _a.call(fkAssociation);
                        if (otherEnd && otherEnd.typeReference) {
                            // FK is required parent if NOT a collection and NOT nullable
                            fkIsRequiredParent = !otherEnd.typeReference.isCollection && !otherEnd.typeReference.isNullable;
                        }
                    }
                }
            }
        }
        // Check for infrastructure-managed field
        const isSetByInfrastructure = child.hasMetadata && child.hasMetadata("set-by-infrastructure") &&
            child.getMetadata("set-by-infrastructure") === "true";
        // Determine if mappable using Intent Architect's logic
        // An attribute is mappable if:
        // - It's NOT an auto-generated primary key
        // - If it's a foreign key, only if it's NOT a required parent (non-collection, non-nullable)
        // - It's NOT infrastructure-managed
        const isMappable = !isAutoGeneratedPk &&
            (!isForeignKey || !fkIsRequiredParent) &&
            !isSetByInfrastructure;
        const attribute = {
            id: child.id,
            name: child.getName(),
            typeId: (_b = child.typeReference) === null || _b === void 0 ? void 0 : _b.getTypeId(),
            typeDisplayText: ((_c = child.typeReference) === null || _c === void 0 ? void 0 : _c.display) || "",
            icon: child.getIcon(),
            isManagedKey: child.hasMetadata(METADATA_KEYS.IS_MANAGED_KEY) && child.getMetadata(METADATA_KEYS.IS_MANAGED_KEY) === "true",
            hasPrimaryKeyStereotype: child.hasStereotype && child.hasStereotype("Primary Key"),
            isCollection: (_d = child.typeReference) === null || _d === void 0 ? void 0 : _d.isCollection,
            isNullable: (_e = child.typeReference) === null || _e === void 0 ? void 0 : _e.isNullable,
            isAutoGeneratedPk,
            isForeignKey,
            fkIsRequiredParent,
            isSetByInfrastructure,
            isMappable
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
            case "ADD":
                return `[ADD] ${discrepancy.targetAttributeName}: ${discrepancy.targetAttributeTypeName}`;
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
    ADD: "#22c55e", // Green
    DELETE: "#ef4444", // Red
    RENAME: "#007777", // Teal
    CHANGE_TYPE: "#f97316" // Orange
};
const DISCREPANCY_LABELS = {
    ADD: "[ADD]",
    DELETE: "[DELETE]",
    RENAME: "[RENAME]",
    CHANGE_TYPE: "[CHANGE TYPE]"
};
// Build full type display with modifiers (e.g., "string[]?", "int")
function buildTypeDisplay(baseType, isCollection, isNullable) {
    let display = baseType || "Unknown";
    if (isCollection)
        display += "[*]";
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
        case "ADD":
            // cleanFieldName: type [ADD]
            components.push({ text: cleanFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            const newTypeDisplay = buildTypeDisplay(discrepancy.targetAttributeTypeName || "", discrepancy.targetIsCollection || false, discrepancy.targetIsNullable || false);
            components.push({ text: newTypeDisplay, cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.ADD, color: statusInfo.color });
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
        case "ADD":
            return { color: DISCREPANCY_COLORS.ADD, cssClass: "keyword" };
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
 * Structure-first DTO field synchronization engine
 *
 * The engine receives three explicit context elements to eliminate conditional checks:
 * 1. rootSourceElement: Where associations are located (Command or Service Operation)
 * 2. rootTargetEntity: The root entity being synchronized to
 * 3. workingSourceElement: Where new DTO fields are created (DTO or Command)
 *
 * This architecture provides clear separation of concerns:
 * - Association lookups: always on rootSourceElement
 * - Field mutations: always on current workingSourceElement (changes per recursion level)
 * - Entity context: current workingTargetEntity (changes per recursion level)
 */
class FieldSyncEngine {
    constructor() {
        this.lastStructureTree = null;
        this.entityMapCache = new Map();
    }
    /**
     * Get the tree nodes for display - no conversion needed since IExtendedTreeNode extends ISelectableTreeNode
     */
    buildHierarchicalTreeNodes() {
        // Return children directly - IExtendedTreeNode is compatible with ISelectableTreeNode
        if (!this.lastStructureTree || !this.lastStructureTree.children) {
            return [];
        }
        return this.lastStructureTree.children;
    }
    /**
     * Apply sync actions for selected tree nodes
     *
     * @param rootSourceElement - Where associations are located (Command or Operation)
     * @param rootTargetEntity - Root entity for synchronization
     * @param workingSourceElement - Current DTO being edited
     * @param selectedNodes - Tree nodes selected by user for synchronization
     */
    applySyncActions(rootSourceElement, rootTargetEntity, workingSourceElement, selectedNodes) {
        const executor = new NodeSyncExecutor(rootSourceElement, rootTargetEntity, workingSourceElement);
        executor.execute(selectedNodes);
    }
    /**
     * Main entry point: Analyze field discrepancies between working source and target entity
     *
     * @param rootSourceElement - Where associations are located (Command or Operation) - CONSTANT
     * @param rootTargetEntity - Root entity for synchronization - CONSTANT
     * @param workingSourceElement - Current DTO being analyzed (changes per recursion)
     * @param fieldMappings - Existing field mappings from associations
     * @returns Array of detected discrepancies
     */
    analyzeFieldDiscrepancies(rootSourceElement, rootTargetEntity, workingSourceElement, fieldMappings) {
        console.log(`[BUILD] Starting structure tree for DTO: ${workingSourceElement.getName()}`);
        console.log(`[BUILD] ├─ Root source element: ${rootSourceElement.getName()} (${rootSourceElement.specialization})`);
        console.log(`[BUILD] ├─ Root target entity: ${rootTargetEntity.getName()}`);
        // NEW: Pass entity to buildStructureTree so it can pre-compute targetPath
        this.buildStructureTree(rootSourceElement, rootTargetEntity, workingSourceElement);
        const discrepancies = [];
        const parameters = rootSourceElement.specialization === "Operation" ? rootSourceElement.getChildren("Parameter") : [];
        if (parameters.length > 0) {
            this.detectOperationParameterDiscrepancies(rootSourceElement, workingSourceElement, rootTargetEntity, fieldMappings, discrepancies);
        }
        // Check DTO fields recursively
        this.detectDiscrepanciesRecursive(workingSourceElement, rootTargetEntity, fieldMappings, discrepancies, 0);
        this.annotateTreeWithDiscrepancies(this.lastStructureTree, discrepancies);
        console.log(`[ANALYZE] └─ Total discrepancies detected: ${discrepancies.length}`);
        this.pruneTreeWithoutDiscrepancies(this.lastStructureTree);
        return discrepancies;
    }
    /**
     * Check operation parameters for discrepancies
     * Only called when rootSourceElement is a Service Operation.
     */
    detectOperationParameterDiscrepancies(rootSourceElement, workingSourceElement, rootTargetEntity, mappings, discrepancies) {
        const operationParams = rootSourceElement.getChildren("Parameter");
        console.log(`[ANALYZE-PARAMS] Checking ${operationParams.length} operation parameters`);
        const entityAttrMap = this.getOrBuildEntityAttributeMap(rootTargetEntity);
        const mappingsBySourceId = this.groupMappingsBySourceId(mappings);
        for (const param of operationParams) {
            const paramTypeRef = param.typeReference;
            // Skip the DTO parameter itself
            if (paramTypeRef && paramTypeRef.isTypeFound()) {
                const paramType = paramTypeRef.getType();
                if (paramType && paramType.id === workingSourceElement.id) {
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
                    sourceParentId: rootSourceElement.id
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
                            targetId: targetAttr.id,
                            targetAttributeName: entityAttrName,
                            targetAttributeTypeName: targetAttr.typeDisplayText,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' should be renamed to '${entityAttrName}'`,
                            sourceParentId: rootSourceElement.id
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
                            targetId: targetAttr.id,
                            targetAttributeName: entityAttrName,
                            targetAttributeTypeName: entityType,
                            targetTypeId: targetAttr.typeId,
                            targetIsCollection: targetAttr.isCollection,
                            targetIsNullable: targetAttr.isNullable,
                            icon: param.getIcon(),
                            reason: `Parameter '${paramName}' type mismatch: ${paramType} vs ${entityType}`,
                            sourceParentId: rootSourceElement.id
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
                    nestedEntity = this.findAssociatedEntity(dtoField, entity);
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
                            targetId: targetAttr.id,
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
                            targetId: targetAttr.id,
                            targetAttributeName: entityAttrName,
                            targetAttributeTypeName: entityType,
                            targetTypeId: targetAttr.typeId,
                            targetIsCollection: targetAttr.isCollection,
                            targetIsNullable: targetAttr.isNullable,
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
            // Show NEW discrepancies only for unmapped, mappable attributes
            // Skip attributes that shouldn't be mapped per Intent Architect's mapping rules:
            // - Auto-generated primary keys
            // - Required parent foreign keys (non-collection, non-nullable)
            // - Infrastructure-managed fields
            const isUnmapped = !mappedEntityIds.has(entityAttr.id);
            if (isUnmapped && entityAttr.isMappable) {
                const contextId = parentFieldId || dtoElement.id;
                const discrepancy = {
                    id: `new-${entityAttr.id}-${contextId}`,
                    type: "ADD",
                    sourceFieldId: contextId,
                    sourceFieldName: "(missing)",
                    sourceFieldTypeName: "N/A",
                    targetId: entityAttr.id,
                    targetAttributeName: entityAttr.name,
                    targetAttributeTypeName: entityAttr.typeDisplayText,
                    targetTypeId: entityAttr.typeId,
                    targetIsCollection: entityAttr.isCollection,
                    targetIsNullable: entityAttr.isNullable,
                    icon: entityAttr.icon,
                    reason: `Entity attribute '${entityAttr.name}' is not present in DTO`,
                    sourceParentId: dtoElement.id
                };
                discrepancies.push(discrepancy);
                console.log(`${indent}[ANALYZE] ├─ [NEW] ${entityAttr.name}: Entity attribute not in DTO`);
            }
        }
        // Check for missing associations (nested DTOs)
        // Only process TARGET-END associations (where current entity is the owning/source side)
        const allAssociations = entity.getAssociations("Association");
        const targetEndAssociations = allAssociations.filter(assoc => !assoc.isSourceEnd());
        console.log(`${indent}[ANALYZE-ASSOC] Found ${targetEndAssociations.length} target-end associations on ${entity.getName()}`);
        const mappedAssociationIds = new Set();
        for (const mapping of mappings) {
            // Check if this mapping represents an association
            if (mapping.targetPath && mapping.targetPath.length > 0) {
                const lastTargetId = mapping.targetPath[mapping.targetPath.length - 1];
                mappedAssociationIds.add(lastTargetId);
            }
        }
        // Process target-end associations (where current entity owns/points to composite entities)
        for (const assoc of targetEndAssociations) {
            const assocName = assoc.getName ? assoc.getName() : null;
            const typeRef = assoc.typeReference;
            if (!assocName || !typeRef || !typeRef.isTypeFound()) {
                continue;
            }
            const targetEntity = typeRef.getType();
            const assocId = assoc.id;
            // SKIP: Self-referential associations (target points back to current entity)
            if (targetEntity.id === entity.id) {
                console.log(`${indent}[ANALYZE-ASSOC] Skipping target-end association ${assocName} - self-referential (target is same entity)`);
                continue;
            }
            // SKIP: Aggregate relationships (source end has multiplicity > 1 or is nullable)
            // Only process COMPOSITE relationships (source end has multiplicity of 1)
            const otherEnd = assoc.getOtherEnd ? assoc.getOtherEnd() : null;
            if (!otherEnd || !otherEnd.typeReference) {
                continue;
            }
            const isComposite = !otherEnd.typeReference.isCollection && !otherEnd.typeReference.isNullable;
            if (!isComposite) {
                console.log(`${indent}[ANALYZE-ASSOC] Skipping target-end association ${assocName} - aggregate relationship (source end is collection or nullable)`);
                continue;
            }
            console.log(`${indent}[ANALYZE-ASSOC] Checking target-end association ${assocName} (id: ${assocId}, mapped: ${mappedAssociationIds.has(assocId)})`);
            // Check if this association is mapped
            if (!mappedAssociationIds.has(assocId)) {
                // Before suggesting NEW, check if a field with this association name already exists
                const existingField = dtoChildren.find(field => namesAreEquivalent(field.getName(), assocName));
                if (existingField) {
                    // Field exists but is unmapped - it should be flagged as DELETE at next sync
                    console.log(`${indent}[ANALYZE-ASSOC] Field '${assocName}' exists but is unmapped - will be handled by DELETE logic`);
                }
                else {
                    // Field doesn't exist and association is unmapped - suggest NEW
                    const contextId = parentFieldId || dtoElement.id;
                    const dtoName = dtoElement.getName();
                    const entityName = targetEntity.getName();
                    // Generate expected DTO name: {SourceDTO}{EntityName}Dto
                    const expectedDtoName = `${dtoName}${entityName}Dto`;
                    const isCollection = typeRef.isCollection;
                    const isNullable = typeRef.isNullable;
                    const discrepancy = {
                        id: `new-assoc-${assocId}-${contextId}`,
                        type: "ADD",
                        sourceFieldId: contextId,
                        sourceFieldName: "(missing)",
                        sourceFieldTypeName: expectedDtoName, // Pure display name, no [*]
                        sourceIsCollection: isCollection,
                        sourceIsNullable: isNullable,
                        targetId: assocId,
                        targetIdType: "association", // Discriminator: this is an association
                        targetAttributeName: assocName,
                        targetAttributeTypeName: expectedDtoName, // Pure display name, no [*]
                        targetIsCollection: isCollection,
                        targetIsNullable: isNullable,
                        suggestedDtoName: expectedDtoName, // For applyNewAssociation
                        icon: targetEntity.getIcon(),
                        reason: `Association '${assocName}' (type: ${entityName}) is not present in DTO. Consider adding: ${expectedDtoName}`,
                        sourceParentId: dtoElement.id
                    };
                    discrepancies.push(discrepancy);
                    console.log(`${indent}[ANALYZE] ├─ [NEW] ${assocName}: Target-end association to ${entityName} should be added as ${expectedDtoName}`);
                }
            }
        }
    }
    /**
     * Find the associated entity for a nested DTO field
     * Match by the association name (e.g., "Block1Level3" field should match "Block1Level3" association)
     */
    findAssociatedEntity(dtoField, entity) {
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
     * OPTION A: Pre-compute both sourcePath and targetPath during tree construction
     */
    buildStructureTree(rootSourceElement, rootTargetEntity, workingSourceElement) {
        var _a;
        // For Operations, sourcePath must start with [Operation, Parameter] (NOT including the DTO itself)
        // The DTO is implicit - it's the type of the Parameter
        // For Commands, sourcePath starts with [DTO]
        let rootSourcePath;
        if (rootSourceElement && rootSourceElement.specialization === "Operation") {
            // For Operations: find the DTO parameter and build path [Operation, Parameter]
            // The DTO is the type of the parameter, so we don't include it in the path
            const operationParams = rootSourceElement.getChildren("Parameter");
            const dtoParam = operationParams.find(p => { var _a; return ((_a = p.typeReference) === null || _a === void 0 ? void 0 : _a.typeId) === workingSourceElement.id; });
            if (dtoParam) {
                rootSourcePath = [rootSourceElement.id, dtoParam.id];
                console.log(`[BUILD-TREE] ├─ Operation mode: sourcePath = [Operation, Parameter]`);
            }
            else {
                // Fallback: just use DTO if parameter not found
                rootSourcePath = [workingSourceElement.id];
                console.log(`[BUILD-TREE] ├─ ⚠ Operation found but dto parameter not located, using DTO-only path`);
            }
        }
        else {
            // For Commands: standard path is just [DTO]
            rootSourcePath = [workingSourceElement.id];
        }
        const rootNode = {
            id: workingSourceElement.id,
            label: workingSourceElement.getName(),
            specializationId: "discrepancy",
            elementId: workingSourceElement.id,
            elementType: "DTO",
            originalName: workingSourceElement.getName(),
            originalType: undefined,
            hasDiscrepancies: true, // Set to true so it shows in tree
            discrepancy: undefined,
            isExpanded: true,
            isSelected: false,
            icon: workingSourceElement.getIcon(),
            children: [],
            elementParentId: (_a = workingSourceElement.getParent()) === null || _a === void 0 ? void 0 : _a.id,
            // Pre-computed paths: sourcePath includes Operation+Parameter for Operations
            sourcePath: rootSourcePath,
            targetPath: rootTargetEntity ? [rootTargetEntity.id] : [],
            targetEntityId: rootTargetEntity === null || rootTargetEntity === void 0 ? void 0 : rootTargetEntity.id
        };
        console.log(`[BUILD-TREE] Starting tree build for DTO: ${workingSourceElement.getName()}`);
        if (rootTargetEntity) {
            console.log(`[BUILD-TREE] ├─ Root entity: ${rootTargetEntity.getName()}`);
        }
        // If rootSourceElement is an Operation, show its parameters first
        if (rootSourceElement && rootSourceElement.specialization === "Operation") {
            const operationParams = rootSourceElement.getChildren("Parameter");
            console.log(`[BUILD-TREE] ├─ Operation parameters (${operationParams.length}):`);
            for (const param of operationParams) {
                const paramTypeRef = param.typeReference;
                const paramTypeName = (paramTypeRef === null || paramTypeRef === void 0 ? void 0 : paramTypeRef.display) || "Unknown";
                // Skip the DTO parameter itself - we'll show its fields instead
                if (paramTypeRef && paramTypeRef.isTypeFound()) {
                    const paramType = paramTypeRef.getType();
                    if (paramType && paramType.id === workingSourceElement.id) {
                        console.log(`[BUILD-TREE] │  ├─ (Skipping DTO parameter: ${param.getName()})`);
                        continue;
                    }
                }
                console.log(`[BUILD-TREE] │  ├─ ${param.getName()}: ${paramTypeName}`);
                const paramNode = {
                    id: param.id,
                    label: `${param.getName()}: ${paramTypeName}`,
                    specializationId: "discrepancy",
                    elementId: param.id,
                    elementType: "Operation-Parameter",
                    originalName: param.getName(),
                    originalType: paramTypeName,
                    hasDiscrepancies: true,
                    isExpanded: true,
                    isSelected: false,
                    icon: param.getIcon(),
                    children: [],
                    elementParentId: rootSourceElement.id,
                    // Params don't have paths yet - they're not DTO fields
                    sourcePath: undefined,
                    targetPath: undefined
                };
                rootNode.children.push(paramNode);
            }
        }
        // Add DTO fields (direct properties) - WITH pre-computed paths
        const dtoChildren = workingSourceElement.getChildren("DTO-Field");
        console.log(`[BUILD-TREE] ├─ DTO fields (${dtoChildren.length}):`);
        for (const dtoChild of dtoChildren) {
            const fieldTypeRef = dtoChild.typeReference;
            const fieldTypeName = (fieldTypeRef === null || fieldTypeRef === void 0 ? void 0 : fieldTypeRef.display) || "Unknown";
            console.log(`[BUILD-TREE] │  ├─ ${dtoChild.getName()}: ${fieldTypeName}`);
            // Extend sourcePath from root: [rootSourcePath..., fieldId]
            const sourcePath = [...rootSourcePath, dtoChild.id];
            // NEW: Pre-compute targetPath for root level
            let targetPath = [];
            let fieldTargetEntity;
            if (rootTargetEntity) {
                targetPath = [rootTargetEntity.id];
                fieldTargetEntity = rootTargetEntity;
                // For complex type fields, try to resolve the actual target entity via associations
                if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                    const fieldType = fieldTypeRef.getType();
                    if (fieldType && this.isComplexType(fieldType)) {
                        const assoc = this.findAssociationToEntity(rootTargetEntity, dtoChild);
                        if (assoc) {
                            console.log(`[BUILD-TREE] │  │  ├─ Found association for root field: ${assoc.getName ? assoc.getName() : assoc.id}`);
                            // Extend target path with the association
                            targetPath.push(assoc.id);
                            // Set target entity to the association's target
                            if (assoc.typeReference && assoc.typeReference.isTypeFound()) {
                                fieldTargetEntity = assoc.typeReference.getType();
                                console.log(`[BUILD-TREE] │  │  └─ Target entity: ${fieldTargetEntity === null || fieldTargetEntity === void 0 ? void 0 : fieldTargetEntity.getName()}`);
                            }
                        }
                        else {
                            console.log(`[BUILD-TREE] │  │  ├─ ⚠ No association found for root complex field`);
                        }
                    }
                }
            }
            const fieldNode = {
                id: dtoChild.id,
                label: `${dtoChild.getName()}: ${fieldTypeName}`,
                specializationId: "discrepancy",
                elementId: dtoChild.id,
                elementType: "DTO-Field",
                originalName: dtoChild.getName(),
                originalType: fieldTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: dtoChild.getIcon(),
                children: [],
                elementParentId: workingSourceElement.id,
                // NEW: Store pre-computed paths extending from root
                sourcePath: sourcePath,
                targetPath: targetPath,
                targetEntityId: fieldTargetEntity === null || fieldTargetEntity === void 0 ? void 0 : fieldTargetEntity.id
            };
            if (fieldTypeRef && fieldTypeRef.isTypeFound()) {
                const fieldType = fieldTypeRef.getType();
                if (fieldType && this.isComplexType(fieldType)) {
                    console.log(`[BUILD-TREE] │  │  ├─ Nested DTO: ${fieldType.getName()}`);
                    // Recursively add nested fields WITH path computation
                    this.addDtoFieldsRecursive(fieldNode, fieldType, 2, sourcePath, // Current source path (will extend this)
                    targetPath, // Current target path
                    fieldTargetEntity // Current target entity
                    );
                }
            }
            rootNode.children.push(fieldNode);
        }
        console.log(`[BUILD-TREE] └─ Tree build complete`);
        this.lastStructureTree = rootNode;
    }
    addDtoFieldsRecursive(parentNode, dtoElement, depth, parentSourcePath = [], parentTargetPath = [], parentTargetEntity) {
        const indent = Array(depth).fill("│  ").join("");
        const children = dtoElement.getChildren("DTO-Field");
        console.log(`[BUILD-TREE] ${indent}├─ Processing ${children.length} nested fields in ${dtoElement.getName()}`);
        for (const child of children) {
            const childTypeRef = child.typeReference;
            const childTypeName = (childTypeRef === null || childTypeRef === void 0 ? void 0 : childTypeRef.display) || "Unknown";
            console.log(`[BUILD-TREE] ${indent}│  ├─ ${child.getName()}: ${childTypeName}`);
            // NEW: Extend both paths in parallel
            const childSourcePath = [...parentSourcePath, child.id];
            let childTargetPath = [...parentTargetPath];
            let childTargetEntity = parentTargetEntity;
            // If this child is a complex type, try to find the corresponding entity
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const childType = childTypeRef.getType();
                if (childType && this.isComplexType(childType)) {
                    // Find the association from parentTargetEntity to the child's target entity
                    if (parentTargetEntity) {
                        const childAssoc = this.findAssociationToEntity(parentTargetEntity, child);
                        if (childAssoc) {
                            console.log(`[BUILD-TREE] ${indent}│  │  ├─ Found association: ${childAssoc.getName ? childAssoc.getName() : childAssoc.id}`);
                            // Extend target path with the association ID
                            childTargetPath.push(childAssoc.id);
                            // Get the target entity of this association
                            if (childAssoc.typeReference && childAssoc.typeReference.isTypeFound()) {
                                childTargetEntity = childAssoc.typeReference.getType();
                                console.log(`[BUILD-TREE] ${indent}│  │  └─ Target entity: ${childTargetEntity === null || childTargetEntity === void 0 ? void 0 : childTargetEntity.getName()}`);
                            }
                        }
                        else {
                            console.log(`[BUILD-TREE] ${indent}│  │  ├─ ⚠ No association found for nested DTO field`);
                        }
                    }
                }
            }
            const childNode = {
                id: child.id,
                label: TreeNodeLabelBuilder.buildFieldLabel(child.getName(), childTypeName),
                specializationId: "discrepancy",
                elementId: child.id,
                elementType: "Nested-Field",
                originalName: child.getName(),
                originalType: childTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: child.getIcon(),
                children: [],
                elementParentId: dtoElement.id,
                // NEW: Store pre-computed paths
                sourcePath: childSourcePath,
                targetPath: childTargetPath,
                targetEntityId: childTargetEntity === null || childTargetEntity === void 0 ? void 0 : childTargetEntity.id
            };
            console.log(`[BUILD-TREE] ${indent}│  ├─ Source path: [${childSourcePath.map(id => { var _a, _b; return ((_b = (_a = lookup(id)) === null || _a === void 0 ? void 0 : _a.getName) === null || _b === void 0 ? void 0 : _b.call(_a)) || id; }).join(' → ')}]`);
            console.log(`[BUILD-TREE] ${indent}│  └─ Target path: [${childTargetPath.map(id => { var _a, _b; return ((_b = (_a = lookup(id)) === null || _a === void 0 ? void 0 : _a.getName) === null || _b === void 0 ? void 0 : _b.call(_a)) || id; }).join(' → ')}]`);
            if (childTypeRef && childTypeRef.isTypeFound()) {
                const nestedType = childTypeRef.getType();
                if (nestedType && this.isComplexType(nestedType)) {
                    // Recursively add nested fields WITH updated paths
                    this.addDtoFieldsRecursive(childNode, nestedType, depth + 1, childSourcePath, // Pass extended source path
                    childTargetPath, // Pass extended target path
                    childTargetEntity // Pass the new target entity
                    );
                }
            }
            parentNode.children.push(childNode);
        }
    }
    /**
     * Find the association from parentEntity to a child DTO field
     * Match by field name and the DTO type it references
     */
    findAssociationToEntity(parentEntity, dtoField) {
        const fieldName = dtoField.getName();
        const fieldTypeRef = dtoField.typeReference;
        if (!fieldTypeRef || !fieldTypeRef.isTypeFound()) {
            return null;
        }
        const fieldType = fieldTypeRef.getType();
        if (!fieldType) {
            return null;
        }
        // Search for associations that match this field
        const associations = parentEntity.getAssociations();
        for (const assoc of associations) {
            const assocName = assoc.getName ? assoc.getName() : null;
            if (!assocName) {
                continue;
            }
            // Try to match by name (field name should match association name or singular form)
            const fieldNameSingular = singularize(fieldName);
            const nameMatches = assocName === fieldName || assocName === fieldNameSingular;
            if (!nameMatches) {
                continue;
            }
            // Verify the association points to an entity
            if (assoc.typeReference && assoc.typeReference.isTypeFound()) {
                const assocTarget = assoc.typeReference.getType();
                if (assocTarget && assocTarget.specialization === "Class") {
                    return assoc;
                }
            }
        }
        return null;
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
            if (disc.type === "ADD") {
                continue;
            }
            if (disc.sourceFieldId) {
                elementIdsWithDiscrepancies.add(disc.sourceFieldId);
                discrepancyByElementId.set(disc.sourceFieldId, disc);
            }
            if (disc.targetId) {
                elementIdsWithDiscrepancies.add(disc.targetId);
                discrepancyByElementId.set(disc.targetId, disc);
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
        var _a, _b;
        // Find NEW discrepancies that belong to this node (by parent DTO element ID)
        const newDiscrepanciesForThisNode = discrepancies.filter(d => d.type === "ADD" && d.sourceFieldId === node.elementId);
        if (newDiscrepanciesForThisNode.length > 0) {
            console.log(`[ANNOTATE] Adding ${newDiscrepanciesForThisNode.length} NEW nodes to ${node.originalName} (elementId: ${node.elementId})`);
        }
        // Add them as children
        for (const newDisc of newDiscrepanciesForThisNode) {
            console.log(`[ANNOTATE-NEW] Creating NEW node for: ${newDisc.targetAttributeName}`);
            console.log(`[ANNOTATE-NEW] ├─ parent node: ${node.originalName} (elementId: ${node.elementId})`);
            console.log(`[ANNOTATE-NEW] ├─ parent sourcePath: [${((_a = node.sourcePath) === null || _a === void 0 ? void 0 : _a.join(' → ')) || 'undefined'}]`);
            console.log(`[ANNOTATE-NEW] ├─ parent targetPath: [${((_b = node.targetPath) === null || _b === void 0 ? void 0 : _b.join(' → ')) || 'undefined'}]`);
            console.log(`[ANNOTATE-NEW] ├─ parent targetEntityId: ${node.targetEntityId}`);
            console.log(`[ANNOTATE-NEW] └─ newDisc.sourceParentId: ${newDisc.sourceParentId}`);
            const newNode = {
                id: newDisc.id,
                label: TreeNodeLabelBuilder.buildDiscrepancyLabel(newDisc),
                specializationId: `discrepancy`,
                elementId: newDisc.targetId,
                elementType: "NEW-Field",
                originalName: newDisc.targetAttributeName,
                originalType: newDisc.targetAttributeTypeName,
                hasDiscrepancies: true,
                isExpanded: true,
                isSelected: false,
                icon: newDisc.icon,
                discrepancy: newDisc,
                displayFunction: createDiscrepancyDisplayFunction(newDisc, newDisc.targetAttributeName),
                children: [],
                elementParentId: newDisc.sourceParentId,
                // NEW: Inherit paths from parent node so we have them when applying
                sourcePath: node.sourcePath,
                targetPath: node.targetPath,
                targetEntityId: node.targetEntityId
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
            // Format the node with display properties
            if (node.discrepancy) {
                const fieldName = node.discrepancy.type === "ADD"
                    ? node.discrepancy.targetAttributeName
                    : (node.discrepancy.sourceFieldName || node.discrepancy.targetAttributeName || "Unknown");
                node.label = TreeNodeLabelBuilder.buildDiscrepancyLabel(node.discrepancy);
                node.specializationId = `discrepancy`;
                node.displayFunction = createDiscrepancyDisplayFunction(node.discrepancy, fieldName);
            }
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
}
/**
 * Executes sync actions on selected tree nodes
 * Applies RENAME, DELETE, CHANGE_TYPE, and NEW field/association operations
 */
class NodeSyncExecutor {
    constructor(rootSourceElement, rootTargetEntity, workingSourceElement) {
        this.rootSourceElement = rootSourceElement;
        this.rootTargetEntity = rootTargetEntity;
        this.workingSourceElement = workingSourceElement;
    }
    execute(selectedNodes) {
        console.log(`[APPLY-SYNC] Starting to apply ${selectedNodes.length} sync actions`);
        for (const node of selectedNodes) {
            if (!node.discrepancy) {
                console.log(`[APPLY-SYNC] ⚠ Skipping node ${node.id} - no discrepancy metadata`);
                continue;
            }
            try {
                this.executeNode(node);
                console.log(`[APPLY-SYNC] ✓ Applied [${node.discrepancy.type}] ${node.discrepancy.sourceFieldName}`);
            }
            catch (error) {
                console.log(`[APPLY-SYNC] ✗ Failed [${node.discrepancy.type}] ${node.discrepancy.sourceFieldName}: ${error}`);
                // Continue to next node (no transactions)
            }
        }
        console.log(`[APPLY-SYNC] └─ Sync complete`);
    }
    executeNode(node) {
        var _a, _b;
        const discrepancy = node.discrepancy;
        console.log(`[EXECUTE-NODE] Processing node: ${node.id}`);
        console.log(`[EXECUTE-NODE] ├─ type: ${discrepancy === null || discrepancy === void 0 ? void 0 : discrepancy.type}`);
        console.log(`[EXECUTE-NODE] ├─ sourcePath: [${((_a = node.sourcePath) === null || _a === void 0 ? void 0 : _a.join(' → ')) || 'undefined'}]`);
        console.log(`[EXECUTE-NODE] ├─ targetPath: [${((_b = node.targetPath) === null || _b === void 0 ? void 0 : _b.join(' → ')) || 'undefined'}]`);
        console.log(`[EXECUTE-NODE] ├─ targetEntityId: ${node.targetEntityId}`);
        console.log(`[EXECUTE-NODE] └─ elementParentId: ${node.elementParentId}`);
        let targetDtoElement;
        if (node.elementParentId) {
            targetDtoElement = lookup(node.elementParentId);
        }
        else {
            throw new Error("No ElementParentId specified on node");
        }
        if (!targetDtoElement) {
            throw new Error(`Unable to find target DTO element: ${node.elementParentId}`);
        }
        switch (discrepancy.type) {
            case "RENAME":
                this.applyRename(node, targetDtoElement);
                break;
            case "DELETE":
                this.applyDelete(node, targetDtoElement);
                break;
            case "CHANGE_TYPE":
                this.applyChangeType(node, targetDtoElement);
                break;
            case "ADD":
                // Determine if it's an association or attribute based on targetIdType discriminator
                if (discrepancy.targetIdType === "association") {
                    this.applyNewAssociation(node, targetDtoElement);
                }
                else {
                    // For nested DTOs, resolve the correct entity
                    const targetEntity = this.resolveEntityForDto(targetDtoElement, this.rootTargetEntity);
                    this.applyNewAttribute(node, targetDtoElement, targetEntity);
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
        // For Operations, look for Parameter children; for DTOs, look for DTO-Field children
        const childType = dtoElement.specialization === "Operation" ? ELEMENT_TYPE_NAMES.PARAMETER : ELEMENT_TYPE_NAMES.DTO_FIELD;
        const children = dtoElement.getChildren(childType);
        const field = children.find(child => child.id === discrepancy.sourceFieldId);
        if (!field) {
            throw new Error(`${childType} not found: ${discrepancy.sourceFieldId}`);
        }
        // Rename the field to match entity attribute name
        field.setName(discrepancy.targetAttributeName, true);
        console.log(`[APPLY-SYNC-RENAME] Renamed field from '${discrepancy.sourceFieldName}' to '${discrepancy.targetAttributeName}'`);
    }
    /**
     * Apply DELETE: Remove the DTO field if it has no mappings
     */
    applyDelete(node, dtoElement) {
        const discrepancy = node.discrepancy;
        // For Operations, look for Parameter children; for DTOs, look for DTO-Field children
        const childType = dtoElement.specialization === "Operation" ? ELEMENT_TYPE_NAMES.PARAMETER : ELEMENT_TYPE_NAMES.DTO_FIELD;
        const children = dtoElement.getChildren(childType);
        const field = children.find(child => child.id === discrepancy.sourceFieldId);
        if (!field) {
            throw new Error(`${childType} not found: ${discrepancy.sourceFieldId}`);
        }
        // Delete the field from the DTO/Operation
        field.delete();
        console.log(`[APPLY-SYNC-DELETE] Deleted field '${discrepancy.sourceFieldName}'`);
    }
    /**
     * Apply CHANGE_TYPE: Update the field's type reference to match entity attribute type
     */
    applyChangeType(node, dtoElement) {
        const discrepancy = node.discrepancy;
        // For Operations, look for Parameter children; for DTOs, look for DTO-Field children
        const childType = dtoElement.specialization === "Operation" ? ELEMENT_TYPE_NAMES.PARAMETER : ELEMENT_TYPE_NAMES.DTO_FIELD;
        const children = dtoElement.getChildren(childType);
        const field = children.find(child => child.id === discrepancy.sourceFieldId);
        if (!field) {
            throw new Error(`${childType} not found: ${discrepancy.sourceFieldId}`);
        }
        const typeRef = field.typeReference;
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
    applyNewAttribute(node, dtoElement, entity) {
        var _a, _b;
        const discrepancy = node.discrepancy;
        console.log(`[APPLY-NEW] Processing NEW discrepancy: ${discrepancy.targetAttributeName}`);
        console.log(`[APPLY-NEW] ├─ node.sourcePath: [${((_a = node.sourcePath) === null || _a === void 0 ? void 0 : _a.join(' → ')) || 'undefined'}]`);
        console.log(`[APPLY-NEW] ├─ node.targetPath: [${((_b = node.targetPath) === null || _b === void 0 ? void 0 : _b.join(' → ')) || 'undefined'}]`);
        console.log(`[APPLY-NEW] ├─ node.targetEntityId: ${node.targetEntityId}`);
        console.log(`[APPLY-NEW] ├─ node.elementParentId: ${node.elementParentId}`);
        console.log(`[APPLY-NEW] ├─ dtoElement: ${dtoElement.getName()}`);
        console.log(`[APPLY-NEW] └─ entity: ${entity.getName()}`);
        // Create new field in the DTO
        let newField = dtoElement.addChild("DTO-Field", discrepancy.targetAttributeName);
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
        else {
            console.warn(`[APPLY-SYNC-NEW-ATTR] Warning: No target type ID for new field '${discrepancy.targetAttributeName}', leaving type unset`);
        }
        console.log(`[APPLY-SYNC-NEW-ATTR] Created new field '${discrepancy.targetAttributeName}: ${discrepancy.targetAttributeTypeName}'`);
        // Create mapping between the new field and the entity attribute
        try {
            // Find the actual entity that contains this attribute (may be different from fallback)
            if (!discrepancy.targetId) {
                throw new Error(`No target ID in discrepancy for ${discrepancy.targetAttributeName}`);
            }
            const actualEntity = this.findEntityContainingAttribute(discrepancy.targetId);
            if (!actualEntity) {
                throw new Error(`Could not find entity containing attribute: ${discrepancy.targetAttributeName}`);
            }
            // UPDATED: Use pre-computed paths from node and rootSourceElement for association lookup
            this.createFieldMappingWithPath(dtoElement, actualEntity, newField, discrepancy, node);
            console.log(`[APPLY-SYNC-NEW-ATTR] Created mapping for '${discrepancy.targetAttributeName}'`);
        }
        catch (error) {
            console.log(`[APPLY-SYNC-NEW-ATTR] Warning: Failed to create mapping: ${error}`);
            // Don't throw - field was created successfully even if mapping failed
        }
    }
    /**
     * Select the appropriate advanced mapping for "Data Mapping" type
     *
     * Prefers Create Entity Action, then Update Entity Action, skips Query/Delete actions
     * (those use "Filter Mapping", not "Data Mapping")
     *
     */
    selectMappingForDataMapping(allMappings) {
        const QUERY_ENTITY_UUID = "25f25af9-c38b-4053-9474-b0fabe9d7ea7";
        return allMappings.filter(m => m.mappingTypeId !== QUERY_ENTITY_UUID)[0] || null;
    }
    findRootAssociation(rootDto, rootEntity) {
        // For Commands/Queries, root association is directly on the root source element.
        let association = this.rootSourceElement
            .getAssociations()
            .find(a => { var _a; return ((_a = a.typeReference) === null || _a === void 0 ? void 0 : _a.typeId) === rootEntity.id; });
        if (association) {
            return association;
        }
        // For Operations, the association is on the DTO parameter.
        if (this.rootSourceElement.specialization !== "Operation") {
            return null;
        }
        const params = this.rootSourceElement.getChildren("Parameter");
        for (const param of params) {
            if (param.typeReference && param.typeReference.typeId === rootDto.id) {
                const paramAssociations = param.getAssociations();
                association = paramAssociations
                    .find(a => { var _a; return ((_a = a.typeReference) === null || _a === void 0 ? void 0 : _a.typeId) === rootEntity.id; });
                if (association) {
                    return association;
                }
            }
        }
        return null;
    }
    /**
     * Create a field-to-attribute mapping with OPTION A: pre-computed paths
     *
     * IMPORTANT: The node should already have sourcePath and targetPath pre-computed
     * during buildStructureTree. We use them directly here - NO calculation needed.
     *
     * Strategy:
     * 1. Get pre-computed sourcePath and targetPath from node
     * 2. Append the new field ID to sourcePath
     * 3. Append the target attribute ID to targetPath
     * 4. Find ROOT association (rootDto → rootEntity)
     * 5. Get/create SINGLE advanced mapping on that association
     * 6. Add mappedEnd with complete paths
     */
    createFieldMappingWithPath(dtoElement, entity, newField, discrepancy, node) {
        console.log(`[CREATE-MAPPING-V2] Starting mapping creation for field: ${newField.getName()}`);
        console.log(`[CREATE-MAPPING-V2] ├─ DTO element: ${dtoElement.getName()}`);
        console.log(`[CREATE-MAPPING-V2] ├─ Entity: ${entity.getName()}`);
        // Use pre-computed paths from node (should always be available)
        if (!node || !node.sourcePath || !node.targetPath || !node.targetEntityId) {
            throw new Error(`Node missing required pre-computed paths for ${newField.getName()}`);
        }
        console.log(`[CREATE-MAPPING-V2] ├─ Using PRE-COMPUTED paths from node`);
        // Start with the pre-computed paths from the node
        const sourcePath = [...node.sourcePath, newField.id];
        const targetPath = [...node.targetPath]; // Will append attribute ID below
        // IMPORTANT: Extract the actual DTO ID from sourcePath
        // For Operations: sourcePath = [Operation, Parameter, ...fields...]  → workingSourceElement is the DTO
        // For Commands: sourcePath = [DTO, ...fields...]  → DTO at index 0
        let rootDto;
        if (node.sourcePath.length === 2) {
            // Operations: [Operation, Parameter] - DTO is dtoElement (passed in)
            rootDto = dtoElement;
        }
        else if (node.sourcePath.length === 1) {
            // Commands: [DTO] at index 0
            rootDto = lookup(node.sourcePath[0]) || dtoElement;
        }
        else {
            // Nested case - just use dtoElement
            rootDto = dtoElement;
        }
        const rootEntityId = node.targetPath[0];
        const rootEntity = lookup(rootEntityId);
        if (!rootEntity) {
            throw new Error(`Could not find root entity from targetPath[0]: ${rootEntityId}`);
        }
        // Append the target attribute ID
        if (discrepancy.targetId) {
            targetPath.push(discrepancy.targetId);
            console.log(`[CREATE-MAPPING-V2] ├─ Appended target attribute: ${discrepancy.targetAttributeName}`);
        }
        else {
            throw new Error(`No target ID in discrepancy for ${discrepancy.targetAttributeName}`);
        }
        console.log(`[CREATE-MAPPING-V2] ├─ Final source path (${sourcePath.length} IDs): [${sourcePath.map(id => { var _a, _b; return ((_b = (_a = lookup(id)) === null || _a === void 0 ? void 0 : _a.getName) === null || _b === void 0 ? void 0 : _b.call(_a)) || id; }).join(' → ')}]`);
        console.log(`[CREATE-MAPPING-V2] ├─ Final target path (${targetPath.length} IDs): [${targetPath.map(id => { var _a, _b; return ((_b = (_a = lookup(id)) === null || _a === void 0 ? void 0 : _a.getName) === null || _b === void 0 ? void 0 : _b.call(_a)) || id; }).join(' → ')}]`);
        console.log(`[CREATE-MAPPING-V2] ├─ Root DTO: ${rootDto.getName()}`);
        console.log(`[CREATE-MAPPING-V2] ├─ Root Entity: ${rootEntity.getName()}`);
        // Step 2: Find the ROOT association (root source → root entity)
        const association = this.findRootAssociation(rootDto, rootEntity);
        if (!association) {
            throw new Error(`No root association found between ${rootDto.getName()} and ${rootEntity.getName()}`);
        }
        // Step 3: Get advanced mappings on the root association
        const assocApi = association;
        const allMappings = assocApi.getAdvancedMappings();
        if (!allMappings || allMappings.length === 0) {
            console.log(`[CREATE-MAPPING-V2] ├─ ⚠ No advanced mappings found (FAIL-FAST)`);
            throw new Error(`No advanced mapping found on association between ${rootDto.getName()} and ${rootEntity.getName()}`);
        }
        console.log(`[CREATE-MAPPING-V2] ├─ Found ${allMappings.length} advanced mapping(s):`);
        const mappingDetails = allMappings.map((m, idx) => {
            return `Mapping[${idx}]: ${m.mappingTypeId} - ${m.mappingType}`;
        });
        mappingDetails.forEach(detail => console.log(`[CREATE-MAPPING-V2] │  ├─ ${detail}`));
        // Select the appropriate mapping for "Data Mapping" type
        // Preference: Create Entity Action > Update Entity Action > first available
        // "Data Mapping" is valid only for Create/Update actions, not Query/Delete actions
        let mapping = this.selectMappingForDataMapping(allMappings);
        if (!mapping) {
            console.log(`[CREATE-MAPPING-V2] ├─ ⚠ No suitable advanced mapping found for Data Mapping (all mappings may be Query/Filter type)`);
            throw new Error(`No Create/Update Entity mapping found on association between ${rootDto.getName()} and ${rootEntity.getName()}`);
        }
        console.log(`[CREATE-MAPPING-V2] ├─ ✓ Selected mapping for Data Mapping: ${mapping.mappingTypeId} - ${mapping.mappingType}`);
        // Step 4: Add the mappedEnd with complete paths
        console.log(`[CREATE-MAPPING-V2] ├─ Adding mappedEnd...`);
        console.log(`[CREATE-MAPPING-V2] │  ├─ Mapping type: Data Mapping`);
        console.log(`[CREATE-MAPPING-V2] │  ├─ Source path (${sourcePath.length} IDs): ${sourcePath.join(' → ')}`);
        console.log(`[CREATE-MAPPING-V2] │  └─ Target path (${targetPath.length} IDs): ${targetPath.join(' → ')}`);
        mapping.addMappedEnd("Data Mapping", sourcePath, targetPath);
        console.log(`[CREATE-MAPPING-V2] └─ ✓ MappedEnd added successfully`);
    }
    /**
     * Resolve the entity for a given DTO element
     * For nested DTOs, find the entity via the DTO's association
     */
    resolveEntityForDto(dtoElement, fallbackEntity) {
        // Check if this DTO has any associations that point to an entity
        const associations = dtoElement.getAssociations();
        for (const assoc of associations) {
            if (assoc.typeReference && assoc.typeReference.isTypeFound()) {
                const targetElement = assoc.typeReference.getType();
                if (targetElement && targetElement.specialization === "Class") {
                    console.log(`[RESOLVE-ENTITY] Resolved entity for ${dtoElement.getName()}: ${targetElement.getName()}`);
                    return targetElement;
                }
            }
        }
        // If no association found, use fallback
        console.log(`[RESOLVE-ENTITY] No entity found for ${dtoElement.getName()}, using fallback: ${fallbackEntity.getName()}`);
        return fallbackEntity;
    }
    /**
     * Find which entity contains a given attribute ID
     * Searches from the provided entity and its related entities
     */
    findEntityContainingAttribute(attributeId) {
        const attr = lookup(attributeId);
        if (!attr) {
            return null;
        }
        return attr.getParent();
    }
    /**
     * Apply NEW ASSOCIATION: Create a new nested DTO and add mapping
     */
    applyNewAssociation(node, dtoElement) {
        const discrepancy = node.discrepancy;
        // Use the pre-computed suggested DTO name and collection flag from discrepancy
        const dtoTypeName = discrepancy.suggestedDtoName;
        const isCollection = discrepancy.sourceIsCollection || false;
        // Find the association to get the target entity
        const associationId = discrepancy.targetId;
        const association = lookup(associationId);
        if (!association || !association.typeReference || !association.typeReference.isTypeFound()) {
            throw new Error(`Cannot find association or its target entity: ${associationId}`);
        }
        const targetEntity = association.typeReference.getType();
        console.log(`[APPLY-SYNC-NEW-ASSOC] Target entity: ${targetEntity.getName()}`);
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
        // Populate the new DTO with fields from the target entity
        const entityAttrs = getEntityAttributes(targetEntity);
        let fieldsAdded = 0;
        for (const attr of entityAttrs) {
            // Skip non-mappable attributes (auto-generated PKs, required parent FKs, infrastructure fields)
            if (!attr.isMappable) {
                continue;
            }
            const newField = newNestedDto.addChild("DTO-Field", attr.name);
            if (newField && newField.typeReference && attr.typeId) {
                newField.typeReference.setType(attr.typeId);
                if (attr.isCollection !== undefined) {
                    newField.typeReference.setIsCollection(attr.isCollection);
                }
                if (attr.isNullable !== undefined) {
                    newField.typeReference.setIsNullable(attr.isNullable);
                }
                fieldsAdded++;
            }
        }
        console.log(`[APPLY-SYNC-NEW-ASSOC] Populated ${fieldsAdded} fields in new DTO from entity ${targetEntity.getName()}`);
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
        // Create mappings for all fields in the new DTO
        this.createMappingsForNewDto(dtoElement, newNestedDto, newField, targetEntity, association, node, this.rootSourceElement);
    }
    createMappingsForNewDto(parentDto, newDto, dtoField, targetEntity, association, node, rootSourceElement) {
        var _a;
        console.log(`[CREATE-NEW-DTO-MAPPINGS] Creating mappings for new DTO: ${newDto.getName()}`);
        // Use pre-computed paths from node
        if (!node.sourcePath || !node.targetPath) {
            console.log(`[CREATE-NEW-DTO-MAPPINGS] ⚠ Node missing path information`);
            return;
        }
        // 1. Identify Root Elements using Node Paths
        // Extract the actual DTO ID from sourcePath
        // For Operations: sourcePath = [Operation, Parameter, ...fields...]  → DTO is parentDto (root DTO)
        // For Commands: sourcePath = [DTO, ...fields...]  → DTO at index 0
        let rootDto;
        if (node.sourcePath.length === 2) {
            // Operations: [Operation, Parameter] - DTO is parentDto
            rootDto = parentDto;
        }
        else if (node.sourcePath.length === 1) {
            // Commands: [DTO] at index 0
            rootDto = lookup(node.sourcePath[0]) || parentDto;
        }
        else {
            // Nested case - use parentDto
            rootDto = parentDto;
        }
        // For the entity side, targetPath[0] is the root entity
        const rootEntityId = node.targetPath[0];
        const rootEntity = lookup(rootEntityId);
        if (!rootEntity) {
            console.log(`[CREATE-NEW-DTO-MAPPINGS] ⚠ Could not find root entity: ${rootEntityId}`);
            return;
        }
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Using paths from node`);
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Root DTO: ${rootDto.getName()}`);
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Root Entity: ${rootEntity.getName()}`);
        // 2. Find Root Association
        const rootAssociation = this.findRootAssociation(rootDto, rootEntity);
        if (!rootAssociation) {
            console.log(`[CREATE-NEW-DTO-MAPPINGS] ⚠ Could not find root association between ${rootDto.getName()} and ${rootEntity.getName()}`);
            return;
        }
        // 3. Get Advanced Mapping - select the right one for "Data Mapping"
        const rootAssocApi = rootAssociation;
        const advancedMappings = ((_a = rootAssocApi.getAdvancedMappings) === null || _a === void 0 ? void 0 : _a.call(rootAssocApi)) || [];
        if (advancedMappings.length === 0) {
            console.log(`[CREATE-NEW-DTO-MAPPINGS] ⚠ No advanced mappings found on root association`);
            return;
        }
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Found ${advancedMappings.length} advanced mapping(s)`);
        // Select the appropriate mapping for "Data Mapping" type
        const advancedMapping = this.selectMappingForDataMapping(advancedMappings);
        if (!advancedMapping) {
            console.log(`[CREATE-NEW-DTO-MAPPINGS] ⚠ No Create/Update Entity mapping found (all mappings may be Query/Filter type)`);
            return;
        }
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Selected mapping for Data Mapping`);
        // Debug: Check existing mapped ends
        const existingEnds = advancedMapping.getMappedEnds ? advancedMapping.getMappedEnds() : [];
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Existing mapped ends count: ${existingEnds.length}`);
        // 4. Iterate and Map Fields
        const dtoFields = newDto.getChildren("DTO-Field");
        const entityAttrs = getEntityAttributes(targetEntity);
        let mappingsAdded = 0;
        // Base paths construction:
        // Paths are already pre-computed correctly in node (including Operation+Parameter for Operations)
        // Just extend with the new DTO
        const sourceBasePath = [...node.sourcePath, newDto.id];
        // Target: [...ParentTargetPath, AssociationId]
        const targetBasePath = [...node.targetPath, association.id];
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Source Base Path: ${sourceBasePath.join(' -> ')}`);
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Target Base Path: ${targetBasePath.join(' -> ')}`);
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ New DTO ID: ${newDto.id}`);
        // Debug: Check if advancedMapping is valid
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Advanced mapping object type: ${typeof advancedMapping}`);
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Advanced mapping has addMappedEnd: ${typeof advancedMapping.addMappedEnd === 'function'}`);
        // 5. Map the fields in the new DTO
        console.log(`[CREATE-NEW-DTO-MAPPINGS] ├─ Processing ${dtoFields.length} fields in new DTO`);
        for (const field of dtoFields) {
            const matchingAttr = entityAttrs.find(attr => namesAreEquivalent(attr.name, field.getName()));
            if (matchingAttr) {
                // Full paths: Base + Field/Attribute ID
                const sourcePath = [...sourceBasePath, field.id];
                const targetPath = [...targetBasePath, matchingAttr.id];
                try {
                    console.log(`[CREATE-NEW-DTO-MAPPINGS] │  ├─ Adding mapping for field '${field.getName()}'`);
                    console.log(`[CREATE-NEW-DTO-MAPPINGS] │  │  ├─ Source path (${sourcePath.length} IDs): ${sourcePath.join(' → ')}`);
                    console.log(`[CREATE-NEW-DTO-MAPPINGS] │  │  ├─ Target path (${targetPath.length} IDs): ${targetPath.join(' → ')}`);
                    advancedMapping.addMappedEnd("Data Mapping", sourcePath, targetPath);
                    const afterCount = advancedMapping.getMappedEnds ? advancedMapping.getMappedEnds().length : 'unknown';
                    console.log(`[CREATE-NEW-DTO-MAPPINGS] │  │  └─ Mapped ends count after call: ${afterCount}`);
                    mappingsAdded++;
                    console.log(`[CREATE-NEW-DTO-MAPPINGS] │  └─ ✓ Mapping added for ${field.getName()}`);
                }
                catch (e) {
                    console.log(`[CREATE-NEW-DTO-MAPPINGS] │  └─ ✗ Error: ${e}`);
                }
            }
        }
        console.log(`[CREATE-NEW-DTO-MAPPINGS] └─ Completed: Added ${mappingsAdded} mappings`);
        // Debug: Check mapped ends AFTER addition
        const finalEnds = advancedMapping.getMappedEnds ? advancedMapping.getMappedEnds() : [];
        console.log(`[CREATE-NEW-DTO-MAPPINGS] └─ Final mapped ends count: ${finalEnds.length}`);
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
    // Extract working source element from user's selection
    // For Commands/Queries: the DTO is extracted from the element itself
    // For Service Operations: the DTO parameter becomes the working source
    const workingSourceElement = extractDtoFromElement(element);
    if (!workingSourceElement) {
        throw new Error(`Unable to find DTO: For Operations, a DTO parameter must be present. The operation '${element.getName()}' does not have a DTO parameter.`);
    }
    console.log(`[SYNC] ├─ Working source element: ${workingSourceElement.getName()}`);
    // The root source element is always the element the user clicked
    // This is where associations are located (either Command or Operation)
    const rootSourceElement = element;
    console.log(`[SYNC] ├─ Root source element: ${rootSourceElement.getName()} (${rootSourceElement.specialization})`);
    // Find action associations on the root source element
    const associations = findAssociationsPointingToElement(rootSourceElement, workingSourceElement);
    // Extract root target entity from associations
    const rootTargetEntity = getEntityFromAssociations(associations);
    // If no associations found, throw error
    if (!rootTargetEntity) {
        throw new Error(`No entity mappings found: The '${workingSourceElement.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`);
    }
    console.log(`[SYNC] ├─ Root target entity: ${rootTargetEntity.getName()}`);
    console.log(`[SYNC] ├─ Associations: ${associations.length}`);
    // Extract field mappings from associations
    const fieldMappings = extractFieldMappings(associations);
    console.log(`[SYNC] ├─ Field mappings: ${fieldMappings.length}`);
    // Analyze discrepancies using new architecture with explicit context
    // rootSourceElement: where associations are (Command or Operation)
    // rootTargetEntity: starting entity for mapping
    // workingSourceElement: where new DTO fields are created
    const engine = new FieldSyncEngine();
    const discrepancies = engine.analyzeFieldDiscrepancies(rootSourceElement, rootTargetEntity, workingSourceElement, fieldMappings);
    console.log(`[SYNC] ├─ Discrepancies found: ${discrepancies.length}`);
    // Build tree view model with hierarchical structure
    const treeNodes = engine.buildHierarchicalTreeNodes();
    console.log(`[SYNC] ├─ Tree nodes built: ${treeNodes.length}`);
    // Build node map for quick lookup after dialog (preserves discrepancy metadata)
    const nodeMap = buildNodeMap(treeNodes);
    console.log(`[SYNC] ├─ Node map built with ${nodeMap.size} entries`);
    // Present dialog with results
    const selectedNodeIds = await presentSyncDialog(rootSourceElement, workingSourceElement, rootTargetEntity, treeNodes);
    console.log(`[SYNC] ├─ Selected nodes: ${selectedNodeIds.length}`);
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        // Find the selected tree nodes using the preserved map
        const selectedNodes = findSelectedNodes(nodeMap, selectedNodeIds);
        console.log(`[SYNC] └─ Applying ${selectedNodes.length} sync actions`);
        engine.applySyncActions(rootSourceElement, rootTargetEntity, workingSourceElement, selectedNodes);
    }
    else {
        console.log(`[SYNC] └─ No discrepancies selected`);
    }
}
async function presentSyncDialog(rootSourceElement, workingSourceElement, rootTargetEntity, treeNodes) {
    // Determine root display based on root source element type
    // If it's an Operation, show the Operation; otherwise show the working source (Command/DTO)
    const rootElement = rootSourceElement.specialization === "Operation" ? rootSourceElement : workingSourceElement;
    const rootLabel = rootSourceElement.specialization === "Operation"
        ? `${rootSourceElement.getName()} (${rootSourceElement.specialization})`
        : workingSourceElement.getName();
    console.log(`[DIALOG] rootSourceElement: ${JSON.stringify(rootSourceElement)}`);
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
        title: `Synchronize ${rootSourceElement.getName()} with ${rootTargetEntity.getName()}`,
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
                        specializationId: "discrepancy",
                        children: treeNodes,
                        isExpanded: true,
                        isSelected: false,
                        icon: rootElement.getIcon()
                    },
                    height: "400px",
                    isMultiSelect: true,
                    selectableTypes: [
                        {
                            specializationId: "discrepancy",
                            isSelectable: true,
                            autoExpand: true,
                            autoSelectChildren: true
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
