/// <reference path="types.ts" />
/// <reference path="common.ts" />
/// <reference path="sync-dto-fields-engine.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

async function syncDtoFields(element: MacroApi.Context.IElementApi): Promise<void> {
    console.log(`[syncDtoFields] Called with element: ${element.getName()}, specialization: ${element.specialization}`);
    
    // Validate element
    if (!isValidSyncElement(element)) {
        await dialogService.error(
            `Invalid element type.\n\nThe selected element must be a DTO, Command, Query, or Service Operation. The current element is a '${element.specialization}'.`
        );
        return;
    }
    
    // Extract DTO from element (handles both direct DTO/Command/Query and Operation with DTO parameter)
    const dtoElement = extractDtoFromElement(element);
    if (!dtoElement) {
        await dialogService.error(
            `Unable to find DTO.\n\nFor Operations, a DTO parameter must be present. The operation '${element.getName()}' does not have a DTO parameter.`
        );
        return;
    }
    
    console.log(`[syncDtoFields] DTO Element: ${dtoElement.getName()}`);
    
    // Find action associations - use the original element if it's an Operation, otherwise use the DTO
    const elementToSearchForAssociations = element.specialization === "Operation" ? element : dtoElement;
    const associations = findAssociationsPointingToElement(elementToSearchForAssociations, dtoElement);
    
    console.log(`[syncDtoFields] Associations found: ${associations.length}`);
    
    // Try to get entity from associations
    let entity = getEntityFromAssociations(associations);
    
    // If no associations found, ask user to select entity
    if (!entity) {
        await dialogService.warn(
            `No entity mappings found.\n\nThe '${dtoElement.getName()}' element does not have any associated entity actions (Create, Update, Delete, or Query Entity Actions).`
        );
        return;
    }
    
    console.log(`[syncDtoFields] Entity: ${entity.getName()}`);
    
    // Extract field mappings from associations (not from DTO fields directly)
    const fieldMappings = extractFieldMappings(associations);
    
    // Extract parameter mappings for Operations
    let parameterMappings = new Map<string, string>();
    if (element.specialization === "Operation") {
        parameterMappings = extractParameterMappings(associations);
    }
    
    // Analyze discrepancies
    const engine = new FieldSyncEngine();
    const fieldDiscrepancies = engine.analyzeFieldDiscrepancies(dtoElement, entity, fieldMappings);
    console.log(`[syncDtoFields] Field discrepancies: ${fieldDiscrepancies.length}`);
    fieldDiscrepancies.forEach((d, i) => console.log(`  [${i}] ${d.id}: ${d.type}`));

    
    // For Operations, also analyze primitive parameter discrepancies (casing mismatches)
    let primitiveParamDiscrepancies: IFieldDiscrepancy[] = [];
    if (element.specialization === "Operation") {
        console.log(`[syncDtoFields] Operation detected, analyzing primitive parameter discrepancies...`);
        primitiveParamDiscrepancies = engine.analyzePrimitiveParameterDiscrepancies(element, entity, parameterMappings);
        console.log(`[syncDtoFields] Primitive param discrepancies: ${primitiveParamDiscrepancies.length}`);
        primitiveParamDiscrepancies.forEach((d, i) => console.log(`  [${i}] ${d.id}: ${d.type}`));
    }

    
    // Combine all discrepancies
    const discrepancies = [...fieldDiscrepancies, ...primitiveParamDiscrepancies];
    console.log(`[syncDtoFields] Total discrepancies after combining: ${discrepancies.length}`);
    
    if (discrepancies.length === 0) {
        await dialogService.info(
            `All fields are synchronized.\n\nThe DTO fields are properly synchronized with the entity '${entity.getName()}' attributes.`
        );
        return;
    }
    
    // Build tree view model with hierarchical structure
    console.log(`[syncDtoFields] Building hierarchical tree nodes...`);
    const treeNodes = engine.buildHierarchicalTreeNodes(element, dtoElement, discrepancies);
    console.log(`[syncDtoFields] Tree nodes built: ${treeNodes.length} root nodes`);
    
    // Present dialog with results - pass element for hierarchical rendering
    const selectedNodeIds = await presentSyncDialog(element, dtoElement, entity, discrepancies, treeNodes);
    console.log(`[syncDtoFields] Selected node IDs: ${selectedNodeIds.length}`);
    
    // Apply sync actions for selected discrepancies
    if (selectedNodeIds.length > 0) {
        const selectedDiscrepancies = discrepancies.filter(d => selectedNodeIds.includes(d.id));
        engine.applySyncActions(dtoElement, entity, selectedDiscrepancies, associations);
        
        await dialogService.info(
            `Synchronization complete.\n\n${selectedDiscrepancies.length} field(s) synchronized successfully.`
        );
    }
}

async function presentSyncDialog(
    sourceElement: MacroApi.Context.IElementApi,
    dtoElement: MacroApi.Context.IElementApi,
    entity: MacroApi.Context.IElementApi,
    discrepancies: IFieldDiscrepancy[],
    treeNodes: MacroApi.Context.ISelectableTreeNode[]
): Promise<string[]> {
    // Determine root display based on source element type
    // If it's an Operation, show the Operation; otherwise show the DTO
    const rootElement = sourceElement.specialization === "Operation" ? sourceElement : dtoElement;
    const rootLabel = sourceElement.specialization === "Operation" 
        ? `${sourceElement.getName()} (${sourceElement.specialization})`
        : dtoElement.getName();
    
    const infoHint = sourceElement.specialization === "Operation"
        ? `Synchronizing operation '${sourceElement.getName()}' with entity '${entity.getName()}' - Found ${discrepancies.length} field discrepancy(ies).`
        : `Comparing DTO '${dtoElement.getName()}' with entity '${entity.getName()}' - Found ${discrepancies.length} field discrepancy(ies).`;
    
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
                hint: infoHint,
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
    let selectedIds: string[] = [];
    if (typeof selectedValue === 'string') {
        // Single selection or comma-separated string
        selectedIds = selectedValue.split(',').map(id => id.trim()).filter(id => id.length > 0);
    } else if (Array.isArray(selectedValue)) {
        // Multiple selections as array
        selectedIds = selectedValue;
    }
    
    return selectedIds;
}
