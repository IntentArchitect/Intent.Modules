/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
function isValidSyncElement(element) {
    const validSpecializations = ["DTO", "Command", "Query", "Operation"];
    return validSpecializations.includes(element.specialization);
}
function getValidSpecializations() {
    return ["DTO", "Command", "Query", "Operation"];
}
function extractDtoFromElement(element) {
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
                const type = typeRef.getType();
                if (["DTO", "Command", "Query"].includes(type.specialization)) {
                    return type;
                }
            }
        }
    }
    return null;
}
function findAssociationsPointingToElement(searchElement, dtoElement) {
    const allAssociations = [];
    const actionNames = ["Create Entity Action", "Update Entity Action", "Delete Entity Action", "Query Entity Action"];
    // First try to get associations from searchElement
    for (const actionName of actionNames) {
        try {
            const results = searchElement.getAssociations(actionName);
            if (results && results.length > 0) {
                // For Operations, don't filter - all associations from the operation are valid
                // The DTO mapping is in the mapping source paths
                if (searchElement.specialization === "Operation") {
                    allAssociations.push(...results);
                }
                else {
                    // For DTOs/Commands/Queries, filter to associations that reference this element
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
        }
        catch (e) {
            // Continue to next action type
        }
    }
    // If no associations found and searchElement is a DTO/Command/Query, walk up the hierarchy
    if (allAssociations.length === 0 && ["DTO", "Command", "Query"].includes(searchElement.specialization)) {
        let current = searchElement;
        let depth = 0;
        while (current && allAssociations.length === 0 && depth < 10) {
            // Try all action types on current element
            for (const actionName of actionNames) {
                try {
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
    // Get the typeReference from the first association
    // The association's typeReference points to the entity
    const association = associations[0];
    if (association.typeReference && association.typeReference.isTypeFound()) {
        return association.typeReference.getType();
    }
    return null;
}
function getDtoFields(dtoElement) {
    var _a, _b;
    const fields = [];
    const children = dtoElement.getChildren("DTO-Field");
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
    const children = entity.getChildren("Attribute");
    for (const child of children) {
        const attribute = {
            id: child.id,
            name: child.getName(),
            typeId: (_a = child.typeReference) === null || _a === void 0 ? void 0 : _a.getTypeId(),
            typeDisplayText: ((_b = child.typeReference) === null || _b === void 0 ? void 0 : _b.display) || "",
            icon: child.getIcon(),
            isManagedKey: child.hasMetadata("is-managed-key") && child.getMetadata("is-managed-key") === "true"
        };
        attributes.push(attribute);
    }
    return attributes;
}
function extractFieldMappings(associations) {
    const mappings = [];
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
        }
        catch (error) {
            // Skip associations without advanced mappings
            continue;
        }
    }
    return mappings;
}
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
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
            components.push({ text: "[NEW]", color: statusInfo.color });
            break;
        case "RENAMED":
            // CurrentName → EntityName: type [RENAMED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: "[RENAMED]", color: statusInfo.color });
            break;
        case "TYPE_CHANGED":
            // FieldName: currentType → entityType [TYPE CHANGED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: "[TYPE CHANGED]", color: statusInfo.color });
            break;
        case "DELETED":
            // FieldName: type [DELETED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: "[DELETED]", color: statusInfo.color });
            break;
    }
    return components;
}
function getDiscrepancyStatusInfo(type) {
    switch (type) {
        case "NEW":
            return { color: "#22c55e", cssClass: "keyword" }; // Green
        case "DELETED":
            return { color: "#ef4444", cssClass: "typeref" }; // Red
        case "RENAMED":
            return { color: "#007777", cssClass: "annotation" }; // Teal
        case "TYPE_CHANGED":
            return { color: "#f97316", cssClass: "muted" }; // Orange
        default:
            return { color: "#6b7280", cssClass: "" }; // Gray
    }
}
function createDiscrepancyDisplayFunction(discrepancy) {
    return (context) => formatDiscrepancy(discrepancy);
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
                icon: discrepancy.icon
            };
            // Add display properties dynamically (not part of ISelectableTreeNode type)
            node.displayFunction = discrepancy.displayFunction;
            node.displayMetadata = {
                type: discrepancy.type,
                dtoFieldName: discrepancy.dtoFieldName,
                dtoFieldType: discrepancy.dtoFieldType,
                entityAttributeName: discrepancy.entityAttributeName,
                entityAttributeType: discrepancy.entityAttributeType,
                reason: discrepancy.reason
            };
            nodes.push(node);
        }
        return nodes;
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
    await presentSyncDialog(dtoElement, entity, discrepancies, treeNodes);
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
    await dialogService.openForm(config);
}
