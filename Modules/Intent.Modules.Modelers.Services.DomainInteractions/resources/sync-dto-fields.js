/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
function isValidSyncElement(element) {
    const validSpecializations = ["DTO", "Command", "Query"];
    return validSpecializations.includes(element.specialization);
}
function getValidSpecializations() {
    return ["DTO", "Command", "Query"];
}
function findAssociationsPointingToElement(dtoElement) {
    const package_ = dtoElement.getPackage();
    if (!package_)
        return [];
    const allAssociations = [];
    const actionNames = ["Create Entity Action", "Update Entity Action", "Delete Entity Action", "Query Entity Action"];
    // Get all associations from the element  
    for (const actionName of actionNames) {
        const results = dtoElement.getAssociations(actionName);
        if (results && results.length > 0) {
            allAssociations.push(...results);
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
    const fields = [];
    const children = dtoElement.getChildren("DTO-Field");
    for (const child of children) {
        const field = {
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
function getEntityAttributes(entity) {
    const attributes = [];
    const children = entity.getChildren("Attribute");
    for (const child of children) {
        const attribute = {
            id: child.id,
            name: child.getName(),
            typeId: child.typeReference?.getTypeId(),
            typeDisplayText: child.typeReference?.display || ""
        };
        attributes.push(attribute);
    }
    return attributes;
}
function extractFieldMappings(dtoElement) {
    const mappings = [];
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
/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />
function formatDiscrepancy(discrepancy) {
    const components = [];
    // Status badge with color and icon
    const statusInfo = getDiscrepancyStatusInfo(discrepancy.type);
    components.push({
        text: `[${discrepancy.type}]`,
        cssClass: `text-highlight ${statusInfo.cssClass}`,
        color: statusInfo.color
    });
    components.push({
        text: " ",
        cssClass: ""
    });
    // DTO field name
    components.push({
        text: discrepancy.dtoFieldName,
        cssClass: "text-highlight keyword"
    });
    // Separator and arrow
    components.push({
        text: " → ",
        cssClass: "text-highlight muted"
    });
    // Entity attribute name
    components.push({
        text: discrepancy.entityAttributeName,
        cssClass: "text-highlight typeref"
    });
    // Type information if applicable
    if (discrepancy.dtoFieldType || discrepancy.entityAttributeType) {
        components.push({
            text: " ",
            cssClass: ""
        });
        if (discrepancy.type === "TYPE_CHANGED") {
            components.push({
                text: `(${discrepancy.dtoFieldType} → ${discrepancy.entityAttributeType})`,
                cssClass: "text-highlight annotation"
            });
        }
        else if (discrepancy.dtoFieldType) {
            components.push({
                text: `(${discrepancy.dtoFieldType})`,
                cssClass: "text-highlight muted"
            });
        }
    }
    // Reason if provided
    if (discrepancy.reason) {
        components.push({
            text: " - ",
            cssClass: "text-highlight muted"
        });
        components.push({
            text: discrepancy.reason,
            cssClass: "text-highlight muted"
        });
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
        for (const mapping of mappings) {
            mappedDtoToEntity.set(mapping.sourceFieldId, mapping.targetAttributeId);
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
                    reason: "Field exists in DTO but has no entity mapping"
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            }
        }
        // Check for NEW, RENAMED, and TYPE_CHANGED
        for (const entityAttr of entityAttributes) {
            let found = false;
            let mappedDtoField;
            // Find if this entity attribute is mapped from any DTO field
            for (const dtoField of dtoFields) {
                if (dtoField.isMapped && mappedDtoToEntity.get(dtoField.id) === entityAttr.id) {
                    found = true;
                    mappedDtoField = dtoField;
                    break;
                }
            }
            if (!found) {
                // NEW: Entity attribute not in DTO
                const discrepancy = {
                    id: `new-${entityAttr.id}`,
                    type: "NEW",
                    dtoFieldId: undefined,
                    dtoFieldName: "[Missing]",
                    entityAttributeId: entityAttr.id,
                    entityAttributeName: entityAttr.name,
                    entityAttributeType: entityAttr.typeDisplayText,
                    reason: "Entity attribute does not exist in DTO"
                };
                discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                discrepancies.push(discrepancy);
            }
            else if (mappedDtoField) {
                // Found a mapping - check for RENAMED and TYPE_CHANGED
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
                        reason: "Field name differs from entity attribute name"
                    };
                    discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                    discrepancies.push(discrepancy);
                }
                // TYPE_CHANGED check
                if (mappedDtoField.typeId !== entityAttr.typeId &&
                    mappedDtoField.typeDisplayText !== entityAttr.typeDisplayText) {
                    const discrepancy = {
                        id: `type-changed-${mappedDtoField.id}`,
                        type: "TYPE_CHANGED",
                        dtoFieldId: mappedDtoField.id,
                        dtoFieldName: mappedDtoField.name,
                        dtoFieldType: mappedDtoField.typeDisplayText,
                        entityAttributeId: entityAttr.id,
                        entityAttributeName: entityAttr.name,
                        entityAttributeType: entityAttr.typeDisplayText,
                        reason: "Field type differs from entity attribute type"
                    };
                    discrepancy.displayFunction = createDiscrepancyDisplayFunction(discrepancy);
                    discrepancies.push(discrepancy);
                }
            }
        }
        return discrepancies;
    }
    buildTreeNodes(discrepancies) {
        const nodes = [];
        for (const discrepancy of discrepancies) {
            const node = {
                id: discrepancy.id,
                label: discrepancy.dtoFieldName,
                specializationId: "sync-field-discrepancy",
                isExpanded: true,
                isSelected: false
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
    try {
        // Validate element
        if (!isValidSyncElement(element)) {
            await dialogService.error(`Invalid element type.\n\nThe selected element must be a DTO, Command, or Query. The current element is a '${element.specialization}'.`);
            return;
        }
        // Find action associations
        const associations = findAssociationsPointingToElement(element);
        // Try to get entity from associations
        let entity = getEntityFromAssociations(associations);
        // If no associations found, ask user to select entity
        if (!entity) {
            // For now, show error - later we could add dialog for entity selection
            await dialogService.warn(`No entity mappings found.\n\nThe '${element.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`);
            return;
        }
        // Extract field mappings from DTO fields
        const fieldMappings = extractFieldMappings(element);
        // Analyze discrepancies
        const engine = new FieldSyncEngine();
        const discrepancies = engine.analyzeFieldDiscrepancies(element, entity, fieldMappings);
        if (discrepancies.length === 0) {
            await dialogService.info(`All fields are synchronized.\n\nThe DTO fields are properly synchronized with the entity '${entity.getName()}' attributes.`);
            return;
        }
        // Build tree view model
        const treeNodes = engine.buildTreeNodes(discrepancies);
        // Present dialog with results
        await presentSyncDialog(element, entity, discrepancies, treeNodes);
    }
    catch (error) {
        await dialogService.error(`An error occurred: ${error}`);
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
                        label: `DTO: ${dtoElement.getName()}`,
                        specializationId: "dto-sync-root",
                        children: treeNodes,
                        isExpanded: true,
                        isSelected: false
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
