/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

const diagramSpecializationId = "8c90aca5-86f4-47f1-bd58-116fe79f5c55";

async function getOrCreateDiagram(folder: IElementApi, repositoryOperation: IElementApi, title: string): Promise<IDiagramApi> {
    let diagrams = lookupTypesOf("Diagram", false);
    const newDiagramOption = "create-new-diagram";
    const noDiagramOption = "no-diagram";
    const repositoryId = repositoryOperation.getParent();

    let dialogResult = await dialogService.openForm({
        title: title,
        fields: [
            {
                id: "diagramId",
                fieldType: "select",
                label: "Add to Diagram",
                selectOptions: [{
                    id: noDiagramOption,
                    description: "<None>",
                },
                {
                    id: newDiagramOption,
                    description: "<Create New Diagram>",
                }].concat(diagrams.map(x => {
                    return {
                        id: x.id,
                        description: x.getName(),
                    };
                })),
                value: getCurrentDiagram()?.getOwner()?.id ?? noDiagramOption,
                isRequired: true
            },
        ]
    });

    if (dialogResult.diagramId == noDiagramOption) {
        return null;
    }

    let diagramElement = dialogResult.diagramId == newDiagramOption
        ? createElement(diagramSpecializationId, folder.getName(), folder.id)
        : lookup(dialogResult.diagramId);

    await diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();

    if (!diagram.getVisual(repositoryId)) {
        diagram.layoutVisuals(repositoryId);
    }

    return diagram;
}