/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="sync-dto-fields-engine.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

async function syncDtoFields(element: MacroApi.Context.IElementApi): Promise<void> {
    try {
        // Validate element
        if (!isValidSyncElement(element)) {
            await dialogService.error(
                `Invalid element type.\n\nThe selected element must be a DTO, Command, or Query. The current element is a '${element.specialization}'.`
            );
            return;
        }
        
        // Find action associations
        const associations = findAssociationsPointingToElement(element);
        
        // Try to get entity from associations
        let entity = getEntityFromAssociations(associations);
        
        // If no associations found, ask user to select entity
        if (!entity) {
            // For now, show error - later we could add dialog for entity selection
            await dialogService.warn(
                `No entity mappings found.\n\nThe '${element.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`
            );
            return;
        }
        
        // Extract field mappings from DTO fields
        const fieldMappings = extractFieldMappings(element);
        
        // Analyze discrepancies
        const engine = new FieldSyncEngine();
        const discrepancies = engine.analyzeFieldDiscrepancies(element, entity, fieldMappings);
        
        if (discrepancies.length === 0) {
            await dialogService.info(
                `All fields are synchronized.\n\nThe DTO fields are properly synchronized with the entity '${entity.getName()}' attributes.`
            );
            return;
        }
        
        // Build tree view model
        const treeNodes = engine.buildTreeNodes(discrepancies);
        
        // Present dialog with results
        await presentSyncDialog(element, entity, discrepancies, treeNodes);
        
    } catch (error) {
        await dialogService.error(`An error occurred: ${error}`);
    }
}

async function presentSyncDialog(
    dtoElement: MacroApi.Context.IElementApi,
    entity: MacroApi.Context.IElementApi,
    discrepancies: IFieldDiscrepancy[],
    treeNodes: MacroApi.Context.ISelectableTreeNode[]
): Promise<void> {
    const config: MacroApi.Context.IDynamicFormConfig = {
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
